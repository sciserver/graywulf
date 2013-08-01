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
using Jhu.Graywulf.Schema;
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
        private EntityProperty<DatabaseInstance> temporaryDatabaseInstanceReference;

        private ConcurrentDictionary<string, string> temporaryTables;
        private ConcurrentDictionary<string, string> temporaryViews;

        [NonSerialized]
        private Dictionary<string, TableReference> tableSourceReferences;

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

        protected EntityProperty<DatabaseInstance> TemporaryDatabaseInstanceReference
        {
            get { return temporaryDatabaseInstanceReference; }
        }

        public ConcurrentDictionary<string, string> TemporaryTables
        {
            get { return temporaryTables; }
        }

        public ConcurrentDictionary<string, string> TemporaryViews
        {
            get { return temporaryViews; }
        }

        public Dictionary<string, TableReference> TableSourceReferences
        {
            get { return tableSourceReferences; }
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

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>();

            this.partitioningKeyFrom = double.NaN;
            this.partitioningKeyTo = double.NaN;

            this.temporaryTables = new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            this.temporaryViews = new ConcurrentDictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        }

        private void CopyMembers(QueryPartitionBase old)
        {
            this.id = old.id;

            this.query = old.query;

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>(old.temporaryDatabaseInstanceReference);

            this.partitioningKeyFrom = old.partitioningKeyFrom;
            this.partitioningKeyTo = old.partitioningKeyTo;

            this.temporaryTables = old.temporaryTables;
            this.temporaryViews = old.temporaryViews;
        }

        #endregion

        protected void LoadTemporaryDatabaseInstance(bool forceReinitialize)
        {
            if (!AssignedServerInstanceReference.IsEmpty && (temporaryDatabaseInstanceReference == null || forceReinitialize))
            {
                var ef = new EntityFactory(Context);
                var dd = ef.LoadEntity<DatabaseDefinition>(((GraywulfDataset)query.TemporaryDataset).DatabaseDefinitionName);

                dd.LoadDatabaseInstances(false);

                // Find database instance that is on the same machine
                temporaryDatabaseInstanceReference.Value = dd.DatabaseInstances.Values.FirstOrDefault(ddi => ddi.ServerInstance.Guid == AssignedServerInstanceReference.Guid);
                temporaryDatabaseInstanceReference.Value.GetConnectionString();
            }
            else if (AssignedServerInstanceReference.IsEmpty)
            {
                temporaryDatabaseInstanceReference.Value = null;
            }
        }

        public SqlConnectionStringBuilder GetTemporaryDatabaseConnectionString()
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return new SqlConnectionStringBuilder(((SqlServerDataset)query.TemporaryDataset).ConnectionString);
                case ExecutionMode.Graywulf:
                    return temporaryDatabaseInstanceReference.Value.GetConnectionString();
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
                    return temporaryDatabaseInstanceReference.Value.GetDataset();
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();

            if (this.temporaryDatabaseInstanceReference != null) this.temporaryDatabaseInstanceReference.Context = Context;
        }

        public string GetTemporaryTableName(string tableName)
        {
            return String.Format("{0}_{1}_{2}_{3}", Context.UserName, Context.JobID, id.ToString(), tableName);
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
            foreach (TableReference tr in SelectStatement.EnumerateTableSourceReferences(true))
            {
                tableSourceReferences.Add(tr.FullyQualifiedName, tr);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public override void InitializeQueryObject(Context context, bool forceReinitialize)
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

            base.InitializeQueryObject(context, forceReinitialize);
        }

        /// <summary>
        /// Finds those tables that are required to copy from a remote source
        /// </summary>
        /// <returns></returns>
        public List<TableReference> FindRemoteTableReferences()
        {
            if (ExecutionMode == ExecutionMode.Graywulf /*&& query.CacheRemoteTables*/)
            {
                SchemaManager sc = CreateSchemaManager(false);

                Dictionary<string, TableReference> trs = new Dictionary<string, TableReference>(StringComparer.CurrentCultureIgnoreCase);
                foreach (var tr in this.SelectStatement.EnumerateTableSourceReferences(true))
                {
                    if (!tr.IsSubquery && !tr.IsUdf && !tr.IsComputed && !trs.ContainsKey(tr.FullyQualifiedName) &&
                        IsRemoteDataset(sc.Datasets[tr.DatasetName]))
                    {
                        trs.Add(tr.FullyQualifiedName, tr);
                    }
                }

                return new List<TableReference>(trs.Values);
            }
            else
            {
                // Return empty list if no copying needed
                return new List<TableReference>();
            }
        }

        /// <summary>
        /// Checks whther the given dataset is remote to the assigned server
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected bool IsRemoteDataset(DatasetBase ds)
        {
            if (ds is GraywulfDataset && !((GraywulfDataset)ds).IsSpecificInstanceRequired)
            {
                return false;
            }
            else if (ds is SqlServerDataset)
            {
                // A SqlServer dataset is remote if it's on another server
                var csbr = new SqlConnectionStringBuilder(ds.ConnectionString);
                var csba = AssignedServerInstanceReference.Value.GetConnectionString();

                return StringComparer.InvariantCultureIgnoreCase.Compare(csbr.DataSource, csba.DataSource) != 0;
            }
            else
            {
                // Everything else is remote
                return true;
            }
        }

        public SourceQueryParameters PrepareCopyRemoteTable(TableReference table)
        {
            // Load schema
            var sm = this.CreateSchemaManager(false);
            var ds = sm.Datasets[table.DatasetName];

            var res = new SourceQueryParameters();

            res.Dataset = ds;
            // TODO: delete
            //res.ProviderInvariantName = ds.ProviderName;
            //res.ConnectionString = ds.ConnectionString;

            // Find the query specification this table belongs to
            var qs = table.TableSource.FindAscendant<QuerySpecification>();

            // Run the normalizer
            var cnr = new SearchConditionNormalizer();
            cnr.Execute(qs);

            var cg = SqlCodeGeneratorFactory.CreateCodeGenerator(ds);
            res.Query = cg.GenerateMostRestrictiveTableQuery(qs, table, 0);

            return res;
        }

        public void CopyRemoteTable(TableReference table, SourceQueryParameters source)
        {
            // Temp table name
            string temptable = table.FullyResolvedName.Replace('.', '_').Replace("[", "").Replace("]", "");
            temptable = GetTemporaryTableName(temptable);

            while (!temporaryTables.TryAdd(table.FullyQualifiedName, temptable))
            {
            }

            // Drop temp table if exists
            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                GetTemporaryDatabaseConnectionString().InitialCatalog,
                query.TemporarySchemaName,
                temptable);

            //
            var bcp = CreateQueryImporter(GetTemporaryDatabaseConnectionString().ConnectionString, query.TemporarySchemaName, temptable);
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
                ConnectionString = AssignedServerInstanceReference.Value.GetConnectionString().ConnectionString
            };
        }

        public virtual string GetDestinationTableSchemaSourceQuery()
        {
            SubstituteDatabaseNames(query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames();

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

                foreach (var ts in qs.EnumerateDescendantsRecursive<TableSource>())
                {
                    var pc = ts.FindDescendant<TablePartitionClause>();

                    if (pc != null)
                    {
                        pc.Parent.Stack.Remove(pc);
                    }
                }
            }

            return Jhu.Graywulf.SqlParser.SqlCodeGen.SqlServerCodeGenerator.GetCode(SelectStatement, true);
        }

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        protected virtual void SubstituteDatabaseNames(string databaseVersion)
        {
            SchemaManager sc = CreateSchemaManager(false);

            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // *** Nothing to do here?
                    break;
                case ExecutionMode.Graywulf:
                    {
                        var ef = new Jhu.Graywulf.Registry.EntityFactory(Context);

                        foreach (TableReference tr in SelectStatement.EnumerateTableSourceReferences(true))
                        {
                            // ***
                            // check if works without the commented condition and remove if does
                            if (!tr.IsSubquery && !tr.IsComputed /*&& sc.Datasets.ContainsKey(tr.DatasetName)*/)
                            {
                                DatasetBase ds = sc.Datasets[tr.DatasetName];

                                // Graywulf datasets have changing database names depending on the server
                                // the database is on.
                                if (ds is GraywulfDataset)
                                {
                                    var gds = ds as GraywulfDataset;

                                    DatabaseInstance di;
                                    if (gds.IsSpecificInstanceRequired)
                                    {
                                        di = ef.LoadEntity<DatabaseInstance>(gds.DatabaseInstanceName);
                                    }
                                    else
                                    {
                                        // Load database definition
                                        var dd = ef.LoadEntity<DatabaseDefinition>(gds.DatabaseDefinitionName);

                                        // Find appropriate database instance
                                        dd.LoadDatabaseInstances(false);
                                        di = dd.DatabaseInstances.Values.Where(ddi => ddi.DatabaseVersion.Name == databaseVersion && ddi.ServerInstance.Guid == AssignedServerInstanceReference.Guid).FirstOrDefault();
                                    }

                                    // Replace name in query
                                    ((GraywulfDataset)tr.TableOrViewDescription.Dataset).ConnectionString = di.GetConnectionString().ConnectionString;
                                }
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// /// <remarks>This function call must be synchronized!</remarks>
        protected virtual void SubstituteRemoteTableNames()
        {
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // No remote table support

                    // Replace remote table references with temp table references
                    foreach (TableReference tr in SelectStatement.EnumerateTableSourceReferences(true))
                    {
                        if (!tr.IsSubquery && temporaryTables.ContainsKey(tr.FullyQualifiedName))
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                case ExecutionMode.Graywulf:
                    //if (query.CacheRemoteTables)  // TODO: delete, always cache
                    //{
                    // Replace remote table references with temp table references
                    foreach (TableReference tr in SelectStatement.EnumerateTableSourceReferences(true))
                    {
                        if (!tr.IsSubquery && temporaryTables.ContainsKey(tr.FullyQualifiedName))
                        {
                            // Replace name in the query
                            tr.TableOrViewDescription.SchemaName = query.TemporarySchemaName;
                            tr.TableOrViewDescription.ObjectName = temporaryTables[tr.FullyQualifiedName];

                            GraywulfDataset dd = new GraywulfDataset(tr.TableOrViewDescription.Dataset);

                            dd.ConnectionString = temporaryDatabaseInstanceReference.Value.GetConnectionString().ConnectionString;
                            dd.IsCacheable = false;
                            dd.Name = "TEMP";   // TODO: ?
                            tr.TableOrViewDescription.Dataset = dd;
                        }
                    }
                    //}
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public virtual void PrepareExecuteQuery(Context context)
        {
            InitializeQueryObject(context);
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
        public void InitializeDestinationTable(Context context)
        {
            InitializeQueryObject(context);

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

                        foreach (string table in temporaryTables.Keys)
                        {
                            DropTable(tdd.ConnectionString,
                                tdd.DatabaseName,
                                query.TemporarySchemaName,
                                temporaryTables[table]);
                        }

                        temporaryTables.Clear();

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

                        foreach (string table in temporaryTables.Keys)
                        {
                            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                                temporaryDatabaseInstanceReference.Value.DatabaseName,
                                query.TemporarySchemaName,
                                temporaryTables[table]);

                            // Log event
                            Jhu.Graywulf.Logging.Event e = new Jhu.Graywulf.Logging.Event("DropTemporaryTables", Guid.Empty);
                            e.UserData.Add("TableName", temporaryTables[table]);
                            e.EventSource = Jhu.Graywulf.Logging.EventSource.UserCode;
                            Context.LogEvent(e);
                        }

                        temporaryTables.Clear();

                        // If output was to a temporary table, and clean-up required
                        if ((query.ResultsetTarget & ResultsetTarget.TemporaryTable) != 0 && !query.KeepTemporaryDestinationTable)
                        {
                            DropTable(GetTemporaryDatabaseConnectionString().ConnectionString,
                                temporaryDatabaseInstanceReference.Value.DatabaseName,
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

                        foreach (string view in temporaryViews.Keys)
                        {
                            DropView(tdd.ConnectionString,
                                tdd.DatabaseName,
                                query.TemporarySchemaName,
                                temporaryViews[view]);
                        }

                        temporaryViews.Clear();
                    }
                    break;
                case ExecutionMode.Graywulf:
                    {
                        foreach (string view in temporaryViews.Keys)
                        {
                            DropView(temporaryDatabaseInstanceReference.Value.GetConnectionString().ConnectionString,
                                temporaryDatabaseInstanceReference.Value.DatabaseName,
                                query.TemporarySchemaName,
                                temporaryViews[view]);

                            // Log event
                            Jhu.Graywulf.Logging.Event e = new Jhu.Graywulf.Logging.Event("DropTemporaryViews", Guid.Empty);
                            e.UserData.Add("ViewName", temporaryViews[view]);
                            e.EventSource = Jhu.Graywulf.Logging.EventSource.UserCode;
                            Context.LogEvent(e);
                        }

                        temporaryViews.Clear();
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
                    cs = temporaryDatabaseInstanceReference.Value.GetConnectionString();
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
