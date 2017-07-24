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

            // Delete old LogFiles, FileGroups and create new ones ---
            DropLogFiles(dto);
            DropFileGroups(dto);
            this.CreateFileGroups(dto);

            // Create directory for new files, if necessary
            foreach (var fg in FileGroups.Values)
            {
                foreach (var f in fg.Files.Values)
                {
                    var dir = Path.GetDirectoryName(f.GetFullLocalFilename());
                    Directory.CreateDirectory(dir);
                }
            }

            // Create the empty database with filegroups and files
            dto.Create();
            LogDebug(String.Format("Created database {0} on {1}.", DatabaseName, ServerInstance.GetCompositeName()));

            // Change deployment state to deployed
            this.DeploymentState = DeploymentState.Deployed;
            this.RunningState = RunningState.Attached;
            this.Save();

            LogDebug();
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

                    LogDebug(String.Format("Dropped database {0} on {1}.", DatabaseName, ServerInstance.GetCompositeName()));

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

                    LogDebug(String.Format("Deleted database files for {0} on {1}.", DatabaseName, ServerInstance.GetCompositeName()));

                    break;
                default:
                    throw new NotImplementedException();
            }


            // Change deployment state to undeployed
            this.DeploymentState = DeploymentState.Undeployed;
            this.RunningState = (int)RunningState.Unknown;
            this.Save();

            LogDebug();
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

            LogDebug();
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
                    files.Add(f.GetFullLocalFilename());
                }
            }

            smo::Server server = this.ServerInstance.GetSmoServer();

            server.AttachDatabase(this.DatabaseName, files);

            if (attachAsReadOnly)
            {
                var db = GetSmoDatabase();
                db.ReadOnly = true;
                db.Alter();
            }

            this.DeploymentState = DeploymentState.Deployed;
            this.RunningState = RunningState.Attached;
            this.Save();

            LogDebug();
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

            LogDebug();
        }

        public void Allocate()
        {
            Deploy();
        }

        public void Drop()
        {
            Undeploy();
        }
    }
}
