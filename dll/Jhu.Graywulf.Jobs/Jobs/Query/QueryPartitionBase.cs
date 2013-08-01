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
                    throw new Exception("Cannot determine temp database"); // TODO ***
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

        public string GetTemporaryTableName(string tableName)
        {
            if (Context != null)
            {
                return String.Format("{0}_{1}_{2}_{3}", Context.UserName, Context.JobID, id.ToString(), tableName);
            }
            else
            {
                return String.Format("skyquerytemp_{0}_{1}", id.ToString(), tableName);
            }
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
            // *** TODO: delete, not needed
            //RemoteTableReferences.Clear();

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
            source.Query = cg.GenerateMostRestrictiveTableQuery(table, 0);

            return source;
        }

        public void CopyRemoteTable(TableReference table, SourceQueryParameters source)
        {
            // Temp table name

            // *** TODO: delete next line if UniqueName works
            //string temptable = table.GetFullyResolvedName().Replace('.', '_').Replace("[", "").Replace("]", "");
            string temptable = table.UniqueName;
            temptable = GetTemporaryTableName(temptable);

            TemporaryTables.TryAdd(table.UniqueName, temptable);

            // Drop temp table if exists
            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                GetTemporaryDatabaseConnectionString().InitialCatalog,
                query.TemporarySchemaName,
                temptable);

            //
            var connectionString = GetTemporaryDatabaseConnectionString().ConnectionString;
            var bcp = CreateQueryImporter(connectionString, query.TemporarySchemaName, temptable);
            bcp.Source = source;

            bcp.CreateDestinationTable();

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, bcp);

            bcp.Execute();

            UnregisterCancelable(guid);
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

            /*
            var sql = Jhu.Graywulf.SqlParser.SqlCodeGen.SqlServerCodeGenerator.GetCode(SelectStatement, true);
            return sql;*/
        }

        public virtual void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler);
        }

        public abstract void ExecuteQuery();

        /// <summary>
        /// Creates or truncates destination table in the output database (usually MYDB)
        /// </summary>
        /// <remarks>
        /// This has to be in the QueryPartition class because the Query class does not
        /// have information about the database server the partition is executing on and
        /// the temporary tables are required to generate the destination table schema.
        /// </remarks>
        public void InitializeDestinationTable(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler);

            lock (query.syncRoot)
            {
                if (!query.IsDestinationTableInitialized && (query.ResultsetTarget & ResultsetTarget.DestinationTable) != 0)
                {

                    switch (query.ExecutionMode)
                    {
                        case ExecutionMode.SingleServer:
                            {
                                string sql = GetDestinationTableSchemaSourceQuery();

                                switch (query.Destination.Operation)
                                {
                                    case DestinationTableOperation.Drop | DestinationTableOperation.Create:
                                    case DestinationTableOperation.Create:
                                        {
                                            SqlServerDataset ddd = (SqlServerDataset)query.Destination.Table.Dataset;
                                            SqlServerDataset tdd = (SqlServerDataset)query.TemporaryDataset;

                                            CreateTableForBulkCopy(
                                                tdd,
                                                sql,
                                                ddd.ConnectionString,
                                                query.Destination.Table.SchemaName,
                                                query.Destination.Table.TableName);
                                        }
                                        break;
                                    case DestinationTableOperation.Clear:
                                        try
                                        {
                                            TruncateTable(
                                                query.GetDestinationDatabaseConnectionString().ConnectionString,
                                                query.Destination.Table.SchemaName,
                                                query.Destination.Table.TableName);
                                        }
                                        catch (Exception)
                                        {
                                            goto case DestinationTableOperation.Create;
                                        }
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            break;
                        case ExecutionMode.Graywulf:
                            {
                                var sourceds = GetDestinationTableSchemaSourceDataset();
                                string sql = GetDestinationTableSchemaSourceQuery();

                                switch (query.Destination.Operation)
                                {
                                    case DestinationTableOperation.Drop | DestinationTableOperation.Create:
                                    case DestinationTableOperation.Create:
                                        CreateTableForBulkCopy(
                                            sourceds,
                                            sql,
                                            query.GetDestinationDatabaseConnectionString().ConnectionString,
                                            query.Destination.Table.SchemaName,
                                            query.Destination.Table.TableName);
                                        break;
                                    case DestinationTableOperation.Clear:
                                        try
                                        {
                                            TruncateTable(
                                                query.GetDestinationDatabaseConnectionString().ConnectionString,
                                                query.Destination.Table.SchemaName,
                                                query.Destination.Table.TableName);
                                        }
                                        catch (Exception)
                                        {
                                            goto case DestinationTableOperation.Create;
                                        }
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    query.IsDestinationTableInitialized = true;
                }
            }
        }

        public virtual void PrepareCopyResultset(Context context)
        {
            query.InitializeQueryObject(context);
            InitializeQueryObject(context);
        }

        public abstract void CopyResultset();

        public void DropTemporaryTables()
        {
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        SqlServerDataset tdd = (SqlServerDataset)query.TemporaryDataset;

                        foreach (string table in TemporaryTables.Values)
                        {
                            DropTable(tdd.ConnectionString,
                                tdd.DatabaseName,
                                query.TemporarySchemaName,
                                table);
                        }

                        TemporaryTables.Clear();

                        // If output was to a temporary table, and clean-up required
                        if ((query.ResultsetTarget & ResultsetTarget.TemporaryTable) != 0 && !query.KeepTemporaryDestinationTable)
                        {
                            DropTable(tdd.ConnectionString,
                                tdd.DatabaseName,
                                query.TemporarySchemaName,
                                GetTemporaryTableName(query.TemporaryDestinationTableName));
                        }
                    }
                    break;
                case ExecutionMode.Graywulf:
                    {

                        foreach (string table in TemporaryTables.Values)
                        {
                            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                                TemporaryDatabaseInstanceReference.Value.DatabaseName,
                                query.TemporarySchemaName,
                                table);

                            // Log event
                            Jhu.Graywulf.Logging.Event e = new Jhu.Graywulf.Logging.Event("DropTemporaryTables", Guid.Empty);
                            e.UserData.Add("TableName", table);
                            e.EventSource = Jhu.Graywulf.Logging.EventSource.UserCode;
                            Context.LogEvent(e);
                        }

                        TemporaryTables.Clear();

                        // If output was to a temporary table, and clean-up required
                        if ((query.ResultsetTarget & ResultsetTarget.TemporaryTable) != 0 && !query.KeepTemporaryDestinationTable)
                        {
                            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                                TemporaryDatabaseInstanceReference.Value.DatabaseName,
                                query.TemporarySchemaName,
                                GetTemporaryTableName(query.TemporaryDestinationTableName));
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void DropTemporaryViews()
        {
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        SqlServerDataset tdd = (SqlServerDataset)query.TemporaryDataset;

                        foreach (string view in TemporaryViews.Keys)
                        {
                            DropView(tdd.ConnectionString,
                                tdd.DatabaseName,
                                query.TemporarySchemaName,
                                TemporaryViews[view]);
                        }

                        TemporaryViews.Clear();
                    }
                    break;
                case ExecutionMode.Graywulf:
                    {
                        foreach (string view in TemporaryViews.Keys)
                        {
                            DropView(TemporaryDatabaseInstanceReference.Value.GetConnectionString().ConnectionString,
                                TemporaryDatabaseInstanceReference.Value.DatabaseName,
                                query.TemporarySchemaName,
                                TemporaryViews[view]);

                            // Log event
                            Jhu.Graywulf.Logging.Event e = new Jhu.Graywulf.Logging.Event("DropTemporaryViews", Guid.Empty);
                            e.UserData.Add("ViewName", TemporaryViews[view]);
                            e.EventSource = Jhu.Graywulf.Logging.EventSource.UserCode;
                            Context.LogEvent(e);
                        }

                        TemporaryViews.Clear();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
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

        protected void DropTemporaryTable(string temptable)
        {
            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                      GetTemporaryDatabaseConnectionString().InitialCatalog,
                      query.TemporarySchemaName,
                      temptable);
        }

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
