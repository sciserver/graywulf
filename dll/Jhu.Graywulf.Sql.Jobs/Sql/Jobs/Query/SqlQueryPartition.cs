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
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.LogicalExpressions;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
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
        private Dictionary<string, RemoteSourceTable> remoteSourceTables;

        [NonSerialized]
        private Dictionary<string, RemoteOutputTable> remoteOutputTables;

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

        public Dictionary<string, RemoteSourceTable> RemoteSourceTables
        {
            get { return remoteSourceTables; }
        }

        public Dictionary<string, RemoteOutputTable> RemoteOutputTables
        {
            get { return remoteOutputTables; }
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

            this.remoteSourceTables = new Dictionary<string, RemoteSourceTable>();
            this.remoteOutputTables = new Dictionary<string, RemoteOutputTable>();
        }

        private void CopyMembers(SqlQueryPartition old)
        {
            this.id = old.id;

            this.query = old.query;

            this.partitioningKeyMin = old.partitioningKeyMin;
            this.partitioningKeyMax = old.partitioningKeyMax;

            this.remoteSourceTables = new Dictionary<string, RemoteSourceTable>(old.remoteSourceTables);
            this.remoteOutputTables = new Dictionary<string, RemoteOutputTable>(old.remoteOutputTables);
        }

        public override object Clone()
        {
            return new SqlQueryPartition(this);
        }

        #endregion

        protected virtual SqlQueryCodeGenerator CreateCodeGenerator()
        {
            return new SqlQueryCodeGenerator(this)
            {
                TableNameRendering = NameRendering.FullyQualified,
                TableAliasRendering = AliasRendering.Default,
                ColumnNameRendering = NameRendering.FullyQualified,
                ColumnAliasRendering = AliasRendering.Always,
                FunctionNameRendering = NameRendering.FullyQualified
            };
        }

        protected override void OnNamesResolved(bool forceReinitialize)
        {
        }

        #region Remote table caching functions

        public void IdentifyRemoteOutputTables()
        {
            var cg = CreateCodeGenerator();

            RemoteOutputTables.Clear();

            // TODO: now it's SELECT INTO
            // extend this logic to support CREATE TABLE, INSERT, UPDATE etc.

            if (Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                // Collect remote tables
                foreach (var key in QueryDetails.OutputTableReferences.Keys)
                {
                    RemoteOutputTable rot = null;

                    foreach (var tr in QueryDetails.OutputTableReferences[key])
                    {
                        // TODO: Add support for insert and create table

                        if (tr.TableContext.HasFlag(TableContext.Into) &&
                            IsRemoteDataset(tr.DatabaseObject.Dataset))
                        {
                            if (rot == null)
                            {
                                // Add entry for temp table
                                var table = (TableOrView)tr.DatabaseObject;
                                var tablekey = table.UniqueKey;
                                var tempname = cg.GenerateEscapedUniqueName(table, null);
                                var temptable = GetTemporaryTable(tempname);
                                var temptablekey = temptable.UniqueKey;

                                rot = new RemoteOutputTable()
                                {
                                    Table = table,
                                    UniqueKey = tablekey,
                                    TableReferences = new List<TableReference>(),
                                    TempTable = temptable
                                };

                                TemporaryTables.Add(table, temptable);
                                RemoteOutputTables.Add(tablekey, rot);

                                if (this.id == 0)
                                {
                                    LogOperation(LogMessages.RemoteOutputTableIdentified, table.FullyResolvedName);
                                }
                            }

                            rot.TableReferences.Add(tr);
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
        /// Finds those tables that are required to execute the query but had to be
        /// copied from a remote source
        /// </summary>
        /// <returns></returns>
        public void IdentifyRemoteSourceTables()
        {
            var cg = CreateCodeGenerator();
            RemoteSourceTables.Clear();

            if (Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                // Collect remote tables
                foreach (var key in QueryDetails.SourceTableReferences.Keys)
                {
                    RemoteSourceTable rst = null;

                    foreach (var tr in QueryDetails.SourceTableReferences[key])
                    {
                        if (tr.TableContext.HasFlag(TableContext.TableOrView) && 
                            tr.IsCachable && IsRemoteDataset(tr.DatabaseObject.Dataset))
                        {
                            if (rst == null)
                            {
                                var table = (TableOrView)tr.DatabaseObject;

                                // Add entry for temp table
                                var tempname = cg.GenerateEscapedUniqueName(table, null);
                                var temptable = GetTemporaryTable(tempname);

                                rst = new RemoteSourceTable()
                                {
                                    Table = table,
                                    UniqueKey = tr.DatabaseObject.UniqueKey,
                                    TableReferences = new List<TableReference>(),
                                    TempTable = temptable
                                };

                                TemporaryTables.Add(table, temptable);
                                RemoteSourceTables.Add(rst.UniqueKey, rst);

                                if (this.id == 0)
                                {
                                    LogOperation(LogMessages.RemoteSourceTableIdentified, table.FullyResolvedName);
                                }
                            }

                            rst.TableReferences.Add(tr);
                        }
                    }
                }

                // Generate most restrictive queries
                var queries = GenerateRemoteSourceTableQueries(ColumnContext.AllReferenced, 0);

                foreach (var key in queries.Keys)
                {
                    if (remoteSourceTables.ContainsKey(key))
                    {
                        remoteSourceTables[key].RemoteQuery = queries[key];
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
        public void PrepareCopySourceTable(string tableKey, out SourceQuery query)
        {
            // TODO: add option to do by table or tr

            var table = remoteSourceTables[tableKey];
            var sm = GetSchemaManager();
            var ds = sm.Datasets[table.Table.DatasetName];

            // Graywulf dataset has to be converted to prevent registry access
            if (ds is GraywulfDataset)
            {
                ds = new SqlServerDataset(ds);
            }

            query = new SourceQuery()
            {
                Dataset = ds,
                Query = remoteSourceTables[tableKey].RemoteQuery
            };
        }

        /// <summary>
        /// Copies a table from a remote data source by creating and
        /// executing a table copy task.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="source"></param>
        public async Task CopySourceTableAsync(string tableKey, SourceQuery source)
        {
            // TODO: add option to do by table or tr

            var table = remoteSourceTables[tableKey].Table;
            var temptable = TemporaryTables.GetValue(table);

            var dest = new DestinationTable(
                temptable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create | TableInitializationOptions.CreatePrimaryKey);

            using (var tc = CreateTableCopyTask(source, dest, false, out var settings))
            {
                var res = await tc.Value.ExecuteAsyncEx(source, dest, settings);

                for (int i = 0; i < res.Count; i++)
                {
                    LogOperation(LogMessages.RemoteSourceTableCopied, res[i].SourceTable, id, res[i].Status, res[i].Elapsed.TotalSeconds, res[i].RecordsAffected);
                }
            }
        }

        public Dictionary<string, string> GenerateRemoteSourceTableQueries(ColumnContext columnContext, int top)
        {
            var res = new Dictionary<string, string>();

            // Normalize all search conditions
            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(QueryDetails.ParsingTree);

            // Loop through all tables
            foreach (var key in QueryDetails.SourceTableReferences.Keys)
            {
                var columns = new Dictionary<string, ColumnReference>();
                DatabaseObject table = null;
                TableReference ntr = null;
                CodeGeneratorBase cg = null;

                // Loop through all references to the table
                foreach (var tr in QueryDetails.SourceTableReferences[key])
                {
                    if (ntr == null)
                    {
                        table = tr.DatabaseObject;

                        ntr = new TableReference(tr);
                        ntr.Alias = null;

                        cg = CodeGeneratorFactory.CreateCodeGenerator(table.Dataset);
                        cg.TableNameRendering = NameRendering.FullyQualified;
                        cg.TableAliasRendering = AliasRendering.Never;
                        cg.ColumnNameRendering = NameRendering.IdentifierOnly;
                        cg.ColumnAliasRendering = AliasRendering.Never;
                        cg.FunctionNameRendering = NameRendering.FullyQualified;
                    }

                    // Remap all table reference to the first one; this is to prevent different aliases etc.
                    // Certain tables can be referenced multiple times without different alias,
                    // for example UNION etc. queries.

                    if (!cg.TableReferenceMap.ContainsKey(tr))
                    {
                        cg.TableReferenceMap.Add(tr, ntr);
                    }

                    // Collect columns that will be returned
                    foreach (var c in tr.FilterColumnReferences(columnContext))
                    {
                        if (!columns.ContainsKey(c.ColumnName))
                        {
                            columns.Add(c.ColumnName, new ColumnReference(c));
                        }
                    }
                }

                // Generate select list
                var columnlist = cg.CreateColumnListGenerator();
                columnlist.ListType = ColumnListType.SelectWithOriginalNameNoAlias;
                columnlist.TableAlias = String.Empty;
                columnlist.Columns.AddRange(columns.Values);

                // Generate where clause
                var where = scn.GenerateWherePredicatesSpecificToTable(QueryDetails.SourceTableReferences[key]);

                var select = cg.GenerateMostRestrictiveTableQuery(
                    cg.GetResolvedTableName(table),
                    null,
                    columnlist.Execute(),
                    cg.Execute(where),
                    top);

                res.Add(key, select);
            }

            return res;
        }

        #endregion
        #region Final query execution

        public virtual void PrepareExecuteQuery(out SourceQuery sourceQuery, out DestinationTable destinationTable)
        {
            // Source query will be run on the code database to have access to UDTs
            // At this point, SELECT INTOs and similar table creation statements with
            // explicit table names should be directed to the temp database
            // Outputs from simple selects will be dealt with later in ExecuteQueryAsync

            var cg = CreateCodeGenerator();

            sourceQuery = cg.GetExecuteQuery();
            sourceQuery.Dataset = CodeDataset;

            // Destination tables go to the local temp database first and will
            // be gathered lates

            // TODO: with single partition queries, we could write the tables
            // directly to MyDB

            // TODO: Now we create the PK automatically, still in tempdb,
            // which is not optimal since order by is not supported in the
            // follow up bulk-insert step. Figure out how to convey info on PK
            // from here to the final table copy step in meta-data or else.

            var temp = GetTemporaryTable(Constants.DefaultOutputTableNamePattern);
            destinationTable = new DestinationTable()
            {
                Dataset = TemporaryDataset,
                SchemaName = TemporaryDataset.DefaultSchemaName,
                TableNamePattern = temp.TableName,
                Options = TableInitializationOptions.Create |
                          TableInitializationOptions.CreatePrimaryKey,
            };
        }

        public async Task ExecuteQueryAsync(SourceQuery source, DestinationTable destination)
        {
            var hostname = AssignedServerInstance.Machine.HostName.ResolvedValue;

            using (var task = RemoteService.RemoteServiceHelper.CreateObject<ICopyTable>(CancellationContext, hostname, false))
            {
                var settings = new TableCopySettings()
                {
                    BatchName = Parameters.BatchName,
                    BypassExceptions = false,
                    Timeout = Parameters.QueryTimeout,
                };

                var results = await task.Value.ExecuteAsyncEx(source, destination, settings);

                // The results collection now contains output tables that are not
                // explicitly named in the query (SELECT INTO etc), but are generated as
                // output table from simple SELECTs. These won't automatically show up in the
                // temporary tables collection so add them now.

                foreach (var result in results)
                {
                    if (result.Status == TableCopyStatus.Success &&
                        !TemporaryTables.ContainsValue(result.DestinationTable))
                    {
                        var temp = TemporaryDataset.Tables[result.DestinationTable];
                        TemporaryTables.Add(temp, temp);

                        var rot = new RemoteOutputTable()
                        {
                            TempTable = temp
                        };

                        remoteOutputTables.Add(temp.UniqueKey, rot);
                    }

                    LogOperation(LogMessages.OutputTableCreated, result.DestinationTable, id, result.Status, result.Elapsed.TotalSeconds, result.RecordsAffected);
                }
            }
        }

        #endregion
        #region Destination table and copy resultset functions

        public Task InitializeOutputTableAsync(string remoteTable)
        {
            var rt = RemoteOutputTables[remoteTable];

            // Check if temp table is created at all. Scripts with conditions might not
            // create all output tables

            if (rt.TempTable.IsExisting)
            {
                // Create output table based on temp table colums
                var columns = new List<Column>();

                foreach (var name in rt.TempTable.Columns.Keys)
                {
                    columns.Add(new Column(rt.TempTable.Columns[name]));
                }

                // TODO: it works now with SELECT INTO and unnamed resultsets but options will need
                // to be changes for INSERT, UPDATE etc.

                // If it's an unnamed resultset then the remote table doesn't exist yet
                if (rt.Table == null)
                {
                    rt.Table = new Table(Query.Parameters.DefaultOutputDataset)
                    {
                        SchemaName = Query.Parameters.DefaultOutputDataset.DefaultSchemaName,
                        TableName = rt.TempTable.TableName,
                    };
                }

                if (!rt.Table.IsExisting)
                {
                    // TODO: make table initialization async
                    ((Table)rt.Table).Initialize(columns, TableInitializationOptions.Create | TableInitializationOptions.CreatePrimaryKey);
                }

                // TODO: how to lock in case of multiple partitions and multiple
                // output tables?

                lock (Parameters)
                {
                    Parameters.OutputTables.Add((Table)rt.Table);
                }

                LogOperation(LogMessages.RemoteOutputTableInitialized, rt.Table.FullyResolvedName);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Copies resultset from the output temporary table to the destination database (MYDB)
        /// </summary>
        public async Task CopyOutputTableAsync(string remoteTable)
        {
            switch (Query.Parameters.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Do nothing as execute writes results directly into destination table
                    break;
                case ExecutionMode.Graywulf:
                    {
                        var rt = RemoteOutputTables[remoteTable];
                        var tempTable = rt.TempTable;

                        // Check if temp table is created at all. Scripts with conditions might not
                        // create all output tables

                        if (tempTable.IsExisting)
                        {
                            var outputTable = (Table)rt.Table;
                            var columns = new List<Column>();

                            var source = SourceTable.Create(rt.TempTable);
                            var destination = DestinationTable.Create((Table)rt.Table, TableInitializationOptions.Append);

                            // Create bulk copy task and execute it
                            using (var tc = CreateTableCopyTask(source, destination, false, out var settings))
                            {
                                var res = await tc.Value.ExecuteAsyncEx(source, destination, settings);

                                for (int i = 0; i < res.Count; i++)
                                {
                                    LogOperation(LogMessages.RemoteOutputTableCopied, res[i].DestinationTable, id, res[i].Status, res[i].Elapsed.TotalSeconds, res[i].RecordsAffected);
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<Table> PrepareCreateDestinationTablePrimaryKeyAsync()
        {
            // TODO:
            // rename to outputtablepk
            // make sure PKs are recorded somewhere when the output tables
            // are written to tempdb so they can be created at this point

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

            return destination;*/

            return null;
        }

        public Task CreateOutputTablePrimaryKeyAsync(Table destination)
        {
            /*
            if (destination != null && destination.PrimaryKey != null)
            {
                var cg = CreateCodeGenerator();
                var sql = cg.GenerateCreatePrimaryKeyScript(destination, true);

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
            */

            return Task.CompletedTask;
        }

        #endregion
        #region Temporary table logic

        public override Table GetTemporaryTable(string tableName)
        {
            string tempname;

            switch (Parameters.ExecutionMode)
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
