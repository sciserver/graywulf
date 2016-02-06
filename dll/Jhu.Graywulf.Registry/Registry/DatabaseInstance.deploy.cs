using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Security.AccessControl;
using smo = Microsoft.SqlServer.Management.Smo;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements extension methods for the <b>DatabaseInstance</b> class
    /// to provide SMO based deployment of databases.
    /// </summary>
    public partial class DatabaseInstance
    {
        /// <summary>
        /// Allocates database files and creates file groups without copying
        /// database objects
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        public override void Deploy()
        {
            // Make sure it's new or undeployed
            if (this.DeploymentState != DeploymentState.New &&
                this.DeploymentState != DeploymentState.Undeployed)
            {
                throw new DeployException(ExceptionMessages.CannotDeployDatabase);
            }

            // Change deployment state to deploying
            this.DeploymentState = DeploymentState.Deploying;
            this.Save();

            // Get SMO object to the target database
            smo::Server sto = this.ServerInstance.GetSmoServer();
            smo::Database dto = new smo::Database(sto, this.DatabaseName);
            
            // Important non-default settings
            dto.RecoveryModel = smo.RecoveryModel.Simple;
            dto.Collation = "SQL_Latin1_General_CP1_CI_AS";

            // --- Delete old LogFiles, FileGroups and create new ones ---
            DropLogFiles(dto);
            DropFileGroups(dto);
            this.CreateFileGroups(dto);

            // Create the empty database with filegroups and files
            dto.Create();
            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Deploy[Create database]", this.Guid));

            // Change deployment state to deployed
            this.DeploymentState = DeploymentState.Deployed;
            this.RunningState = RunningState.Attached;
            this.Save();

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Deploy[Done]", this.Guid));
        }

        /// <summary>
        /// Drops a database.
        /// </summary>
        /// <param name="databaseInstance">The database instance cluster schema object.</param>
        public override void Undeploy()
        {
            // Make sure it's new or undeployed
            if (this.DeploymentState != DeploymentState.Deployed)
            {
                throw new DeployException(ExceptionMessages.CannotUndeployDatabase);
            }

            // Change deployment state to deploying
            this.DeploymentState = DeploymentState.Undeploying;
            this.Save();

            switch ((RunningState)this.RunningState)
            {
                case RunningState.Attached:
                    // Drop database using SMO
                    smo::Database d = this.GetSmoDatabase();
                    d.Parent.KillDatabase(d.Name);
                    
                    this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Undeploy[Drop database]", this.Guid));
                    break;
                case RunningState.Detached:
                    // Database not attached, simply delete files
                    this.LoadFileGroups(false);
                    foreach (DatabaseInstanceFileGroup fg in this.FileGroups.Values)
                    {
                        fg.LoadAllChildren();
                        foreach (DatabaseInstanceFile f in fg.Files.Values)
                        {
                            File.Delete(f.GetFullLocalFilename());
                        }
                    }
                    this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Undeploy[Delete files]", this.Guid));
                    break;
                default:
                    throw new NotImplementedException();
            }


            // Change deployment state to undeployed
            this.DeploymentState = DeploymentState.Undeployed;
            this.RunningState = (int)RunningState.Unknown;
            this.Save();

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Undeploy[Done]", this.Guid));
        }

        public void Detach()
        {
            if (this.DeploymentState != DeploymentState.Deployed ||
                this.RunningState != RunningState.Attached)
            {
                throw new DeployException(ExceptionMessages.CannotDetachDatabase);
            }

            SqlConnectionStringBuilder csb = this.GetConnectionString();
            string dbname = csb.InitialCatalog;
            csb.InitialCatalog = String.Empty;  // connect to master to be able to detach the particular db

            using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                string sql = String.Format(
                    "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db '{0}', TRUE;",
                    dbname);

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            this.RunningState = RunningState.Detached;
            this.Save();

            // Reset permission on database files to reflect that of the folder

            this.LoadFileGroups(true);

            this.LoadAllChildren();
            foreach (DatabaseInstanceFileGroup fg in this.FileGroups.Values)
            {
                fg.LoadAllChildren();
                foreach (DatabaseInstanceFile f in fg.Files.Values)
                {
                    ResetFilePermission(f.GetFullUncFilename());    
                }
            }

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Detach", this.Guid));
        }

        private static void ResetFilePermission(string filename)
        {
            FileSecurity fs = File.GetAccessControl(filename);

            fs.SetAccessRuleProtection(false, false);

            File.SetAccessControl(filename, fs);
        }

        public void Attach()
        {
            Attach(true);
        }

        public void Attach(bool attachAsReadOnly)
        {
            if (this.RunningState == RunningState.Attached)
            {
                throw new DeployException(ExceptionMessages.CannotAttachDatabase);
            }

            // collect filenames for attach
            System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();

            this.LoadAllChildren();
            foreach (DatabaseInstanceFileGroup fg in this.FileGroups.Values)
            {
                fg.LoadAllChildren();
                foreach (DatabaseInstanceFile f in fg.Files.Values)
                {
                    // Check if file exists and don't attempt to attach if not.
                    // Useful when database files are deleted manually but are still in the registry.
                    // Attach will fail anyway, if a database file is really missing.

                    if (File.Exists(f.GetFullUncFilename()))
                    {
                        files.Add(f.GetFullLocalFilename());
                    }
                }
            }

            smo::Server server = this.ServerInstance.GetSmoServer();

            server.AttachDatabase(this.DatabaseName, files);

            if (attachAsReadOnly)
            {
                var db = GetSmoDatabase();
                db.ReadOnly = true;
            }

            this.DeploymentState = DeploymentState.Deployed;
            this.RunningState = RunningState.Attached;
            this.Save();

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Attach", this.Guid));
        }

        /// <summary>
        /// Drops log files from the database.
        /// </summary>
        /// <param name="dto">The SMO database object.</param>
        private static void DropLogFiles(smo::Database dto)
        {
            while (dto.LogFiles.Count > 0)
            {
                dto.LogFiles[0].Drop();
            }
        }

        /// <summary>
        /// Drops file groups from the database
        /// </summary>
        /// <param name="dto">The SMO database object.</param>
        private static void DropFileGroups(smo::Database dto)
        {
            while (dto.FileGroups.Count > 0)
            {
                dto.FileGroups[0].Drop();
            }
        }

        /* *** TODO: delete
        /// <summary>
        /// Adds files to a database based on the cluster schema information.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        private void CreateFiles(smo::Database dto)
        {
            this.LoadFiles(false);
            foreach (DatabaseInstanceFile f in this.Files.Values)
            {
                f.DeploymentState = DeploymentState.Deploying; f.Save();

                string localFilename = f.GetFullLocalFilename();
                string networkFilename = f.GetFullUncFilename();

                // Check directory
                string dir = Path.GetDirectoryName(networkFilename);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Create new file logfile
                if (f.DatabaseFileType == DatabaseFileType.Log)
                {
                    smo::LogFile lf = new smo::LogFile(dto, f.Filename);

                    lf.FileName = localFilename;
                    lf.Growth = 0;
                    lf.GrowthType = smo::FileGrowthType.None;
                    //nf.MaxSize = (double)fi.AllocatedSpace / 0x400; // in kilobytes
                    lf.Size = (double)f.AllocatedSpace / 0x400; // in kilobytes

                    dto.LogFiles.Add(lf);
                }

                f.DeploymentState = DeploymentState.Deployed; f.Save();
            }
        }*/

        /// <summary>
        /// Adds file group to a database based on the cluster schema information.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        private void CreateFileGroups(smo::Database dto)
        {
            // Add FileGroups
            this.LoadFileGroups(false);
            foreach (DatabaseInstanceFileGroup fg in this.FileGroups.Values)
            {
                fg.DeploymentState = DeploymentState.Deploying;
                fg.Save();

                if (fg.FileGroupType == FileGroupType.Log)
                {
                    fg.LoadFiles(false);
                    foreach (DatabaseInstanceFile fi in fg.Files.Values)
                    {
                        fi.DeploymentState = DeploymentState.Deploying;
                        fi.Save();

                        string localFilename = fi.GetFullLocalFilename();
                        string networkFilename = fi.GetFullUncFilename();

                        // Check directory
                        string dir = Path.GetDirectoryName(networkFilename);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        // Create new File
                        smo::LogFile lf = new smo::LogFile(dto, fi.LogicalName);
                        lf.FileName = localFilename;
                        lf.Growth = 0;
                        lf.GrowthType = smo::FileGrowthType.None;
                        //nf.MaxSize = (double)fi.AllocatedSpace / 0x400; // in kilobytes
                        lf.Size = Math.Max(4096, (double)fi.AllocatedSpace / 0x400); // in kilobytes

                        dto.LogFiles.Add(lf);

                        fi.DeploymentState = DeploymentState.Deployed;
                        fi.Save();
                    }
                }
                else if (fg.FileGroupType == FileGroupType.Data)
                {
                    // Create new File Group
                    smo::FileGroup nfg = new smo::FileGroup(dto, fg.FileGroupName);

                    // Add files to the File Group
                    fg.LoadFiles(false);
                    foreach (DatabaseInstanceFile fi in fg.Files.Values)
                    {
                        fi.DeploymentState = DeploymentState.Deploying; fi.Save();

                        string localFilename = fi.GetFullLocalFilename();
                        string networkFilename = fi.GetFullUncFilename();

                        // Check directory
                        string dir = Path.GetDirectoryName(networkFilename);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        // Create new File
                        smo::DataFile nf = new smo::DataFile(nfg, fi.LogicalName);
                        nf.FileName = localFilename;
                        nf.Growth = 0;
                        nf.GrowthType = smo::FileGrowthType.None;
                        //nf.MaxSize = (double)fi.AllocatedSpace / 0x400; // in kilobytes
                        nf.Size = Math.Max(4096, (double)fi.AllocatedSpace / 0x400); // in kilobytes

                        nfg.Files.Add(nf);

                        fi.DeploymentState = DeploymentState.Deployed; fi.Save();
                    }

                    // Add new File Group to the Database
                    dto.FileGroups.Add(nfg);
                }
                else
                {
                    throw new NotImplementedException();
                }

                fg.DeploymentState = DeploymentState.Deployed; fg.Save();
            }

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.CreateFileGroups", this.Guid));
        }

        // ****

#if false
        /* Old code to copy schema with/without smo
        /// <summary>
        /// Deploys a new database.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        public static void Deploy(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance)
        {
            // Change deployment state to deploying
            databaseInstance.DeploymentState = DeploymentState.Deploying;
            databaseInstance.Save();

            // Get SMO object to the template database
            smo::Database dfrom = databaseInstance.DatabaseDefinition.GetSmoDatabase();
            dfrom.PrefetchObjects();

            // Get SMO object to the target database
            smo::Server sto = databaseInstance.ServerInstance.GetSmoServer();
            smo::Database dto = new smo::Database(sto, databaseInstance.DatabaseName);

            dto.Copy(dfrom, false, true);

            // --- Delete old LogFiles, FileGroups and create new ones ---
            DropLogFiles(dto);
            DropFileGroups(dto);
            databaseInstance.CreateFiles(dto);
            databaseInstance.CreateFileGroups(dto);

            //// (SQL Transactions are not used here)
            //// databaseInstance.Context.CommitTransaction();

#if TODO_DELETE_IF_UNUSED
            // Create the empty database with filegroups and files

            /* Workaround:  In SQL Server 2008, CREATE DATABASE (which is generated by the following call to Create)
             * is subject to a deadlock condition between master.sys.sysdbreg and database model.  Serializing calls
             * to Create using a global mutex is reasonable assuming all callers are executing on the same machine
             * (and the same machine as the target database).
             */

            // get a reference to a global (system-scope) mutex
            Mutex m = new Mutex( false, "Global/Jhu_Graywulf_Schema_Deploy_DatabaseInstanceExtensions" );

            try
            {
                /* wait 120 seconds for any pending calls to Create to complete; this timeout value
                    should be reasonable assuming it takes no more than 10 seconds to create a database */
                int msTimeout = 120 * 1000;
                if (m.WaitOne(msTimeout))
                {
                    dto.Create();
                }
                else
                    throw new ApplicationException(string.Format("timeout ({0}ms) waiting for mutex", msTimeout));
            }
            finally
            {
                if (m != null)
                    m.ReleaseMutex();
            }

            /* (if the above code is used, remove the following call to Create...)
             */
#endif

            // Create the empty database with filegroups and files
            dto.Create();
            databaseInstance.Context.LogEvent( new Event( "Jhu.Graywulf.Registry.DatabaseInstance.Deploy[Create database]", databaseInstance.Guid ) );

            // Create schemas
            CreateSchemas(dto);

            // Create assemblies
            dto.Assemblies.Copy(dfrom.Assemblies);

            // Generate partitioning objects
            if (databaseInstance.DatabaseDefinition.LayoutType == DatabaseLayoutType.Sliced)
            {
                databaseInstance.CreatePartitionFunctions(dto);
                databaseInstance.CreatePartitionSchemes(dto);
            }

            // Copy all other objects (tables, views, sps etc.)
            CopyObjects(dfrom, dto);
            databaseInstance.Context.CommitTransaction();

            // Create DPVs
            databaseInstance.CreateDistributedPartitionedViews(dto, true);

            // Copy security settings
            // TODO: make if faster
            CopyObjectPermissions(dfrom, dto);
            databaseInstance.Context.CommitTransaction();

            // Change deployment state to deployed
            databaseInstance.DeploymentState = DeploymentState.Deployed;
            databaseInstance.Save();

            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.Deploy[Done]", databaseInstance.Guid));
        }
