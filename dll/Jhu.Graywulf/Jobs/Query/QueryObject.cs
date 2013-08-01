using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.IO;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Namespace="")]
    public abstract class QueryObject : ICancelableTask, ICloneable
    {
        #region Member variables

        [NonSerialized]
        internal object syncRoot;

        #endregion
        #region Property storage member variables

        [NonSerialized]
        private Context context;

        private string queryFactoryTypeName;
        private Lazy<QueryFactory> queryFactory;

        private EntityProperty<Federation> federationReference;

        private string queryString;

        private string databaseVersionName;
        private string defaultDatasetName;
        private string defaultSchemaName;

        private List<DatasetBase> customDatasets;

        private ExecutionMode executionMode;

        [NonSerialized]
        private bool isCanceled;
        [NonSerialized]
        private Dictionary<string, ICancelableTask> cancelableTasks;

        private EntityProperty<ServerInstance> assignedServerInstanceReference;
        [NonSerialized]
        private string interpretedQueryString;
        [NonSerialized]
        private SelectStatement selectStatement;

        #endregion
        #region Properties

        [IgnoreDataMember]
        public Context Context
        {
            get { return context; }
            set
            {
                context = value;
                UpdateContext();
            }
        }

        [DataMember]
        public string QueryFactoryTypeName
        {
            get { return queryFactoryTypeName; }
            set { queryFactoryTypeName = value; }
        }

        [IgnoreDataMember]
        protected QueryFactory QueryFactory
        {
            get { return queryFactory.Value; }
        }

        [DataMember]
        public EntityProperty<Federation> FederationReference
        {
            get { return federationReference; }
            set { federationReference = value; }
        }

        /// <summary>
        /// Input query string
        /// </summary>
        [DataMember]
        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

        [DataMember]
        public string DatabaseVersionName
        {
            get { return databaseVersionName; }
            set { databaseVersionName = value; }
        }

        [DataMember]
        public string DefaultDatasetName
        {
            get { return defaultDatasetName; }
            set { defaultDatasetName = value; }
        }

        [DataMember]
        public string DefaultSchemaName
        {
            get { return defaultSchemaName; }
            set { defaultSchemaName = value; }
        }

        /// <summary>
        /// In case of Graywulf execution mode, this stores
        /// the datasets not in the default list (remote datasets,
        /// for instance)
        /// </summary>
        [DataMember]
        public List<DatasetBase> CustomDatasets
        {
            get { return customDatasets; }
            private set { customDatasets = value; }
        }

        /// <summary>
        /// Graywulf or single server
        /// </summary>
        [DataMember]
        public ExecutionMode ExecutionMode
        {
            get { return executionMode; }
            set { executionMode = value; }
        }

        [IgnoreDataMember]
        public bool IsCanceled
        {
            get { return isCanceled; }
        }

        [DataMember]
        public EntityProperty<ServerInstance> AssignedServerInstanceReference
        {
            get { return assignedServerInstanceReference; }
            set { assignedServerInstanceReference = value; }
        }

        [IgnoreDataMember]
        protected string InterpretedQueryString
        {
            get { return interpretedQueryString; }
            set { interpretedQueryString = value; }
        }

        [IgnoreDataMember]
        public SelectStatement SelectStatement
        {
            get { return selectStatement; }
            protected set { selectStatement = value; }
        }
            

        #endregion
        #region Constructors and initializers

        public QueryObject()
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryObject(Context context)
        {
            InitializeMembers(new StreamingContext());

            this.context = context;
        }

        public QueryObject(QueryObject old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.syncRoot = new object();

            this.context = null;

            this.queryFactoryTypeName = null;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);
            
            this.federationReference = new EntityProperty<Federation>();
            
            this.queryString = null;

            this.databaseVersionName = null;
            this.defaultDatasetName = null;
            this.defaultSchemaName = null;

            this.customDatasets = new List<DatasetBase>();

            this.executionMode = ExecutionMode.SingleServer;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityProperty<ServerInstance>();
            this.interpretedQueryString = null;
            this.selectStatement = null;
        }

        private void CopyMembers(QueryObject old)
        {
            this.syncRoot = new object();

            this.context = old.context;

            this.queryFactoryTypeName = old.queryFactoryTypeName;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.federationReference = new EntityProperty<Registry.Federation>(old.federationReference);
            
            this.queryString = old.queryString;

            this.databaseVersionName = old.databaseVersionName;
            this.defaultDatasetName = old.defaultDatasetName;
            this.defaultSchemaName = old.defaultSchemaName;

            this.customDatasets = new List<DatasetBase>(old.customDatasets);

            this.executionMode = old.executionMode;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityProperty<ServerInstance>(old.assignedServerInstanceReference);
            this.interpretedQueryString = null;
            this.selectStatement = null;
        }

        #endregion

        public Jhu.Graywulf.Registry.Context CreateContext(IGraywulfActivity activity, System.Activities.CodeActivityContext activityContext, Jhu.Graywulf.Registry.ConnectionMode connectionMode, Jhu.Graywulf.Registry.TransactionMode transactionMode)
        {
            switch (executionMode)
            {
                case Query.ExecutionMode.SingleServer:
                    return null;
                case Query.ExecutionMode.Graywulf:
                    return Jhu.Graywulf.Registry.ContextManager.Instance.CreateContext(activity, activityContext, connectionMode, transactionMode);
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void UpdateContext()
        {
            federationReference.Context = context;

            if (assignedServerInstanceReference != null)
            {
                assignedServerInstanceReference.Context = context;
            }
        }

        public void InitializeQueryObject(Context context)
        {
            InitializeQueryObject(context, false);
        }

        public virtual void InitializeQueryObject(Context context, bool forceReinitialize)
        {
            lock (syncRoot)
            {
                if (context != null)
                {
                    Context = context;

                    switch (executionMode)
                    {
                        case ExecutionMode.SingleServer:
                            break;
                        case ExecutionMode.Graywulf:
                            LoadAssignedServerInstance(forceReinitialize);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                Parse(forceReinitialize);
                Interpret(forceReinitialize);
                Validate();
            }
        }

        #region Cluster registry query functions

        /// <summary>
        /// Returns local datasets that are required to execute the query.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The function only returns GraywulfDatasets.
        /// </remarks>
        public Dictionary<string, GraywulfDataset> FindRequiredDatasets()
        {
            SchemaManager sc = CreateSchemaManager(false);

            // Collect list of required databases
            var ds = new Dictionary<string, GraywulfDataset>(StringComparer.CurrentCultureIgnoreCase);

            var trs = new List<TableReference>();
            foreach (TableReference tr in selectStatement.EnumerateTableSourceReferences(true))
            {
                if (!tr.IsSubquery && !tr.IsComputed)
                {
                    // Filter out non-graywulf datasets
                    if (!ds.ContainsKey(tr.DatasetName) && (sc.Datasets[tr.DatasetName] is GraywulfDataset))
                    {
                        ds.Add(tr.DatasetName, (GraywulfDataset)sc.Datasets[tr.DatasetName]);
                    }
                }
            }

            return ds;
        }

        protected void LoadAssignedServerInstance(bool forceReinitialize)
        {
            if (!assignedServerInstanceReference.IsEmpty || forceReinitialize)
            {
                assignedServerInstanceReference.LoadEntity();
                assignedServerInstanceReference.Value.GetConnectionString();
            }
        }

        #endregion
        #region Parsing functions

        /// <summary>
        /// Parses the query
        /// </summary>
        protected void Parse(bool forceReinitialize)
        {
            // Reparse only if needed
            if (selectStatement == null || forceReinitialize)
            {
                var parser = queryFactory.Value.CreateParser();
                selectStatement = (SelectStatement)parser.Execute(queryString);
            }
        }

        protected void Validate()
        {
            // **** TODO: move to SQ SkyQueryValidator vr = new SkyQueryValidator();

            // Validate on SQL level
            SqlValidator vr = new SqlValidator();
            vr.Execute(selectStatement);
        }

        /// <summary>
        /// Interprets the parsed query
        /// </summary>
        protected bool Interpret(bool forceReinitialize)
        {
            if (interpretedQueryString == null || forceReinitialize)
            {
                // --- Execute name resolution
                var nr = queryFactory.Value.CreateNameResolver();
                nr.SchemaManager = CreateSchemaManager(forceReinitialize);

                nr.DefaultSchemaName = defaultSchemaName;
                nr.DefaultDatasetName = defaultDatasetName;

                nr.Execute(selectStatement);

                // --- Normalize where conditions
                var wcn = new SearchConditionNormalizer();
                wcn.Execute(selectStatement.FindDescendant<QueryExpression>().FindDescendant<QuerySpecification>());

                FinishInterpret(forceReinitialize);

                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void FinishInterpret(bool forceReinitialize)
        {
            // --- Get interpreted query
            interpretedQueryString = SqlServerCodeGenerator.GetCode(selectStatement, true);
        }

        /// <summary>
        /// Creates a SqlSchemaConnector that will look up and cache database table schema
        /// information for query parsing
        /// </summary>
        /// <returns>An initialized SqlSchemaConnector instance.</returns>
        /// <remarks>
        /// The function adds custom datasets (usually MYDBs or remote dataset) defined
        /// for the query job.
        /// </remarks>
        protected virtual SchemaManager CreateSchemaManager(bool clearCache)
        {
            SchemaManager sc = CreateSchemaManagerInternal();

            if (clearCache)
            {
                SchemaManager.ClearCache();
            }

            // Add custom dataset defined by code
            foreach (var ds in customDatasets)
            {
                // *** TODO: check this
                sc.Datasets[ds.Name] = ds;
            }

            return sc;
        }

        private SchemaManager CreateSchemaManagerInternal()
        {
            switch (executionMode)
            {
                case ExecutionMode.SingleServer:
                    return new SqlServerSchemaManager();
                case ExecutionMode.Graywulf:
                    return new GraywulfSchemaManager(context, federationReference.Name);
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Cancelable command execution

        protected void RegisterCancelable(Guid key, ICancelableTask task)
        {
            RegisterCancelable(key.ToString(), task);
        }

        protected void RegisterCancelable(string key, ICancelableTask task)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Add(key, task);
            }
        }

        protected void UnregisterCancelable(Guid key)
        {
            UnregisterCancelable(key.ToString());
        }

        protected void UnregisterCancelable(string key)
        {
            lock (cancelableTasks)
            {
                cancelableTasks.Remove(key);
            }
        }

        public virtual void Cancel()
        {
            if (isCanceled)
            {
                throw new InvalidOperationException(Jhu.Graywulf.Tasks.ExceptionMessages.TaskAlreadyCanceled);
            }

            lock (cancelableTasks)
            {
                foreach (var t in cancelableTasks.Values)
                {
                    t.Cancel();
                }
            }

            isCanceled = true;
        }

        #endregion

        #region Generic SQL functions with cancel support
        // *** TODO: move all these to a util class

        protected void ExecuteLongCommandNonQuery(SqlCommand cmd)
        {

            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);

            RegisterCancelable(guid, ccmd);

            try
            {
#if !SKIPQUERIES
                ccmd.ExecuteNonQuery();
#endif
            }
            finally
            {
                UnregisterCancelable(guid);
            }

        }

        protected object ExecuteLongCommandScalar(SqlCommand cmd)
        {
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);

            RegisterCancelable(guid, ccmd);

            try
            {
#if !SKIPQUERIES
                return ccmd.ExecuteScalar();
#else
            return 0;
#endif
            }
            finally
            {
                UnregisterCancelable(guid);
            }
        }

        protected void ExecuteLongCommandReader(SqlCommand cmd, Action<IDataReader> action)
        {
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);

            RegisterCancelable(guid, ccmd);

            try
            {
                ccmd.ExecuteReader(action);
            }
            finally
            {
                UnregisterCancelable(guid);
            }
        }

        #endregion
        #region Specialized SQL manipulation function

        protected void CreateTable(string destinationDatabaseConnectionString, string destinationSchemaName, string destinationTableName, IEnumerable<ColumnReference> columns)
        {
            string sql = "CREATE TABLE [{0}].[{1}] ({2})";
            string columnlist = String.Empty;

            int q = 0;
            foreach (ColumnReference cr in columns)
            {
                if (q != 0)
                {
                    columnlist += ",\r\n";
                }

                columnlist += String.Format("{0} {1}", cr.ColumnAlias, cr.DataType);
                q++;
            }

            // Execute CREATE TABLE query 
            using (SqlConnection dcn = new SqlConnection(destinationDatabaseConnectionString))
            {
                dcn.Open();

                sql = String.Format(sql, destinationSchemaName, destinationTableName, columnlist);

                using (SqlCommand cmd = new SqlCommand(sql, dcn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected bool IsTableExisting(string databaseConnectionString, string schemaName, string tableName)
        {
            string sql = String.Format(
                "SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U')",
                schemaName,
                tableName);

            using (SqlConnection cn = new SqlConnection(databaseConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
        }

        protected IQueryImporter CreateQueryImporter(DatasetBase dataset, string sourceQuery, string destinationDatabaseConnectionString, string destinationSchemaName, string destinationTableName)
        {
            var qi = CreateQueryImporter(destinationDatabaseConnectionString, destinationSchemaName, destinationTableName);

            // Create bulk operation
            var source = new SourceQueryParameters();
            source.Dataset = dataset;
            source.Query = sourceQuery;

            qi.Source = source;

            return qi;
        }

        protected IQueryImporter CreateQueryImporter(string destinationDatabaseConnectionString, string destinationSchemaName, string destinationTableName)
        {
            var host = GetHostnameFromSqlConnectionString(destinationDatabaseConnectionString);

            var destination = new DestinationTableParameters();
            destination.Table = new Table()
            {
                Dataset = new SqlServerDataset("", destinationDatabaseConnectionString),
                SchemaName = destinationSchemaName,
                TableName = destinationTableName
            };
            destination.Operation = DestinationTableOperation.Append;
            

            var qi = RemoteServiceHelper.CreateObject<IQueryImporter>(host);
            qi.Destination = destination;

            return qi;
        }

        protected string GetHostnameFromSqlConnectionString(string connectionString)
        {
            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string
            // Get server name from data source name (requires trimming the sql server instance name)
            string host;

            var csb = new SqlConnectionStringBuilder(connectionString);
            int i = csb.DataSource.IndexOf('\\');
            if (i > -1)
            {
                host = csb.DataSource.Substring(i);
            }
            else
            {
                host = csb.DataSource;
            }

            return System.Net.Dns.GetHostEntry(host).HostName;
        }

        /// <summary>
        /// Takes a query, analyzes its columns and creates a table with the same schema
        /// in the destination database
        /// </summary>
        /// <param name="sourceDatabaseConnectionString"></param>
        /// <param name="sourceQuery"></param>
        /// <param name="destinationDatabaseConnectionString"></param>
        /// <param name="destinationSchemaName"></param>
        /// <param name="destinationTableName"></param>
        protected void CreateTableForBulkCopy(DatasetBase dataset, string sourceQuery, string destinationDatabaseConnectionString, string destinationSchemaName, string destinationTableName)
        {
#if !SKIPQUERIES

            var bcp = CreateQueryImporter(dataset, sourceQuery, destinationDatabaseConnectionString, destinationSchemaName, destinationTableName);
            bcp.CreateDestinationTable();

#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDatabaseConnectionString"></param>
        /// <param name="sourceQuery"></param>
        /// <param name="destinationDatabaseConnectionString"></param>
        /// <param name="destinationSchemaName"></param>
        /// <param name="destinationTableName"></param>
        /// <remarks>
        /// Function assumes that destination table is already created
        /// </remarks>
        protected void ExecuteBulkCopy(DatasetBase dataset, string sourceQuery, string destinationDatabaseConnectionString, string destinationSchemaName, string destinationTableName, int timeout)
        {
            var bcp = CreateQueryImporter(dataset, sourceQuery, destinationDatabaseConnectionString, destinationSchemaName, destinationTableName);
            bcp.Source.Timeout = timeout;
            bcp.Destination.BulkInsertTimeout = timeout;

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, bcp);

#if !SKIPQUERIES
            bcp.Execute();
#endif

            UnregisterCancelable(guid);
        }


        // TODO: clean up this a little bit

        protected void ExecuteSelectInto(string sourceDatabaseConnectionString, string sourceQuery, string destinationDatabaseName, string destinationSchemaName, string destinationTableName, int timeout)
        {
#if !SKIPQUERIES
            string sql = String.Format("SELECT tablealias.* INTO [{0}].[{1}].[{2}] FROM ({3}) AS tablealias",
                                       destinationDatabaseName,
                                       destinationSchemaName,
                                       destinationTableName,
                                       sourceQuery);

            using (SqlConnection cn = new SqlConnection(sourceDatabaseConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = timeout;

                    ExecuteLongCommandNonQuery(cmd);
                }
            }
#endif
        }

        protected void ExecuteInsertInto(string sourceDatabaseConnectionString, string sourceQuery, string destinationDatabaseName, string destinationSchemaName, string destinationTableName, int timeout)
        {
#if !SKIPQUERIES
            string sql = String.Format("INSERT [{0}].[{1}].[{2}] SELECT tablealias.* FROM ({3}) AS tablealias",
                                       destinationDatabaseName,
                                       destinationSchemaName,
                                       destinationTableName,
                                       sourceQuery);

            using (SqlConnection cn = new SqlConnection(sourceDatabaseConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = timeout;
                    ExecuteLongCommandNonQuery(cmd);
                }
            }
#endif
        }

        protected void TruncateTable(string connectionString, string schema, string table)
        {
#if !SKIPQUERIES
            string sql = String.Format("TRUNCATE TABLE [{0}].[{1}]", schema, table);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
#endif
        }

        protected void DropTable(string connectionString, string database, string schema, string table)
        {
#if !SKIPQUERIES
            string sql = String.Format(@"
USE [{0}]

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{1}' AND  TABLE_NAME = '{2}'))
BEGIN
    DROP TABLE [{0}].[{1}].[{2}]
END", database, schema, table);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
#endif
        }

        protected void DropView(string connectionString, string database, string schema, string view)
        {
#if !SKIPQUERIES
            string sql = String.Format(@"
USE [{0}]

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = '{1}' AND  TABLE_NAME = '{2}'))
BEGIN
    DROP VIEW [{0}].[{1}].[{2}]
END", database, schema, view);

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
#endif
        }

        #endregion

        public abstract object Clone();
    }
}
