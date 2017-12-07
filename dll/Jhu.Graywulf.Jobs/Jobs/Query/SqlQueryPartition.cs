using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryPartition : QueryObject, ICloneable
    {
        #region Property storage variables

        private int id;

        private SqlQuery query;

        private IComparable partitioningKeyMin;
        private IComparable partitioningKeyMax;

        [NonSerialized]
        private Dictionary<string, TableReference> remoteTableReferences;

        #endregion
        #region Properties

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public SqlQuery Query
        {
            get { return query; }
        }

        public IComparable PartitioningKeyMin
        {
            get { return partitioningKeyMin; }
            set { partitioningKeyMin = value; }
        }

        public IComparable PartitioningKeyMax
        {
            get { return partitioningKeyMax; }
            set { partitioningKeyMax = value; }
        }

        public Dictionary<string, TableReference> RemoteTableReferences
        {
            get { return remoteTableReferences; }
        }

        [IgnoreDataMember]
        private SqlQueryCodeGenerator CodeGenerator
        {
            get { return (SqlQueryCodeGenerator)CreateCodeGenerator(); }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryPartition()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryPartition(SqlQuery query)
            : base(query)
        {
            InitializeMembers(new StreamingContext());

            this.query = query;
        }

        public SqlQueryPartition(SqlQueryPartition old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.id = 0;

            this.query = null;

            this.partitioningKeyMin = null;
            this.partitioningKeyMax = null;

            this.remoteTableReferences = new Dictionary<string, TableReference>(SchemaManager.Comparer);
        }

        private void CopyMembers(SqlQueryPartition old)
        {
            this.id = old.id;

            this.query = old.query;

            this.partitioningKeyMin = old.partitioningKeyMin;
            this.partitioningKeyMax = old.partitioningKeyMax;

            this.remoteTableReferences = new Dictionary<string, TableReference>(old.remoteTableReferences, SchemaManager.Comparer);
        }

        public override object Clone()
        {
            return new SqlQueryPartition(this);
        }

        #endregion

        protected virtual SqlServerCodeGenerator CreateCodeGenerator()
        {
            return new SqlQueryCodeGenerator(this);
        }

        protected override void FinishInterpret(bool forceReinitialize)
        {
        }

        #region Remote table caching functions

        /// <summary>
        /// Finds those tables that are required to execute the query but had to be
        /// copied from a remote source
        /// </summary>
        /// <returns></returns>
        public void FindRemoteTableReferences()
        {
            if (ExecutionMode == ExecutionMode.Graywulf)
            {
                var sc = GetSchemaManager();

                foreach (var tr in SelectStatement.EnumerateSourceTableReferences(true))
                {
                    if (tr.IsCachable && !remoteTableReferences.ContainsKey(tr.UniqueName) &&
                        IsRemoteDataset(sc.Datasets[tr.DatasetName]))
                    {
                        remoteTableReferences.Add(tr.UniqueName, tr);
                    }
                }
            }
            else
            {
                // nothing to do here
            }
        }

        /// <summary>
        /// Composes a source query for a remote table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public void PrepareCopyRemoteTable(TableReference table, out SourceTableQuery query)
        {
            // -- Load schema
            var sm = GetSchemaManager();
            var ds = sm.Datasets[table.DatasetName];

            // Graywulf dataset has to be converted to prevent registry access
            if (ds is GraywulfDataset)
            {
                ds = new SqlServerDataset(ds);
            }

            // Generate most restrictive query
            // Use code generator specific to the remote database platform!
            // TODO: For some reason, the remote table object contains referenced columns
            // only and column context is not set for them, so we need to generate
            // the column list from all available columns. Check this and figure
            // out why columns are not resolved.
            var cg = SqlCodeGeneratorFactory.CreateCodeGenerator(ds);
            var qs = ((TableSource)table.Node).QuerySpecification;
            var sql = cg.GenerateMostRestrictiveTableQuery(qs, table, ColumnContext.All, 0);

            query = new SourceTableQuery()
            {
                Dataset = ds,
                Query = sql
            };
        }

        /// <summary>
        /// Copies a table from a remote data source by creating and
        /// executing a table copy task.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="source"></param>
        public void CopyRemoteTable(TableReference table, SourceTableQuery source)
        {
            // Create a target table name
            var cg = new SqlServerCodeGenerator();
            var temptable = GetTemporaryTable(cg.GenerateEscapedUniqueName(table));
            TemporaryTables.TryAdd(table.UniqueName, temptable);

            var dest = new DestinationTable(
                temptable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create);

            using (var tc = CreateTableCopyTask(source, dest, false))
            {
                var guid = Guid.NewGuid();
                RegisterCancelable(guid, tc.Value);

                tc.Value.Execute();

                UnregisterCancelable(guid);
            }
        }

        /// <summary>
        /// Checks if table is a remote table cached locally and if so,
        /// substitutes corresponding temp table name
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string SubstituteRemoteTableName(TableReference tr)
        {
            if (RemoteTableReferences.ContainsKey(tr.UniqueName))
            {
                return CodeGenerator.GetResolvedTableName(TemporaryTables[tr.UniqueName]);
            }
            else
            {
                return CodeGenerator.GetResolvedTableName(tr);
            }
        }

        /// <summary>
        /// Checks if table is a remote table cached locally and if so,
        /// substitutes the corresponding table name and alias for
        /// FROM clauses
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        public string SubstituteRemoteTableNameWithAlias(TableReference tr)
        {
            if (RemoteTableReferences.ContainsKey(tr.UniqueName))
            {
                return CodeGenerator.GetResolvedTableNameWithAlias(TemporaryTables[tr.UniqueName], tr.Alias);
            }
            else
            {
                return CodeGenerator.GetResolvedTableNameWithAlias(tr);
            }
        }

        #endregion
        #region Final query execution

        /// <summary>
        /// Gets a query that can be used to figure out the schema of
        /// the destination table.
        /// </summary>
        /// <returns></returns>
        protected SourceTableQuery GetExecuteSourceQuery()
        {
            // Source query will be run on the code database to have
            // access to UDTs
            var source = CodeGenerator.GetExecuteQuery(SelectStatement);
            source.Dataset = CodeDataset;
            return source;
        }

        public virtual void PrepareExecuteQuery(RegistryContext context, Scheduler.IScheduler scheduler, out SourceTableQuery source, out Table destination)
        {
            InitializeQueryObject(context, scheduler, true);

            // Destination table
            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // In single-server mode results are directly written into destination table
                    destination = Query.Destination.GetTable();
                    break;
                case ExecutionMode.Graywulf:
                    // In graywulf mode results are written into a temporary table first
                    destination = GetOutputTable();
                    TemporaryTables.TryAdd(destination.TableName, destination);

                    // Drop destination table, in case it already exists for some reason
                    destination.Drop();
                    break;
                default:
                    throw new NotImplementedException();
            }

            source = GetExecuteSourceQuery();
        }

        public void ExecuteQuery(SourceTableQuery source, Table destination)
        {
            var dt = new DestinationTable(destination, TableInitializationOptions.Create);

            var insert = new InsertIntoTable()
            {
                Source = source,
                Destination = dt,
            };

            insert.Execute();
        }

        #endregion
        #region Destination table and copy resultset functions

        /// <summary>
        /// Generates the query that can be used to copy the results to the final
        /// destination table (usually in mydb)
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOutputQueryText()
        {
            // TODO move completely to codegen
            return String.Format(
                "SELECT __tablealias.* FROM {0} AS __tablealias",
                CodeGenerator.GetResolvedTableName(GetOutputTable()));
        }

        /// <summary>
        /// Gets a query that can be used to figure out the schema of
        /// the destination table.
        /// </summary>
        /// <returns></returns>
        protected SourceTableQuery GetOutputSourceQuery()
        {
            return new SourceTableQuery()
            {
                Dataset = TemporaryDataset,
                Query = GetOutputQueryText()
            };
        }

        protected void PrepareDestinationTable()
        {
            lock (syncRoot)
            {
                // Only initialize target table if it's still uninitialzed
                if (!query.IsDestinationTableInitialized)
                {
                    var source = GetOutputSourceQuery();

                    // TODO: figure out metadata from query
                    var table = query.Destination.GetQueryOutputTable(BatchName, QueryName, null, null);
                    var columns = source.GetColumns();
                    table.Initialize(columns, query.Destination.Options);

                    // At this point the name of the destination is determined
                    // mark it as the output
                    query.Output = table;
                }

                query.IsDestinationTableInitialized = true;
            }
        }

        /// <summary>
        /// Creates or truncates destination table in the output database (usually MYDB)
        /// </summary>
        /// <remarks>
        /// This has to be in the QueryPartition class because the Query class does not
        /// have information about the database server the partition is executing on and
        /// the temporary tables are required to generate the destination table schema.
        /// 
        /// The destination table is created by the very first partition that gets to
        /// the point of copying results. This is when the name of the target table is
        /// determined in case only a table name pattern is specified and automatic
        /// unique naming is turned on.
        /// </remarks>
        public void PrepareDestinationTable(RegistryContext context, IScheduler scheduler)
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Output is already written to the target table
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    InitializeQueryObject(context, scheduler, true);
                    PrepareDestinationTable();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void PrepareCreateDestinationTablePrimaryKey(RegistryContext context, IScheduler scheduler, out Table destination)
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    throw new NotImplementedException();
                case Jobs.Query.ExecutionMode.Graywulf:
                    {
                        InitializeQueryObject(context, scheduler, true);

                        lock (syncRoot)
                        {
                            destination = query.Destination.GetQueryOutputTable(BatchName, QueryName, null, null);

                            var source = GetExecuteSourceQuery();
                            var columns = source.GetColumns().Where(ci => ci.IsKey).ToArray();

                            if (columns.Length > 0)
                            {
                                var pk = new Index(destination, columns, null, true);
                                destination.Indexes.TryAdd(pk.IndexName, pk);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void CreateDestinationTablePrimaryKey(Table destination)
        {
            if (destination.PrimaryKey != null)
            {
                var sql = CodeGenerator.GenerateCreatePrimaryKeyScript(destination, true);

                using (var cmd = new SqlCommand(sql))
                {
                    // Since it's hard to tell whether an output column is unique
                    // we just attempt to create the PK here and eat the exception
                    // it happens. Users will be able to create a PK themselves if
                    // necessary.

                    try
                    {
                        ExecuteSqlOnDataset(cmd, destination.Dataset);
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1505)
                        {
                            // TODO
                            // Duplicate key, eat exception
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }


        public virtual void PrepareCopyResultset(RegistryContext context)
        {
            this.InitializeQueryObject(context);
        }

        /// <summary>
        /// Copies resultset from the output temporary table to the destination database (MYDB)
        /// </summary>
        public void CopyResultset()
        {
            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Do nothing as execute writes results directly into destination table
                    break;
                case ExecutionMode.Graywulf:
                    {
                        var source = GetOutputSourceQuery();

                        var destination = new DestinationTable(Query.Output, TableInitializationOptions.Append);

                        DumpSqlCommand(source.Query, CommandTarget.Temp);

                        // Create bulk copy task and execute it
                        using (var tc = CreateTableCopyTask(source, destination, false))
                        {
                            var guid = Guid.NewGuid();
                            RegisterCancelable(guid, tc.Value);

                            tc.Value.Execute();

                            UnregisterCancelable(guid);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        #region Temporary table logic

        public override Table GetTemporaryTable(string tableName)
        {
            string tempname;

            switch (ExecutionMode)
            {
                case Jobs.Query.ExecutionMode.SingleServer:
                    tempname = String.Format("skyquerytemp_{0}_{1}", id.ToString(), tableName);
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    tempname = String.Format("{0}_{1}_{2}_{3}", RegistryContext.User.Name, JobContext.Current.JobID, id.ToString(), tableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return GetTemporaryTableInternal(tempname);
        }

        public Table GetOutputTable()
        {
            return GetTemporaryTable("output");
        }

        public void CleanUp(bool suppressErrors)
        {
            DropTemporaryTables(suppressErrors);
            DropTemporaryViews(suppressErrors);
        }

        #endregion
        #region Query execution

        protected internal override string GetDumpFileName(CommandTarget target)
        {
            string server = GetSystemDatabaseConnectionStringOnAssignedServer(target).DataSource;
            server = server.Replace('\\', '_').Replace('/', '_');
            return String.Format("dump_{0}_{1}.sql", server, this.id);
        }

        #endregion
    }
}