#endif
#if false

       // TODO: old code for schema copy, reuse if needed
        // but would be the best to rewrite to use scripts
        

        /// <summary>
        /// Creates schemas in a database based on the template database.
        /// </summary>
        /// <param name="dto">The SMO database object pointing to the target database.</param>
        private static void CreateSchemas(smo::Database dto)
        {
            foreach (Microsoft.SqlServer.Management.Smo.Schema s in dto.Schemas)
            {
                // TODO IT'S A HACK, FIGURE OUT HOW TO CREATE ONLY USER SCHEMAS!!!
                // try is req'd, s.Create() throws exception when trying to create sys schemas
                try
                {
                    s.Create();
                }
                catch (System.Exception)
                {
                }
            }
        }

        /// <summary>
        /// Creates the main partition function based on the cluster schema information.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        /// <remarks>
        /// This function is only used for creating the main partition function, i.e. the one
        /// defined in the database definition entity. All other partition functions are simply
        /// copied from the template database.
        /// </remarks>
        private static void CreatePartitionFunctions(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance, smo::Database dto)
        {
            databaseInstance.Slice.LoadPartitions(false);

            List<Partition> partitions = new List<Partition>(databaseInstance.Slice.Partitions);
            object[] ranges = new object[partitions.Count - 1];
            // TODO make distinction between LEFT and RIGHT ?
            for (int i = 0; i < ranges.Length; i++)
            {
                ranges[i] = partitions[i].To;
            }

            foreach (smo::PartitionFunction pf in dto.PartitionFunctions)
            {

                if (pf.Name == databaseInstance.DatabaseDefinition.PartitionFunction)
                {
                    pf.RangeType = (smo::RangeType)databaseInstance.DatabaseDefinition.PartitionRangeType;
                    pf.RangeValues = ranges;
                }

                pf.Create();
            }
            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.CreatePartitionFunctions", databaseInstance.Guid));
        }

        /// <summary>
        /// Creates the partition schemes based on the cluster schema and template database information.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        private static void CreatePartitionSchemes(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance, smo::Database dto)
        {
            databaseInstance.DatabaseDefinition.LoadFileGroups(false);

            foreach (smo::PartitionScheme ps in dto.PartitionSchemes)
            {
                if (ps.PartitionFunction == databaseInstance.DatabaseDefinition.PartitionFunction)
                {
                    // Ensure that Partition Schemes are mapped to a single File Group
                    if (ps.FileGroups.Count != 1)
                    {
                        throw new Exception();    // TODO create typed exception
                    }

                    // Look up file group in cluster description
                    FileGroup fg = databaseInstance.DatabaseDefinition.FileGroups.First<FileGroup>(f => f.FileGroupName == ps.FileGroups[0]);

                    // Clear old file group mapping
                    ps.FileGroups.Clear();

                    // Add new file group mappings
                    foreach (DatabaseInstanceFileGroup ffg in databaseInstance.FileGroups.Where<DatabaseInstanceFileGroup>(f => f.FileGroupReference.Guid == fg.Guid))
                    {
                        ps.FileGroups.Add(ffg.Name); // TODO replace display name with something else
                    }
                }

                ps.Create();
            }
            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.CreatePartitionSchemes", databaseInstance.Guid));
        }

        /// <summary>
        /// Creates the distributed partitioned views in the target database based on the cluster schema information.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        /// <param name="recreateLinkedServers">If true, all linked server connections will be recreated.</param>
        private static void CreateDistributedPartitionedViews(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance, smo::Database dto, bool recreateLinkedServers)
        {
            smo::Database db = databaseInstance.GetSmoDatabase();

            databaseInstance.DatabaseDefinition.LoadDistributedPartitionedViews(false);
            foreach (DistributedPartitionedView dpv in databaseInstance.DatabaseDefinition.DistributedPartitionedViews)
            {
                if (dpv.RedundancyState.Guid == databaseInstance.RedundancyState.Guid)
                {
                    dpv.Deploy(db, databaseInstance, recreateLinkedServers);
                }
            }
            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.CreateDistributedPartitionedViews", databaseInstance.Guid));
        }

        /// <summary>
        /// Copies SQL database schema object from the template database to the target database.
        /// </summary>
        /// <param name="dfrom">The SMO object pointing to the template (source) database.</param>
        /// <param name="dto">The SMO object pointing to the target database.</param>
        private static void CopyObjects(smo::Database dfrom, smo::Database dto)
        {
            // Collect objects for dependency check
            List<smo::SqlSmoObject> depobjs = new List<smo::SqlSmoObject>();

            foreach (smo::Table i in dfrom.Tables) { if (!i.IsSystemObject) depobjs.Add(i); }
            foreach (smo::View i in dfrom.Views) { if (!i.IsSystemObject) depobjs.Add(i); }
            foreach (smo::StoredProcedure i in dfrom.StoredProcedures) { if (!i.IsSystemObject) depobjs.Add(i); }
            foreach (smo::UserDefinedFunction i in dfrom.UserDefinedFunctions) { if (!i.IsSystemObject) depobjs.Add(i); }

            //UserDefinedFunction, View, Table, StoredProcedure, Default, Rule, Trigger, UserDefinedAggregate, Synonym, UserDefinedDataType, XmlSchemaCollection, UserDefinedType, PartitionScheme, PartitionFunction, SqlAssembly

            if (depobjs.Count == 0)
            {
                throw new DeployException(ExceptionMessages.NoObjectsToGenerate);
            }

            // Create dependency tree and generate schema
            smo::Scripter src = new smo::Scripter(dfrom.Parent);
            smo::DependencyTree tree = src.DiscoverDependencies(depobjs.ToArray(), true);

            smo::DependencyWalker depwalker = new smo::DependencyWalker();
            smo::DependencyCollection depcoll = depwalker.WalkDependencies(tree);

            foreach (smo::DependencyCollectionNode dep in depcoll)
            {
                if (dep.Urn.Type == "Table")
                {
                    dto.Tables[dep.Urn.GetAttribute("Name"), dep.Urn.GetAttribute("Schema")].Create();
                    continue;
                }
                if (dep.Urn.Type == "View")
                {
                    // TODO dummy DPVs cannot be generated because contain reference to another template DB
                    // Have to work out the way of creating dummy DPVs in template DBs
                    try
                    {
                        dto.Views[dep.Urn.GetAttribute("Name"), dep.Urn.GetAttribute("Schema")].Create();
                    }
                    catch (System.Exception)
                    {
                    }
                    continue;
                }
                if (dep.Urn.Type == "StoredProcedure")
                {
                    dto.StoredProcedures[dep.Urn.GetAttribute("Name"), dep.Urn.GetAttribute("Schema")].Create();
                    continue;
                }
                if (dep.Urn.Type == "UserDefinedFunction")
                {
                    dto.UserDefinedFunctions[dep.Urn.GetAttribute("Name"), dep.Urn.GetAttribute("Schema")].Create();
                    continue;
                }
            }
        }

        /// <summary>
        /// Copies data from the template database tables to the target database tables.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <remarks>
        /// Tables in the template database should have the 'meta.CopyData' extended property
        /// set to 1 in order to copy them to the target database.
        /// </remarks>
        public static void CopyTableData(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance)
        {
            string cstrfrom = databaseInstance.DatabaseDefinition.GetConnectionString().ConnectionString;
            string cstrto = databaseInstance.GetConnectionString().ConnectionString;

            smo::Database d = databaseInstance.GetSmoDatabase();

            foreach (smo::Table t in d.Tables)
            {
                if (t.ExtendedProperties.Contains("meta.CopyData") && Convert.ToInt32(t.ExtendedProperties["meta.CopyData"].Value) != 0)
                {
                    CopyTableData(cstrfrom, cstrto, t.Schema, t.Name);
                }
            }
            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.CopyTables", databaseInstance.Guid));
        }

        /// <summary>
        /// Copies data from a template database table to the target database table.
        /// </summary>
        /// <param name="cstrFrom">Connection string to the template database.</param>
        /// <param name="cstrTo">Connection string to the target database.</param>
        /// <param name="schema">Schema of the table to be copied.</param>
        /// <param name="table">Name of the table to be copied.</param>
        private static void CopyTableData(string cstrFrom, string cstrTo, string schema, string table)
        {
            /* Workaround: The following is a workaround for a sporadic timeout problem that was observed
                with the SMO-based implementation below.
            */ 
            SqlConnectionStringBuilder scsbFrom = new SqlConnectionStringBuilder( cstrFrom );
            string sql = string.Format( @"insert [{3}] with (tablockx)
                                          select *
                                            from [{0}].[{1}].[{2}].[{3}] with (nolock)",
                                        scsbFrom.DataSource,
                                        scsbFrom.InitialCatalog,
                                        schema,       // SQL schema name (in source DB)
                                        table );      // table name (in source and target DBs)

            using( SqlConnection cn = new SqlConnection( cstrTo ) )
            {
                cn.Open();
                using( SqlCommand cmd = new SqlCommand( sql, cn ) )
                {
                    cmd.CommandTimeout = 30;
                    cmd.ExecuteNonQuery();
                }

                cn.Close();
            }

#if SEE_ABOVE_COMMENTS
            using (SqlConnection cn = new SqlConnection(cstrFrom))
            {
                cn.Open();

                string sql = string.Format("SELECT * FROM [{0}].[{1}] with (nolock)", schema, table);
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    using (IDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        using (SqlBulkCopy copy = new SqlBulkCopy(cstrTo,SqlBulkCopyOptions.TableLock))
                        {
                            copy.DestinationTableName = table;
                            copy.WriteToServer(dr);
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }
#endif
        }

        /// <summary>
        /// Copies object permissions from the template database to the target database.
        /// </summary>
        /// <param name="dfrom">SMO object pointing to the template database.</param>
        /// <param name="dto">SMO object pointing to the target database.</param>
        private static void CopyObjectPermissions(smo::Database dfrom, smo::Database dto)
        {
            foreach (smo::Schema i in dto.Schemas) { i.CopyObjectPermissions(dfrom.Schemas[i.Name]); }
            foreach (smo::DatabaseRole i in dto.Roles) { i.CopyObjectPermissions(dfrom.Roles[i.Name]); }
            foreach (smo::User i in dto.Users) { i.CopyObjectPermissions(dfrom.Users[i.Name]); }
            foreach (smo::Table i in dto.Tables) { i.CopyObjectPermissions(dfrom.Tables[i.Name, i.Schema]); }
            foreach (smo::View i in dto.Views) { i.CopyObjectPermissions(dfrom.Views[i.Name, i.Schema]); }
            foreach (smo::StoredProcedure i in dto.StoredProcedures) { i.CopyObjectPermissions(dfrom.StoredProcedures[i.Name, i.Schema]); }
            foreach (smo::UserDefinedFunction i in dto.UserDefinedFunctions) { i.CopyObjectPermissions(dfrom.UserDefinedFunctions[i.Name, i.Schema]); }
        }

        /// <summary>
        /// Populates the IndexMap table of the target database with description of the
        /// indices found in the template database.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <remarks>
        /// Instead of using the IndexMap table, a better way is to get indices directly from
        /// the template database.
        /// </remarks>
        public static void PopulateIndexMapTable(this Jhu.Graywulf.Registry.DatabaseInstance databaseInstance)
        {
            using (SqlConnection cn = new SqlConnection(databaseInstance.GetConnectionString().ConnectionString))
            {
                cn.Open();

                string sql = "TRUNCATE TABLE meta.IndexMap";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }

                sql = @"meta.spAddIndexMapEntry";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                    cmd.Parameters.Add("@IndexGroup", SqlDbType.NVarChar, 50);
                    cmd.Parameters.Add("@Type", SqlDbType.Char, 1);
                    cmd.Parameters.Add("@TableSchema", SqlDbType.NVarChar, 128);
                    cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 128);
                    cmd.Parameters.Add("@ColumnList", SqlDbType.NVarChar, 1024);
                    cmd.Parameters.Add("@ReferencedTableSchema", SqlDbType.NVarChar, 128);
                    cmd.Parameters.Add("@ReferencedTableName", SqlDbType.NVarChar, 128);
                    cmd.Parameters.Add("@ReferencedKey", SqlDbType.NVarChar, 128);



                    smo::Database dfrom = databaseInstance.DatabaseDefinition.GetSmoDatabase();
                    foreach (smo::Table t in dfrom.Tables)
                    {
                        // --- Process indices

                        foreach (smo::Index i in t.Indexes)
                        {
                            string collist = string.Empty;
                            int q = 0;
                            foreach (smo::IndexedColumn c in i.IndexedColumns)
                            {
                                if (q > 0) collist += ",";
                                collist += c.Name;
                                q++;
                            }

                            char type = ' ';
                            switch (i.IndexKeyType)
                            {
                                case Microsoft.SqlServer.Management.Smo.IndexKeyType.None:
                                    type = 'I';
                                    break;
                                case smo::IndexKeyType.DriPrimaryKey:
                                    type = 'K';
                                    break;
                                case smo::IndexKeyType.DriUniqueKey:
                                    type = 'U';
                                    break;
                            }

                            cmd.Parameters["@Name"].Value = i.Name;
                            cmd.Parameters["@IndexGroup"].Value = i.ExtendedProperties.Contains("IndexGroup") ? (string)i.ExtendedProperties["IndexGroup"].Value : string.Empty;
                            cmd.Parameters["@Type"].Value = type;
                            cmd.Parameters["@TableSchema"].Value = t.Schema;
                            cmd.Parameters["@TableName"].Value = t.Name;
                            cmd.Parameters["@ColumnList"].Value = collist;
                            cmd.Parameters["@ReferencedTableSchema"].Value = string.Empty;
                            cmd.Parameters["@ReferencedTableName"].Value = string.Empty;
                            cmd.Parameters["@ReferencedKey"].Value = string.Empty;

                            cmd.ExecuteNonQuery();
                        }

                        // --- Process foreign keys


                        foreach (smo::ForeignKey f in t.ForeignKeys)
                        {
                            string collist = string.Empty;
                            foreach (smo::ForeignKeyColumn c in f.Columns)
                            {
                                if (collist != string.Empty) collist += ",";
                                collist += c.Name;
                            }

                            cmd.Parameters["@Name"].Value = f.Name;
                            cmd.Parameters["@IndexGroup"].Value = f.ExtendedProperties.Contains("IndexGroup") ? (string)f.ExtendedProperties["IndexGroup"].Value : string.Empty;
                            cmd.Parameters["@Type"].Value = 'F';
                            cmd.Parameters["@TableSchema"].Value = t.Schema;
                            cmd.Parameters["@TableName"].Value = t.Name;
                            cmd.Parameters["@ColumnList"].Value = collist;
                            cmd.Parameters["@ReferencedTableSchema"].Value = f.ReferencedTableSchema;
                            cmd.Parameters["@ReferencedTableName"].Value = f.ReferencedTable;
                            cmd.Parameters["@ReferencedKey"].Value = f.ReferencedKey;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            databaseInstance.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DatabaseInstance.PopulateIndexMapTables", databaseInstance.Guid));
        }


        public static void CopyFiles(this Jhu.Graywulf.Registry.DatabaseInstance from, Jhu.Graywulf.Registry.DatabaseInstance to)
        {
            // Execute check steps to make sure copy is possible

            //
            from.LoadChildren();
            to.LoadChildren();

            // Copy files one by one from source machine to destination machine
            List<DatabaseInstanceFile> fromfiles = new List<DatabaseInstanceFile>(from.Files);
            List<DatabaseInstanceFile> tofiles = new List<DatabaseInstanceFile>(to.Files);

            foreach (DatabaseInstanceFileGroup fg in from.FileGroups)
            {
                fg.LoadChildren();
                fromfiles.AddRange(fg.Files);
            }
            foreach (DatabaseInstanceFileGroup fg in to.FileGroups)
            {
                fg.LoadChildren();
                tofiles.AddRange(fg.Files);
            }

            from.Context.CommitTransaction();

            // Copy one-by-one
            BulkOpClient bc = new BulkOpClient();
            bc.ServerName = from.ServerInstance.Machine.ServerName;

            for (int i = 0; i < fromfiles.Count; i++)
            {
                Console.WriteLine("File copy:\r\n\t{0}\r\n\t{1}", fromfiles[i].GetFullUncFilename(), tofiles[i].GetFullUncFilename());
                
                bc.CopyFile(fromfiles[i].GetFullUncFilename(), tofiles[i].GetFullUncFilename(), true);
                tofiles[i].DeploymentState = DeploymentState.Detached;
                tofiles[i].Save();
            }

            foreach (DatabaseInstanceFileGroup fg in to.FileGroups)
            {
                fg.DeploymentState = DeploymentState.Detached;
                fg.Save();
            }

            //
            to.DeploymentState = DeploymentState.Detached;
            to.Save();
        }

#endif

    }
}
