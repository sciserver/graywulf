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

        [NonSerialized]
        private EntityProperty<DatabaseInstance> temporaryDatabaseInstanceReference;

        private ConcurrentDictionary<string, string> temporaryTables;
        private ConcurrentDictionary<string, string> temporaryViews;

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

        public ConcurrentDictionary<string, string> TemporaryTables
        {
            get { return temporaryTables; }
        }

        public ConcurrentDictionary<string, string> TemporaryViews
        {
            get { return temporaryViews; }
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

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>();

            this.temporaryTables = new ConcurrentDictionary<string, string>(SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, string>(SchemaManager.Comparer);
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

            this.temporaryDatabaseInstanceReference = new EntityProperty<DatabaseInstance>(old.temporaryDatabaseInstanceReference);

            this.temporaryTables = new ConcurrentDictionary<string, string>(old.temporaryTables, SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, string>(old.temporaryViews, SchemaManager.Comparer);
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
            SchemaManager sc = GetSchemaManager(false);

            // Collect list of required databases
            var ds = new Dictionary<string, GraywulfDataset>(SchemaManager.Comparer);
            var trs = new List<TableReference>();

            foreach (TableReference tr in selectStatement.EnumerateSourceTableReferences(true))
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
                var nr = queryFactory.Value.CreateNameResolver();
                nr.SchemaManager = GetSchemaManager(forceReinitialize);

                nr.DefaultSchemaName = defaultSchemaName;
                nr.DefaultDatasetName = defaultDatasetName;

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
            SchemaManager sc = CreateSchemaManager();

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

        #endregion
        #region Name substitution

#if false
        protected NameMapper CreateDatasetNameMapper(string databaseVersionName)
        {
            return CreateDatasetNameMapper(databaseVersionName, AssignedServerInstance.Guid, null, null);
        }

        protected NameMapper CreateDatasetNameMapper(string databaseVersionName, Guid serverInstance, string temporaryDatabaseName, string temporarySchemaName)
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:

                    // Replace remote table references with temp table references
                    foreach (TableReference tr in SelectStatement.EnumerateTableSourceReferences(true))
                    {
                        if (!tr.IsSubquery && temporaryTables.ContainsKey(tr.FullyQualifiedName))
                        {
                            throw new NotImplementedException();
                        }
                    }

                    return null;
                case ExecutionMode.Graywulf:
                    {
                        var sm = CreateSchemaManager(false);

                        var mapper = new NameMapper();

                        foreach (var tr in SelectStatement.EnumerateTableSourceReferences(true))
                        {
                            if (temporaryDatabaseName != null && temporaryTables.ContainsKey(tr.FullyQualifiedName))
                            {
                                // Replace remote table references with temp table references
                                AddRemoteTableNameMapping(mapper, sm, tr, temporaryDatabaseName, temporarySchemaName);
                            }
                            else if (!tr.IsSubquery && !tr.IsComputed)
                            {
                                AddDatabaseNameMapping(mapper, sm, tr, serverInstance, databaseVersionName);
                            }
                        }

                        return mapper;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        protected NameMapper CreateDatasetNameMapper(string databaseVersionName, Guid serverInstance, TableReference tr)
        {
            var sm = CreateSchemaManager(false);

            var mappings = new NameMapper();
            AddDatabaseNameMapping(mappings, sm, tr, serverInstance, databaseVersionName);

            return mappings;
        }

        private void AddDatabaseNameMapping(NameMapper mapper, SchemaManager sm, TableReference tr, Guid serverInstance, string databaseVersionName)
        {
            var ds = sm.Datasets[tr.DatasetName];

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
                    di.Guid = scheduler.GetDatabaseInstances(serverInstance, gwds.DatabaseDefinition.Guid, databaseVersionName)[0];
                    di.Load();
                }

                var nm = new DatasetNameMapping()
                {
                    DatabaseName = di.DatabaseName
                };

                // If already exists, make sure it refers to the same database
                if (mapper.DatasetNameMappings.ContainsKey(ds.Name))
                {
                    if (mapper.DatasetNameMappings[ds.Name].DatabaseName != nm.DatabaseName)
                    {
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    mapper.DatasetNameMappings.Add(ds.Name, nm);
                }
            }
        }

        private void AddRemoteTableNameMapping(NameMapper mapper, SchemaManager sm, TableReference tr, string temporaryDatabaseName, string temporarySchemaName)
        {
            var nm = new DatabaseObjectNameMapping()
            {
                DatabaseName = temporaryDatabaseInstanceReference.Value.DatabaseName,
                SchemaName = temporarySchemaName,
                ObjectName = temporaryTables[tr.FullyQualifiedName],
            };

            mapper.DatabaseObjectNameMappings.Add(tr.FullyQualifiedName, nm);

#if false
            // TODO: old code, delete
            // Replace name in the query
            tr.TableOrViewDescription.SchemaName = temporarySchemaName;
            tr.TableOrViewDescription.ObjectName = temporaryTables[tr.FullyQualifiedName];

            GraywulfDataset dd = new GraywulfDataset(tr.TableOrViewDescription.Dataset);

            dd.ConnectionString = temporaryDatabaseInstanceReference.Value.GetConnectionString().ConnectionString;
            dd.IsCacheable = false;
            dd.Name = "TEMP";   // TODO: ?
            tr.TableOrViewDescription.Dataset = dd;
#endif
        }
#endif

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
                            SubstituteDatabaseName(tr, serverInstance, databaseVersion);
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

                    //tr.DatabaseObject.Dataset = di.GetDataset();
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
            // TODO: write function to determine if a table is to be copied
            if (tr.IsCachable && TemporaryTables.ContainsKey(tr.UniqueName) &&
                IsRemoteDataset(sm.Datasets[tr.DatasetName]))
            {
                tr.DatabaseName = temporaryDataset.DatabaseName;
                tr.SchemaName = temporarySchemaName;
                tr.DatabaseObjectName = TemporaryTables[tr.UniqueName];
                tr.DatabaseObject = null;
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
            string sql = String.Format("SELECT __tablealias.* INTO [{0}].[{1}].[{2}] FROM ({3}) AS __tablealias",
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
            string sql = String.Format("INSERT [{0}].[{1}].[{2}] SELECT __tablealias.* FROM ({3}) AS __tablealias",
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
