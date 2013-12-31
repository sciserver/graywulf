using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.IO;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class QueryObject : ICancelableTask, ICloneable
    {
        #region Member variables

        [NonSerialized]
        internal object syncRoot;

        #endregion
        #region Property storage member variables

        [NonSerialized]
        private Context context;

        [NonSerialized]
        private IScheduler scheduler;

        private string queryFactoryTypeName;
        private Lazy<QueryFactory> queryFactory;

        private EntityProperty<Federation> federationReference;

        private string queryString;

        private SqlServerDataset defaultDataset;
        private SqlServerDataset temporaryDataset;
        private SqlServerDataset codeDataset;
        private List<DatasetBase> customDatasets;

        //private string databaseVersionName;
        //private string defaultDatasetName;
        //private string defaultSchemaName;
      
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

        [NonSerialized]
        private EntityProperty<DatabaseInstance> temporaryDatabaseInstanceReference;

        private ConcurrentDictionary<string, Table> temporaryTables;
        private ConcurrentDictionary<string, View> temporaryViews;

        [NonSerialized]
        private EntityProperty<DatabaseInstance> codeDatabaseInstanceReference;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the registry context
        /// </summary>
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

        /// <summary>
        /// Gets the scheduler instance
        /// </summary>
        [IgnoreDataMember]
        public IScheduler Scheduler
        {
            get { return scheduler; }
        }

        /// <summary>
        /// Gets or sets the type name string of the query factory class
        /// </summary>
        [DataMember]
        public string QueryFactoryTypeName
        {
            get { return queryFactoryTypeName; }
            set { queryFactoryTypeName = value; }
        }

        /// <summary>
        /// Gets a query factory instance
        /// </summary>
        [IgnoreDataMember]
        protected QueryFactory QueryFactory
        {
            get { return queryFactory.Value; }
        }

        /// <summary>
        /// Gets or sets the Federation
        /// </summary>
        [DataMember]
        public EntityProperty<Federation> FederationReference
        {
            get { return federationReference; }
            set { federationReference = value; }
        }

        /// <summary>
        /// Gets or sets the query string of the query job
        /// </summary>
        [DataMember]
        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

#if false
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This cannot be an entity reference as versions names identify
        /// databases accross database definitions
        /// </remarks>
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
#endif

        [DataMember]
        public SqlServerDataset DefaultDataset
        {
            get { return defaultDataset; }
            set { defaultDataset = value; }
        }

        [DataMember]
        public SqlServerDataset TemporaryDataset
        {
            get { return temporaryDataset; }
            set { temporaryDataset = value; }
        }

        [DataMember]
        public SqlServerDataset CodeDataset
        {
            get { return codeDataset; }
            set { codeDataset = value; }
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


        [DataMember]    // TODO: does it have to be serialized?
        public EntityProperty<ServerInstance> AssignedServerInstanceReference
        {
            get { return assignedServerInstanceReference; }
            set { assignedServerInstanceReference = value; }
        }

        public ServerInstance AssignedServerInstance
        {
            get { return assignedServerInstanceReference.Value; }
            set { assignedServerInstanceReference.Value = value; }
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

        protected EntityProperty<DatabaseInstance> TemporaryDatabaseInstanceReference
        {
            get { return temporaryDatabaseInstanceReference; }
        }

        public ConcurrentDictionary<string, Table> TemporaryTables
        {
            get { return temporaryTables; }
        }

        public ConcurrentDictionary<string, View> TemporaryViews
        {
            get { return temporaryViews; }
        }

        protected EntityProperty<DatabaseInstance> CodeDatabaseInstanceReference
        {
            get { return codeDatabaseInstanceReference; }
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
            this.scheduler = null;

            this.queryFactoryTypeName = null;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.federationReference = new EntityProperty<Federation>();

            this.queryString = null;

            this.defaultDataset = null;
            this.temporaryDataset = null;
            this.codeDataset = null;
            this.customDatasets = new List<DatasetBase>();

            /*this.databaseVersionName = null;
            this.defaultDatasetName = null;
            this.defaultSchemaName = null;*/

            this.executionMode = ExecutionMode.SingleServer;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityProperty<ServerInstance>();
            this.interpretedQueryString = null;
            this.selectStatement = null;

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>();

            this.temporaryTables = new ConcurrentDictionary<string, Table>(SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(SchemaManager.Comparer);

            this.codeDatabaseInstanceReference = new EntityProperty<DatabaseInstance>();
        }

        private void CopyMembers(QueryObject old)
        {
            this.syncRoot = new object();

            this.context = old.context;
            this.scheduler = old.scheduler;

            this.queryFactoryTypeName = old.queryFactoryTypeName;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.federationReference = new EntityProperty<Registry.Federation>(old.federationReference);

            this.queryString = old.queryString;

            //this.databaseVersionName = old.databaseVersionName;
            //this.defaultDatasetName = old.defaultDatasetName;
            //this.defaultSchemaName = old.defaultSchemaName;
            this.defaultDataset = old.defaultDataset;
            this.temporaryDataset = old.temporaryDataset;
            this.codeDataset = old.codeDataset;
            this.customDatasets = new List<DatasetBase>(old.customDatasets);

            this.executionMode = old.executionMode;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityProperty<ServerInstance>(old.assignedServerInstanceReference);
            this.interpretedQueryString = null;
            this.selectStatement = null;

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>(old.temporaryDatabaseInstanceReference);

            this.temporaryTables = new ConcurrentDictionary<string, Table>(old.temporaryTables, SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(old.temporaryViews, SchemaManager.Comparer);

            this.codeDatabaseInstanceReference = new EntityProperty<DatabaseInstance>(old.codeDatabaseInstanceReference);
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

            if (temporaryDatabaseInstanceReference != null)
            {
                temporaryDatabaseInstanceReference.Context = Context;
            }

            if (codeDatabaseInstanceReference != null)
            {
                codeDatabaseInstanceReference.Context = Context;
            }
        }

        public void InitializeQueryObject(Context context)
        {
            InitializeQueryObject(context, null, false);
        }

        public void InitializeQueryObject(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler, false);
        }

        public virtual void InitializeQueryObject(Context context, IScheduler scheduler, bool forceReinitialize)
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

                if (scheduler != null)
                {
                    this.scheduler = scheduler;
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
            var sc = GetSchemaManager(false);

            // Collect list of required databases
            var ds = new Dictionary<string, GraywulfDataset>(SchemaManager.Comparer);
            var trs = new List<TableReference>();

            foreach (var tr in selectStatement.EnumerateSourceTableReferences(true))
            {
                if (!tr.IsUdf && !tr.IsSubquery && !tr.IsComputed)
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
            var validator = queryFactory.Value.CreateValidator();
            validator.Execute(selectStatement);
        }

        /// <summary>
        /// Interprets the parsed query
        /// </summary>
        protected bool Interpret(bool forceReinitialize)
        {
            if (interpretedQueryString == null || forceReinitialize)
            {
                // --- Execute name resolution
                var nr = CreateNameResolver(forceReinitialize);
                nr.Execute(selectStatement);

                // --- Normalize where conditions
                var wcn = new SearchConditionNormalizer();
                wcn.Execute(selectStatement);

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

            // Ezen a ponton a konkrét adatbázis instance neve kell, és nem a prototípusé!!!
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
        protected virtual SchemaManager GetSchemaManager(bool clearCache)
        {
            var sc = CreateSchemaManager();

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

        private SchemaManager CreateSchemaManager()
        {
            switch (executionMode)
            {
                case ExecutionMode.SingleServer:
                    return new Schema.SqlServer.SqlServerSchemaManager();
                case ExecutionMode.Graywulf:
                    return new GraywulfSchemaManager(context, federationReference.Name);
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual SqlNameResolver CreateNameResolver(bool forceReinitialize)
        {
            var nr = queryFactory.Value.CreateNameResolver();
            nr.SchemaManager = GetSchemaManager(forceReinitialize);

            nr.DefaultTableDatasetName = defaultDataset.Name;
            nr.DefaultFunctionDatasetName = codeDataset.Name;
            
            return nr;
        }

        #endregion
        #region Name substitution

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        protected void SubstituteDatabaseNames(Guid serverInstance, string databaseVersion)
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // *** Nothing to do here?
                    break;
                case ExecutionMode.Graywulf:
                    {
                        var ef = new Jhu.Graywulf.Registry.EntityFactory(Context);

                        foreach (var tr in SelectStatement.EnumerateSourceTableReferences(true))
                        {
                            if (!tr.IsUdf && !tr.IsSubquery && !tr.IsComputed)
                            {
                                SubstituteDatabaseName(tr, serverInstance, databaseVersion);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void SubstituteDatabaseName(TableReference tr, Guid serverInstance, string databaseVersion)
        {
            SchemaManager sc = GetSchemaManager(false);

            if (!tr.IsSubquery && !tr.IsComputed)
            {
                DatasetBase ds = sc.Datasets[tr.DatasetName];

                // Graywulf datasets have changing database names depending on the server
                // the database is on.
                if (ds is GraywulfDataset)
                {
                    var gwds = ds as GraywulfDataset;
                    gwds.Context = Context;

                    DatabaseInstance di;
                    if (gwds.IsSpecificInstanceRequired)
                    {
                        di = gwds.DatabaseInstance.Value;
                    }
                    else
                    {
                        // Find appropriate database instance
                        di = new DatabaseInstance(Context);
                        di.Guid = scheduler.GetDatabaseInstances(serverInstance, gwds.DatabaseDefinition.Guid, databaseVersion)[0];
                        di.Load();
                    }

                    tr.DatabaseName = di.DatabaseName;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// /// <remarks>This function call must be synchronized!</remarks>
        protected virtual void SubstituteRemoteTableNames(DatasetBase temporaryDataset, string temporarySchemaName)
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // No remote table support

                    // Replace remote table references with temp table references
                    foreach (TableReference tr in SelectStatement.EnumerateSourceTableReferences(true))
                    {
                        if (!tr.IsSubquery && TemporaryTables.ContainsKey(tr.UniqueName))
                        {
                            throw new NotImplementedException();
                        }
                    }
                    break;
                case ExecutionMode.Graywulf:
                    var sm = GetSchemaManager(false);

                    // Replace remote table references with temp table references
                    foreach (TableReference tr in SelectStatement.EnumerateSourceTableReferences(true))
                    {
                        SubstituteRemoteTableName(sm, tr, temporaryDataset, temporarySchemaName);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SubstituteRemoteTableName(SchemaManager sm, TableReference tr, DatasetBase temporaryDataset, string temporarySchemaName)
        {
            // Save unique name because it will change as names are substituted
            var un = tr.UniqueName;

            // TODO: write function to determine if a table is to be copied
            // ie. the condition in the if clause
            if (tr.IsCachable && TemporaryTables.ContainsKey(tr.UniqueName) &&
                IsRemoteDataset(sm.Datasets[tr.DatasetName]))
            {
                tr.DatabaseName = temporaryDataset.DatabaseName;
                tr.SchemaName = temporarySchemaName;
                tr.DatabaseObjectName = TemporaryTables[un].TableName;
                tr.DatabaseObject = null;
            }
        }

        /// <summary>
        /// Checks whether the given dataset is remote to the assigned server
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
                var csba = AssignedServerInstance.GetConnectionString();

                return StringComparer.InvariantCultureIgnoreCase.Compare(csbr.DataSource, csba.DataSource) != 0;
            }
            else
            {
                // Everything else is remote
                return true;
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
                throw new InvalidOperationException(ExceptionMessages.TaskAlreadyCanceled);
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
        /*
        protected void ExecuteCommandNonQuery(string sql, string connectionString)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
#if !SKIPQUERIES
                    cmd.ExecuteNonQuery();
#endif
                }
            }
        }
         * */

        /*
        protected object ExecuteCommandScalar(string sql, string connectionString)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    return cmd.ExecuteScalar();
                }
            }
        }*/

        protected void ExecuteLongCommandNonQuery(string sql, string connectionString, int timeout)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandTimeout = timeout;
                    ExecuteLongCommandNonQuery(cmd);
                }
            }
        }

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

        /* TODO: moved to schema, delete
        protected bool IsTableExisting(Table table)
        {
            // TODO: rewrite this and move function to schema

            string sql = String.Format(
                "SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U')",
                table.SchemaName,
                table.TableName);

            return (int)ExecuteCommandScalar(sql, table.Dataset.ConnectionString) == 1;
        }*/

        /* TODO: delete, not used ?
        protected void CreateTable(TableReference table)
        {
            string sql = "CREATE TABLE [{0}].[{1}] ({2})";
            string columnlist = String.Empty;

            int q = 0;
            foreach (var cr in table.ColumnReferences)
            {
                if (q != 0)
                {
                    columnlist += ",\r\n";
                }

                columnlist += String.Format("{0} {1}", cr.ColumnAlias, cr.DataType);
                q++;
            }

            sql = String.Format(sql, table.SchemaName, table.DatabaseObjectName, columnlist);

            ExecuteCommandNonQuery(sql, table.DatabaseObject.Dataset.ConnectionString);
        }
         * */

        /*
        protected void CreateTableForBulkCopy(SourceQueryParameters source, DestinationTableParameters destination, bool local)
        {
#if !SKIPQUERIES

            var bcp = CreateTableCopyTask(source, destination, local);
            bcp.CreateDestinationTable();

#endif
        }
         * */

        /*
        protected void ExecuteBulkCopy(SourceQueryParameters source, DestinationTableParameters destination, bool local, int timeout)
        {
            var bcp = CreateTableCopyTask(source, destination, local);
            bcp.Destination.BulkInsertTimeout = timeout;

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, bcp);

#if !SKIPQUERIES
            bcp.Execute();
#endif

            UnregisterCancelable(guid);
        }
         * */

        /*
        protected void DropTableOrView(TableOrView tableOrView)
        {
            // TODO: move this to schema eventually
            string sql = @"
IF (OBJECT_ID('[{0}].[{1}].[{2}]') IS NOT NULL)
BEGIN
    DROP {3} [{0}].[{1}].[{2}]
END";

            sql = String.Format(
                sql,
                !String.IsNullOrWhiteSpace(tableOrView.DatabaseName) ? tableOrView.DatabaseName : tableOrView.Dataset.DatabaseName,
                tableOrView.SchemaName,
                tableOrView.ObjectName,
                tableOrView.GetType().Name);

            ExecuteCommandNonQuery(sql, tableOrView.Dataset.ConnectionString);
        }*/

        protected void ExecuteSelectInto(SourceTableQuery source, Table destination, int timeout)
        {
            string sql = String.Format(
                "SELECT __tablealias.* INTO [{0}].[{1}].[{2}] FROM ({3}) AS __tablealias",
                !String.IsNullOrWhiteSpace(destination.DatabaseName) ? destination.DatabaseName : destination.Dataset.DatabaseName,
                destination.SchemaName,
                destination.TableName,
                source.Query);

            ExecuteLongCommandNonQuery(sql, source.Dataset.ConnectionString, timeout);
        }

        protected void ExecuteInsertInto(SourceTableQuery source, Table destination, int timeout)
        {
            string sql = String.Format(
                "INSERT [{0}].[{1}].[{2}] WITH (TABLOCKX) SELECT __tablealias.* FROM ({3}) AS __tablealias",
                !String.IsNullOrWhiteSpace(destination.DatabaseName) ? destination.DatabaseName : destination.Dataset.DatabaseName,
                destination.SchemaName,
                destination.TableName,
                source.Query);

            ExecuteLongCommandNonQuery(sql, source.Dataset.ConnectionString, timeout);
        }

        /// <summary>
        /// Creates and initializes a remote or local table copy task
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="local"></param>
        /// <returns></returns>
        protected ICopyTable CreateTableCopyTask(SourceTableQuery source, DestinationTable destination, bool local)
        {
            var desthost = GetHostnameFromSqlConnectionString(destination.Dataset.ConnectionString);

            ICopyTable qi;

            if (local)
            {
                qi = new CopyTable();
            }
            else
            {
                qi = RemoteServiceHelper.CreateObject<ICopyTable>(desthost);
            }

            qi.Source = source;
            qi.Destination = destination;

            return qi;
        }

        private string GetHostnameFromSqlConnectionString(string connectionString)
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

            // The best way to find a served would be to do a reverse lookup but
            // GW config is always broken, so we use IP address instead
            //return System.Net.Dns.GetHostEntry(host).HostName;
            return host;
        }

        #endregion

        public abstract object Clone();
    }
}
