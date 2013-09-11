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
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class QueryPartitionBase : QueryObject
    {
        #region Property storage variables

        private int id;

        private QueryBase query;

        private double partitioningKeyFrom;
        private double partitioningKeyTo;

        [NonSerialized]
        private Dictionary<string, TableReference> tableSourceReferences;

        [NonSerialized]
        private Dictionary<string, TableReference> remoteTableReferences;

        #endregion

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public QueryBase Query
        {
            get { return query; }
        }

        public double PartitioningKeyFrom
        {
            get { return partitioningKeyFrom; }
            set { partitioningKeyFrom = value; }
        }

        public double PartitioningKeyTo
        {
            get { return partitioningKeyTo; }
            set { partitioningKeyTo = value; }
        }

        public Dictionary<string, TableReference> TableSourceReferences
        {
            get { return tableSourceReferences; }
        }

        public Dictionary<string, TableReference> RemoteTableReferences
        {
            get { return remoteTableReferences; }
        }

        #region Constructors and initializer functions

        public QueryPartitionBase()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryPartitionBase(QueryPartitionBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        public QueryPartitionBase(QueryBase query, Context context)
            : base(query)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
            this.query = query;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.id = 0;

            this.query = null;

            this.partitioningKeyFrom = double.NegativeInfinity;
            this.partitioningKeyTo = double.PositiveInfinity;

            this.tableSourceReferences = new Dictionary<string, TableReference>(SchemaManager.Comparer);
            this.remoteTableReferences = new Dictionary<string, TableReference>(SchemaManager.Comparer);
        }

        private void CopyMembers(QueryPartitionBase old)
        {
            this.id = old.id;

            this.query = old.query;

            this.partitioningKeyFrom = old.partitioningKeyFrom;
            this.partitioningKeyTo = old.partitioningKeyTo;

            this.tableSourceReferences = new Dictionary<string, TableReference>(old.tableSourceReferences, SchemaManager.Comparer);
            this.remoteTableReferences = new Dictionary<string, TableReference>(old.remoteTableReferences, SchemaManager.Comparer);
        }

        #endregion

        protected void LoadTemporaryDatabaseInstance(bool forceReinitialize)
        {
            if (!AssignedServerInstanceReference.IsEmpty && (TemporaryDatabaseInstanceReference.IsEmpty || forceReinitialize))
            {
                var gwds = (GraywulfDataset)query.TemporaryDataset;
                gwds.Context = Context;
                var dd = gwds.DatabaseVersion.Value.DatabaseDefinition;

                dd.LoadDatabaseInstances(false);
                foreach (var di in dd.DatabaseInstances.Values)
                {
                    di.Context = Context;
                }

                // Find database instance that is on the same machine
                try
                {
                    // TODO: only server instance and database definition is checked here, maybe database version would be better
                    TemporaryDatabaseInstanceReference.Value = dd.DatabaseInstances.Values.FirstOrDefault(ddi => ddi.ServerInstanceReference.Guid == AssignedServerInstance.Guid);
                    TemporaryDatabaseInstanceReference.Value.GetConnectionString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot determine temp database", ex); // TODO ***
                }
            }
            else if (AssignedServerInstanceReference.IsEmpty)
            {
                TemporaryDatabaseInstanceReference.Value = null;
            }
        }

        public SqlConnectionStringBuilder GetTemporaryDatabaseConnectionString()
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return new SqlConnectionStringBuilder(((SqlServerDataset)query.TemporaryDataset).ConnectionString);
                case ExecutionMode.Graywulf:
                    return TemporaryDatabaseInstanceReference.Value.GetConnectionString();
                default:
                    throw new NotImplementedException();
            }
        }

        public DatasetBase GetTemporaryDatabaseDataset()
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return query.TemporaryDataset;
                case ExecutionMode.Graywulf:
                    return TemporaryDatabaseInstanceReference.Value.GetDataset();
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();

            if (this.TemporaryDatabaseInstanceReference != null) this.TemporaryDatabaseInstanceReference.Context = Context;
        }

        public Table GetTemporaryTable(string tableName)
        {
            string tempname;
            var tempds = GetTemporaryDatabaseDataset();

            switch (query.ExecutionMode)
            {
                case Jobs.Query.ExecutionMode.SingleServer:
                    tempname = String.Format("skyquerytemp_{0}_{1}", id.ToString(), tableName);
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    tempname = String.Format("{0}_{1}_{2}_{3}", Context.UserName, Context.JobID, id.ToString(), tableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new Table()
            {
                Dataset = tempds,
                DatabaseName = tempds.DatabaseName,
                SchemaName = query.TemporarySchemaName,
                TableName = tempname,
            };
        }

        /// <summary>
        /// Interprets the parsed query
        /// </summary>
        protected override void FinishInterpret(bool forceReinitialize)
        {
            // --- Retrieve target table information
            IntoClause into = SelectStatement.FindDescendantRecursive<IntoClause>();
            if (into != null)
            {
                // remove into clause from query
                into.Parent.Stack.Remove(into);
            }

            // Cache table source references
            tableSourceReferences = new Dictionary<string, TableReference>();
            foreach (TableReference tr in SelectStatement.EnumerateSourceTableReferences(true))
            {
                tableSourceReferences.Add(tr.UniqueName, tr);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public override void InitializeQueryObject(Context context, IScheduler scheduler, bool forceReinitialize)
        {

            if (context != null)
            {
                Context = context;
            }

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    break;
                case ExecutionMode.Graywulf:
                    LoadTemporaryDatabaseInstance(forceReinitialize);
                    break;
                default:
                    throw new NotImplementedException();
            }

            base.InitializeQueryObject(context, scheduler, forceReinitialize);
        }

        /// <summary>
        /// Finds those tables that are required to copy from a remote source
        /// </summary>
        /// <returns></returns>
        public void FindRemoteTableReferences()
        {
            if (ExecutionMode == ExecutionMode.Graywulf /*&& query.CacheRemoteTables*/)
            {
                SchemaManager sc = GetSchemaManager(false);

                foreach (var tr in this.SelectStatement.EnumerateSourceTableReferences(true))
                {
                    if (tr.IsCachable && !RemoteTableReferences.ContainsKey(tr.UniqueName) &&
                        IsRemoteDataset(sc.Datasets[tr.DatasetName]))
                    {
                        RemoteTableReferences.Add(tr.UniqueName, tr);
                    }
                }
            }
            else
            {
                // nothing to do here
            }
        }

        public SourceQueryParameters PrepareCopyRemoteTable(TableReference table)
        {
            // Load schema
            var sm = this.GetSchemaManager(false);
            var ds = sm.Datasets[table.DatasetName];

            // Graywulf dataset is to be converted to prevent registry access
            if (ds is GraywulfDataset)
            {
                ds = new SqlServerDataset(ds);
            }

            var source = new SourceQueryParameters();

            source.Dataset = ds;

            // Find the query specification this table belongs to
            var qs = ((TableSource)table.Node).QuerySpecification;

            // Run the normalizer
            var cnr = new SearchConditionNormalizer();
            cnr.NormalizeQuerySpecification(qs);

            var cg = SqlCodeGeneratorFactory.CreateCodeGenerator(ds);
            source.Query = cg.GenerateMostRestrictiveTableQuery(table, true, 0);

            return source;
        }

        public void CopyRemoteTable(TableReference table, SourceQueryParameters source)
        {
            // Temp table name
            var temptable = GetTemporaryTable(table.UniqueName);
            TemporaryTables.TryAdd(table.UniqueName, temptable);

            var destination = new DestinationTableParameters()
            {
                Table = temptable,
                Operation = DestinationTableOperation.Drop | DestinationTableOperation.Create,
            };

            var bcp = CreateQueryImporter(source, destination, false);
            bcp.Source = source;
            bcp.Destination = destination;

            bcp.CreateDestinationTable();

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, bcp);

            bcp.Execute();

            UnregisterCancelable(guid);
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
                /*return String.Format("[{0}].[{1}].[{2}]",
                    GetTemporaryDatabaseConnectionString().InitialCatalog,
                    Query.TemporarySchemaName,
                    TemporaryTables[table.UniqueName]);*/

                var table = TemporaryTables[tr.UniqueName];

                return String.Format("[{0}].[{1}].[{2}]",
                    !String.IsNullOrEmpty(table.DatabaseName) ? table.DatabaseName : table.Dataset.DatabaseName,
                    table.SchemaName,
                    table.TableName);
            }
            else
            {
                return tr.GetFullyResolvedName();
            }
        }

        public virtual DatasetBase GetDestinationTableSchemaSourceDataset()
        {
            return new SqlServerDataset()
            {
                ConnectionString = AssignedServerInstance.GetConnectionString().ConnectionString
            };
        }

        public virtual string GetDestinationTableSchemaSourceQuery()
        {
            // strip off order by
            OrderByClause orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }

            // strip off partition on
            foreach (QuerySpecification qs in SelectStatement.EnumerateQuerySpecifications())
            {
                // strip off select into
                IntoClause into = qs.FindDescendant<IntoClause>();
                if (into != null)
                {
                    qs.Stack.Remove(into);
                }

                foreach (var ts in qs.EnumerateDescendantsRecursive<SimpleTableSource>())
                {
                    var pc = ts.FindDescendant<TablePartitionClause>();

                    if (pc != null)
                    {
                        pc.Parent.Stack.Remove(pc);
                    }
                }
            }

            var cg = new Jhu.Graywulf.SqlParser.SqlCodeGen.SqlServerCodeGenerator();
            cg.ResolveNames = true;

            var sw = new StringWriter();
            cg.Execute(sw, SelectStatement);
            return sw.ToString();
        }

        public virtual void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler);
        }

        protected abstract string GetOutputSelectQuery();

        public void ExecuteQuery()
        {
            DestinationTableParameters destination;

            SourceQueryParameters source = new SourceQueryParameters()
            {
                Query = GetOutputSelectQuery(),
                Timeout = Query.QueryTimeout,
            };

            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // In single-server mode results are directly written into destination table
                    source.Dataset = Query.Destination.Table.Dataset;
                    destination = Query.Destination;
                    break;
                case ExecutionMode.Graywulf:
                    // In graywulf mode results are written into a temporary table first
                    var temptable = GetTemporaryTable(Query.TemporaryDestinationTableName);
                    TemporaryTables.TryAdd(temptable.TableName, temptable);
                    DropTableOrView(temptable);     // TODO: not needed

                    source.Dataset = AssignedServerInstance.GetDataset();

                    destination = new DestinationTableParameters()
                    {
                        Operation = DestinationTableOperation.Append,       // TODO: change to drop
                        Table = temptable
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            ExecuteSelectInto(source, destination);
        }

        /// <summary>
        /// Creates or truncates destination table in the output database (usually MYDB)
        /// </summary>
        /// <remarks>
        /// This has to be in the QueryPartition class because the Query class does not
        /// have information about the database server the partition is executing on and
        /// the temporary tables are required to generate the destination table schema.
        /// </remarks>
        public void PrepareDestinationTable(Context context, IScheduler scheduler)
        {
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Output is already written to target table
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    {
                        InitializeQueryObject(context, scheduler);

                        lock (query.syncRoot)
                        {
                            if (!query.IsDestinationTableInitialized && (query.ResultsetTarget & ResultsetTarget.DestinationTable) != 0)
                            {
                                var source = new SourceQueryParameters()
                                {
                                    Query = GetDestinationTableSchemaSourceQuery(),
                                    Dataset = GetDestinationTableSchemaSourceDataset(),
                                    Timeout = Query.QueryTimeout
                                };

                                // TODO: this is screwed up here
                                // drop table in every partition... call it only once

                                if ((query.Destination.Operation & DestinationTableOperation.Drop) != 0)
                                {
                                    DropTableOrView(query.Destination.Table);
                                }

                                if ((query.Destination.Operation & DestinationTableOperation.Create) != 0)
                                {
                                    CreateTableForBulkCopy(source, query.Destination, false);
                                }
                                else if ((query.Destination.Operation & DestinationTableOperation.Clear) != 0)
                                {
                                    // TODO: This might need some revision here
                                    // what if schema differs?
                                    TruncateTable(query.Destination.Table);
                                }
                                else if ((query.Destination.Operation & DestinationTableOperation.Append) != 0)
                                {
                                    // TODO: This might need some revision here
                                    // what if schema differs?
                                    throw new NotImplementedException();
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }

                            query.IsDestinationTableInitialized = true;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public virtual void PrepareCopyResultset(Context context)
        {
            query.InitializeQueryObject(context);
            InitializeQueryObject(context);
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
                        var sql = "SELECT tablealias.* FROM [{0}].[{1}].[{2}] AS tablealias";
                        var temptable = GetTemporaryTable(Query.TemporaryDestinationTableName);

                        sql = String.Format(sql, temptable.DatabaseName, temptable.SchemaName, temptable.TableName);

                        var source = new SourceQueryParameters()
                        {
                            Dataset = temptable.Dataset,
                            Query = sql,
                            Timeout = Query.QueryTimeout,
                        };

                        // Change destination to Append, output table has already been created,
                        // partitions only append to it
                        var destination = new DestinationTableParameters(Query.Destination);
                        destination.Operation = DestinationTableOperation.Append;

                        // Change destination operation to

                        if ((Query.ResultsetTarget & (ResultsetTarget.TemporaryTable | ResultsetTarget.DestinationTable)) != 0)
                        {
                            ExecuteBulkCopy(source, destination, false, Query.QueryTimeout);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void DropTemporaryTables()
        {
            foreach (var table in TemporaryTables.Values)
            {
                DropTableOrView(table);
            }

            TemporaryTables.Clear();
        }

        public void DropTemporaryViews()
        {
            foreach (var view in TemporaryViews.Values)
            {
                DropTableOrView(view);
            }

            TemporaryViews.Clear();
        }

        private string GetDumpFileName()
        {
            string server = GetTemporaryDatabaseConnectionString().DataSource;
            return String.Format("dump_{0}_{1}.sql", server, this.id);
        }

        protected void DumpSqlCommand(string sql)
        {
#if DUMPQUERIES
            string filename = GetDumpFileName();

            File.AppendAllText(filename, String.Format("-- {0}\r\n", DateTime.Now));

            File.AppendAllText(filename, sql + "\r\nGO\r\n");
#endif
        }

        protected void DumpSqlCommand(SqlCommand cmd)
        {
#if DUMPQUERIES
            string filename = GetDumpFileName();

            File.AppendAllText(filename, String.Format("-- {0}\r\n", DateTime.Now));

            StringWriter sw = new StringWriter();

            foreach (SqlParameter par in cmd.Parameters)
            {
                sw.WriteLine(String.Format("DECLARE {0} {1} = {2}",
                    par.ParameterName,
                    par.SqlDbType.ToString(),
                    par.Value.ToString()));
            }

            sw.WriteLine(cmd.CommandText);
            sw.WriteLine("GO");

            File.AppendAllText(filename, sw.ToString());
#endif
        }

#if false
        protected void DropTemporaryTable(string temptable)
        {
            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                      GetTemporaryDatabaseConnectionString().InitialCatalog,
                      query.TemporarySchemaName,
                      temptable);
        }
#endif

        protected void ExecuteSqlCommandOnTemporaryDatabase(string sql)
        {
            DumpSqlCommand(sql);

            SqlConnectionStringBuilder cs = GetTemporaryDatabaseConnectionString();

            using (SqlConnection cn = new SqlConnection(cs.ConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = query.QueryTimeout;
                    // Tamas [2009-05-15]: for joining self
                    // TODO: it's fixed, must work, remove try catch
                    try
                    {
                        ExecuteLongCommandNonQuery(cmd);
                    }
                    catch (SqlException ex)
                    {
                        if (!ex.Message.StartsWith(@"There is already an object named 'user_"))
                            throw ex;
                    }
                }
            }
        }

        protected void ExecuteSqlCommandOnTemporaryDatabase(SqlCommand cmd)
        {
            DumpSqlCommand(cmd);

            SqlConnectionStringBuilder cs = GetTemporaryDatabaseConnectionString();

            using (SqlConnection cn = new SqlConnection(cs.ConnectionString))
            {
                cn.Open();

                cmd.Connection = cn;
                cmd.CommandTimeout = query.QueryTimeout;

                ExecuteLongCommandNonQuery(cmd);
            }
        }

        protected object ExecuteSqlCommandOnTemporaryDatabaseScalar(SqlCommand cmd)
        {
            DumpSqlCommand(cmd);

            SqlConnectionStringBuilder cs;
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    cs = GetTemporaryDatabaseConnectionString();
                    break;
                case ExecutionMode.Graywulf:
                    cs = TemporaryDatabaseInstanceReference.Value.GetConnectionString();
                    break;
                default:
                    throw new NotImplementedException();
            }

            using (SqlConnection cn = new SqlConnection(cs.ConnectionString))
            {
                cn.Open();

                cmd.Connection = cn;

                cmd.CommandTimeout = query.QueryTimeout;

                return ExecuteLongCommandScalar(cmd);
            }
        }
    }
}
