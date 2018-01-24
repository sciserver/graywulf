using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Tasks;
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
        private Dictionary<string, List<TableReference>> remoteTables;

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

        public Dictionary<string, List<TableReference>> RemoteTables
        {
            get { return remoteTables; }
        }

        [IgnoreDataMember]
        private SqlQueryCodeGenerator CodeGenerator
        {
            get { return (SqlQueryCodeGenerator)CreateCodeGenerator(); }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryPartition()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryPartition(CancellationContext cancellationContext)
            : base(cancellationContext)
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

            this.remoteTables = new Dictionary<string, List<TableReference>>(SchemaManager.Comparer);
        }

        private void CopyMembers(SqlQueryPartition old)
        {
            this.id = old.id;

            this.query = old.query;

            this.partitioningKeyMin = old.partitioningKeyMin;
            this.partitioningKeyMax = old.partitioningKeyMax;

            this.remoteTables = new Dictionary<string, List<TableReference>>(old.remoteTables);
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

        protected override void OnNamesResolved(bool forceReinitialize)
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
                foreach (var key in QueryDetails.SourceTables.Keys)
                {
                    foreach (var tr in QueryDetails.SourceTables[key])
                    {
                        if (tr.Type == TableReferenceType.TableOrView)
                        {
                            var uniqueKey = tr.DatabaseObject.UniqueKey;

                            if (tr.IsCachable && IsRemoteDataset(tr.DatabaseObject.Dataset))
                            {
                                if (!remoteTables.ContainsKey(uniqueKey))
                                {
                                    remoteTables.Add(key, new List<TableReference>());
                                }

                                remoteTables[key].Add(tr);
                            }
                        }
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
        /// <param name="tableKey"></param>
        /// <returns></returns>
        public void PrepareCopyRemoteTable(string tableKey, out SourceTableQuery query)
        {
            var tr = remoteTables[tableKey][0];
            var sm = GetSchemaManager();
            var ds = sm.Datasets[tr.DatasetName];

            // Graywulf dataset has to be converted to prevent registry access
            if (ds is GraywulfDataset)
            {
                ds = new SqlServerDataset(ds);
            }

            // Generate most restrictive query
            var cg = CodeGeneratorFactory.CreateCodeGenerator(ds);
            var sql = cg.GenerateMostRestrictiveTableQuery(remoteTables[tableKey], ColumnContext.All, 0);
            
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
        public async Task CopyRemoteTableAsync(string tableKey, SourceTableQuery source)
        {
            var tr = remoteTables[tableKey][0];

            // Create a target table name
            var cg = new SqlServerCodeGenerator();
            var temptable = GetTemporaryTable(cg.GenerateEscapedUniqueName(tr));
                        
            TemporaryTables.TryAdd(tr.TableOrView.UniqueKey, temptable);

            var dest = new DestinationTable(
                temptable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create);

            using (var tc = CreateTableCopyTask(source, dest, false))
            {
                await tc.Value.ExecuteAsync();
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
            if (RemoteTables.ContainsKey(tr.TableOrView.UniqueKey))
            {
                return CodeGenerator.GetResolvedTableName(TemporaryTables[tr.TableOrView.UniqueKey]);
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
            if (RemoteTables.ContainsKey(tr.TableOrView.UniqueKey))
            {
                return CodeGenerator.GetResolvedTableNameWithAlias(TemporaryTables[tr.TableOrView.UniqueKey], tr.Alias);
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
        public SourceTableQuery GetExecuteSourceQuery()
        {
            // Source query will be run on the code database to have
            // access to UDTs
            var source = CodeGenerator.GetExecuteQuery(QueryDetails.ParsingTree);
            source.Dataset = CodeDataset;
            return source;
        }

        public async Task ExecuteQueryAsync(SourceTableQuery source, Table destination)
        {
            var dt = new DestinationTable(destination, TableInitializationOptions.Create);

            var insert = new InsertIntoTable(CancellationContext)
            {
                Source = source,
                Destination = dt,
            };

            await insert.ExecuteAsync();
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

        public Task PrepareDestinationTableAsync()
        {
            return Task.CompletedTask;

            /*
            // Only initialize target table if it's still uninitialized
            if (Interlocked.Exchange(ref query.IsDestinationTableCreated, 1) == 0)
            {
                var source = GetOutputSourceQuery();

                // TODO: figure out metadata from query
                var table = query.Destination.GetQueryOutputTable(BatchName, QueryName, null, null);
                var columns = await source.GetColumnsAsync(CancellationContext.Token);

                // TODO: make schema operation async
                table.Initialize(columns, query.Destination.Options);

                // At this point the name of the destination is determined
                // mark it as the output
                query.Output = table;
            }
            */
        }

        public async Task<Table> PrepareCreateDestinationTablePrimaryKeyAsync()
        {
            throw new NotImplementedException();

            /*
            Table destination = null;

            if (Interlocked.Exchange(ref Query.IsDestinationTablePrimaryKeyCreated, 1) == 0)
            {
                destination = query.Destination.GetQueryOutputTable(BatchName, QueryName, null, null);

                var source = GetExecuteSourceQuery();
                var columns = (await source.GetColumnsAsync(CancellationContext.Token)).Where(ci => ci.IsKey).ToArray();

                if (columns.Length > 0)
                {
                    var pk = new Index(destination, columns, null, true);
                    destination.Indexes.TryAdd(pk.IndexName, pk);
                }
            }

            return destination;
            */
        }

        public async Task CreateDestinationTablePrimaryKeyAsync(Table destination)
        {
            if (destination != null && destination.PrimaryKey != null)
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
                        await ExecuteSqlOnDatasetAsync(cmd, destination.Dataset);
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

        /// <summary>
        /// Copies resultset from the output temporary table to the destination database (MYDB)
        /// </summary>
        public async Task CopyResultsetAsync()
        {
            throw new NotImplementedException();

            /*
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
                            await tc.Value.ExecuteAsync();
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            */
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
