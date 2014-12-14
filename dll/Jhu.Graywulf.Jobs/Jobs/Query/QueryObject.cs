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
using System.Xml.Serialization;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Jobs.Query
{
    /// <summary>
    /// Implements basic functions that are required for query execution
    /// in workflow environments.
    /// </summary>
    /// <remarks>
    /// This class is serialized by the workflow engine when persisted and
    /// serialized into XML when a job is created.
    /// </remarks>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class QueryObject : IContextObject, ICancelableTask, ICloneable
    {
        #region Member variables

        /// <summary>
        /// Used to synchronize on for certain operations that run in
        /// parallel when query workflows are executed
        /// </summary>
        [NonSerialized]
        internal object syncRoot;

        #endregion
        #region Property storage member variables

        /// <summary>
        /// Cache for registry context
        /// </summary>
        [NonSerialized]
        private Context context;

        /// <summary>
        /// Cache for the scheduler interface
        /// </summary>
        [NonSerialized]
        private IScheduler scheduler;

        /// <summary>
        /// Type name of the query factory class
        /// </summary>
        private string queryFactoryTypeName;

        /// <summary>
        /// Holds a reference to the query factory class
        /// </summary>
        [NonSerialized]
        private Lazy<QueryFactory> queryFactory;

        /// <summary>
        /// The original query to be executed
        /// </summary>
        private string queryString;

        private string batchName;
        private string queryName;

        /// <summary>
        /// The dataset to be assumed when no DATASET: part in
        /// table names appear.
        /// </summary>
        private SqlServerDataset defaultDataset;

        /// <summary>
        /// Dataset to store temporary tables during query execution.
        /// </summary>
        private SqlServerDataset temporaryDataset;

        /// <summary>
        /// Dataset to be used to find functions by default.
        /// </summary>
        private SqlServerDataset codeDataset;

        /// <summary>
        /// A list of custom datasets, i.e. those that are not
        /// configured centrally, for example MyDB
        /// </summary>
        private List<DatasetBase> customDatasets;

        /// <summary>
        /// Query execution mode, either single server or Graywulf cluster
        /// </summary>
        private ExecutionMode executionMode;

        /// <summary>
        /// Flag to know if query was already cancelled. Used in ICancelableTask
        /// implementation
        /// </summary>
        [NonSerialized]
        private bool isCanceled;

        /// <summary>
        /// Holds a list of ICancelableTask instances that are all to be canceled
        /// if the query workflow is canceled.
        /// </summary>
        [NonSerialized]
        private Dictionary<string, ICancelableTask> cancelableTasks;

        /// <summary>
        /// The root object of the query parsing tree
        /// </summary>
        [NonSerialized]
        private SelectStatement selectStatement;

        /// <summary>
        /// True, if the FinishInterpret function has completed.
        /// </summary>
        [NonSerialized]
        private bool isInterpretFinished;

        /// <summary>
        /// Holds a list of temporary tables created during query execution.
        /// Need to delete all these after the query has completed.
        /// </summary>
        private ConcurrentDictionary<string, Table> temporaryTables;

        /// <summary>
        /// Holds a list of temporary views created during query execution.
        /// Need to delete all these after the query has completed.
        /// </summary>
        private ConcurrentDictionary<string, View> temporaryViews;

        /// <summary>
        /// Holds a reference to the federation registry object.
        /// </summary>
        private EntityReference<Federation> federationReference;

        /// <summary>
        /// Holds a reference to the code database registry object.
        /// </summary>
        private EntityReference<DatabaseInstance> codeDatabaseInstanceReference;

        /// <summary>
        /// Holds a reference to temporary database registry object.
        /// </summary>
        private EntityReference<DatabaseInstance> temporaryDatabaseInstanceReference;

        /// <summary>
        /// Hold a reference to the server instance that was assigned
        /// by the scheduler to a given partition of the query.
        /// </summary>
        private EntityReference<ServerInstance> assignedServerInstanceReference;

        #endregion
        #region Properties

        [IgnoreDataMember]
        protected SqlServerCodeGenerator CodeGenerator
        {
            get
            {
                return new SqlServerCodeGenerator()
                {
                    ResolveNames = true
                };
            }
        }

        /// <summary>
        /// Gets or sets the registry context
        /// </summary>
        [IgnoreDataMember]
        public Context Context
        {
            get { return context; }
            set { context = value; }
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
        /// Gets a query factory instance.
        /// </summary>
        [IgnoreDataMember]
        protected QueryFactory QueryFactory
        {
            get { return queryFactory.Value; }
        }

        /// <summary>
        /// Gets or sets the Federation.
        /// </summary>
        [DataMember]
        public EntityReference<Federation> FederationReference
        {
            get { return federationReference; }
            set { federationReference = value; }
        }

        /// <summary>
        /// Gets or sets the query string of the query job.
        /// </summary>
        [DataMember]
        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

        [DataMember]
        public string BatchName
        {
            get { return batchName; }
            set { batchName = value; }
        }

        [DataMember]
        public string QueryName
        {
            get { return queryName; }
            set { queryName = value; }
        }

        /// <summary>
        /// Gets or sets the default dataset, i.e. the one that's assumed
        /// when no dataset part is specified in table names.
        /// </summary>
        [DataMember]
        public SqlServerDataset DefaultDataset
        {
            get { return defaultDataset; }
            set { defaultDataset = value; }
        }

        /// <summary>
        /// Gets or sets the temporary dataset to be used to store temporary.
        /// tables.
        /// </summary>
        [DataMember]
        public SqlServerDataset TemporaryDataset
        {
            get { return temporaryDataset; }
            set { temporaryDataset = value; }
        }

        /// <summary>
        /// Gets or sets the code database to be used by default to resolve function calls.
        /// </summary>
        [DataMember]
        public SqlServerDataset CodeDataset
        {
            get { return codeDataset; }
            set { codeDataset = value; }
        }

        /// <summary>
        /// Gets a list of custom datasets.
        /// </summary>
        /// <remarks>
        /// In case of Graywulf execution mode, this stores
        /// the datasets not in the default list (remote datasets,
        /// for instance)
        /// </remarks>
        [IgnoreDataMember]
        public List<DatasetBase> CustomDatasets
        {
            get { return customDatasets; }
            private set { customDatasets = value; }
        }

        [DataMember(Name = "CustomDatasets")]
        [XmlArray]
        public DatasetBase[] CustomDatasets_ForXml
        {
            get { return customDatasets.ToArray(); }
            set { customDatasets = new List<DatasetBase>(value); }
        }

        /// <summary>
        /// Gets or sets query execution mode.
        /// </summary>
        /// <remarks>
        /// Graywulf or single server
        /// </remarks>
        [DataMember]
        public ExecutionMode ExecutionMode
        {
            get { return executionMode; }
            set { executionMode = value; }
        }

        /// <summary>
        /// Gets if the query has been canceled.
        /// </summary>
        [IgnoreDataMember]
        public bool IsCanceled
        {
            get { return isCanceled; }
        }


        /// <summary>
        /// Gets or sets the reference to the assigned server instance registry object.
        /// </summary>
        [IgnoreDataMember]
        public EntityReference<ServerInstance> AssignedServerInstanceReference
        {
            get { return assignedServerInstanceReference; }
            set { assignedServerInstanceReference = value; }
        }

        /// <summary>
        /// Gets or sets the assigned server instance registry object.
        /// </summary>
        [IgnoreDataMember]
        public ServerInstance AssignedServerInstance
        {
            get { return assignedServerInstanceReference.Value; }
            set { assignedServerInstanceReference.Value = value; }
        }

        /// <summary>
        /// Gets or sets the root object of the query parsing tree.
        /// </summary>
        [IgnoreDataMember]
        public SelectStatement SelectStatement
        {
            get { return selectStatement; }
            protected set { selectStatement = value; }
        }

        /// <summary>
        /// Gets a reference to the temporary database instance registry object.
        /// </summary>
        [IgnoreDataMember]
        protected EntityReference<DatabaseInstance> TemporaryDatabaseInstanceReference
        {
            get { return temporaryDatabaseInstanceReference; }
        }

        /// <summary>
        /// Gets the list of temporary tables created during query execution.
        /// </summary>
        [IgnoreDataMember]
        public ConcurrentDictionary<string, Table> TemporaryTables
        {
            get { return temporaryTables; }
        }

        /// <summary>
        /// Gets the list of temporary views created during query execution.
        /// </summary>
        [IgnoreDataMember]
        public ConcurrentDictionary<string, View> TemporaryViews
        {
            get { return temporaryViews; }
        }

        /// <summary>
        /// Gets a reference to the code database instance registry object.
        /// </summary>
        [IgnoreDataMember]
        protected EntityReference<DatabaseInstance> CodeDatabaseInstanceReference
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

            this.federationReference = new EntityReference<Federation>(this);

            this.queryString = null;
            this.batchName = null;
            this.queryName = null;

            this.defaultDataset = null;
            this.temporaryDataset = null;
            this.codeDataset = null;
            this.customDatasets = new List<DatasetBase>();

            this.executionMode = ExecutionMode.SingleServer;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityReference<ServerInstance>(this);
            this.selectStatement = null;
            this.isInterpretFinished = false;

            this.temporaryDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this);

            this.temporaryTables = new ConcurrentDictionary<string, Table>(SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(SchemaManager.Comparer);

            this.codeDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this);
        }

        [OnDeserialized]
        private void UpdateMembers(StreamingContext context)
        {
            this.federationReference.ReferencingObject = this;
            this.assignedServerInstanceReference.ReferencingObject = this;
            this.temporaryDatabaseInstanceReference.ReferencingObject = this;
            this.codeDatabaseInstanceReference.ReferencingObject = this;
        }

        private void CopyMembers(QueryObject old)
        {
            this.syncRoot = new object();

            this.context = old.context;
            this.scheduler = old.scheduler;

            this.queryFactoryTypeName = old.queryFactoryTypeName;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.federationReference = new EntityReference<Registry.Federation>(this, old.federationReference);

            this.queryString = old.queryString;
            this.batchName = old.batchName;
            this.queryName = old.queryName;

            this.defaultDataset = old.defaultDataset;
            this.temporaryDataset = old.temporaryDataset;
            this.codeDataset = old.codeDataset;
            this.customDatasets = new List<DatasetBase>(old.customDatasets);

            this.executionMode = old.executionMode;

            this.isCanceled = false;
            this.cancelableTasks = new Dictionary<string, ICancelableTask>();

            this.assignedServerInstanceReference = new EntityReference<ServerInstance>(this, old.assignedServerInstanceReference);
            this.selectStatement = null;
            this.isInterpretFinished = false;

            this.temporaryDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this, old.temporaryDatabaseInstanceReference);

            this.temporaryTables = new ConcurrentDictionary<string, Table>(old.temporaryTables, SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(old.temporaryViews, SchemaManager.Comparer);

            this.codeDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this, old.codeDatabaseInstanceReference);
        }

        #endregion

        /// <summary>
        /// Returnes a new registry context when in Graywulf execution mode.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="activityContext"></param>
        /// <param name="connectionMode"></param>
        /// <param name="transactionMode"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="context"></param>
        public void InitializeQueryObject(Context context)
        {
            InitializeQueryObject(context, null, false);
        }

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduler"></param>
        public void InitializeQueryObject(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler, false);
        }

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduler"></param>
        /// <param name="forceReinitialize"></param>
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
            var sc = GetSchemaManager();

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
            if (!isInterpretFinished || forceReinitialize)
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

        /// <summary>
        /// Performes additional interpretation steps after the query has been parsed.
        /// </summary>
        /// <param name="forceReinitialize"></param>
        protected virtual void FinishInterpret(bool forceReinitialize)
        {
            this.isInterpretFinished = true;
        }

        /// <summary>
        /// Returns a schema manager, either the cached one, either a newly
        /// created one.
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        protected virtual SchemaManager GetSchemaManager()
        {
            var sc = CreateSchemaManager();

            // Add custom dataset defined by code
            foreach (var ds in customDatasets)
            {
                // *** TODO: check this
                sc.Datasets[ds.Name] = ds;
            }

            return sc;
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
        private SchemaManager CreateSchemaManager()
        {
            switch (executionMode)
            {
                case ExecutionMode.SingleServer:
                    return new Schema.SqlServer.SqlServerSchemaManager();
                case ExecutionMode.Graywulf:
                    return GraywulfSchemaManager.Create(federationReference.Value);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns a new name resolver to be used with the parsed query string.
        /// </summary>
        /// <param name="forceReinitialize"></param>
        /// <returns></returns>
        protected virtual SqlNameResolver CreateNameResolver(bool forceReinitialize)
        {
            var nr = queryFactory.Value.CreateNameResolver();
            nr.SchemaManager = GetSchemaManager();

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

        /// <summary>
        /// Substitutes the database name into a table reference.
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>
        /// During query executions, actual database name are not known until a server instance is
        /// assigned to the query partition.
        /// </remarks>
        protected void SubstituteDatabaseName(TableReference tr, Guid serverInstance, string databaseVersion)
        {
            SchemaManager sc = GetSchemaManager();

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
                        di = gwds.DatabaseInstanceReference.Value;
                    }
                    else
                    {
                        // Find appropriate database instance
                        di = new DatabaseInstance(Context);
                        di.Guid = scheduler.GetDatabaseInstances(serverInstance, gwds.DatabaseDefinitionReference.Guid, databaseVersion)[0];
                        di.Load();
                    }

                    tr.DatabaseName = di.DatabaseName;
                }
            }
        }

        /// <summary>
        /// Substitutes names of remote tables with name of temporary tables
        /// holding a cached version of remote tables.
        /// </summary>
        /// <remarks></remarks>
        // TODO: This function call must be synchronized! ??
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
                    var sm = GetSchemaManager();

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

        /// <summary>
        /// Substitutes the name of a remote tables with name of the temporary table
        /// holding a cached version of the remote data.
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="tr"></param>
        /// <param name="temporaryDataset"></param>
        /// <param name="temporarySchemaName"></param>
        private void SubstituteRemoteTableName(SchemaManager sm, TableReference tr, DatasetBase temporaryDataset, string temporarySchemaName)
        {
            // Save unique name because it will change as names are substituted
            var un = tr.UniqueName;

            // TODO: write function to determine if a table is to be copied
            // ie. the condition in the if clause of the following line
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

        public virtual void Execute()
        {
            throw new NotImplementedException();
        }

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

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connectionString"></param>
        /// <param name="timeout"></param>
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

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="cmd"></param>
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

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="action"></param>
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
