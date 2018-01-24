using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Sql.Jobs.Query
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
    public abstract class QueryObject : CancelableTask, IRegistryContextObject, ICloneable
    {
        protected delegate Task AsyncReaderAction(SqlDataReader dr, CancellationToken cancellationToken);

        #region Property storage member variables

        /// <summary>
        /// Used to synchronize on for certain operations that run in
        /// parallel when query workflows are executed
        /// </summary>
        [NonSerialized]
        internal object syncRoot;

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

        /// <summary>
        /// Query details after parsing and name resolution
        /// </summary>
        private QueryDetails queryDetails;

        /// <summary>
        /// Batch name for automatic output table naming
        /// </summary>
        private string batchName;

        /// <summary>
        /// Query name for automatic output table naming
        /// </summary>
        private string queryName;

        /// <summary>
        /// Database version to be used to execute the queries (PROD)
        /// </summary>
        private string sourceDatabaseVersionName;

        /// <summary>
        /// Database version to be used to calculate statistics (STAT)
        /// </summary>
        private string statDatabaseVersionName;

        /// <summary>
        /// Individual query time-out, overall job timeout is enforced by
        /// the scheduler in a different way.
        /// </summary>
        private int queryTimeout;

        private int maxPartitions;

        /// <summary>
        /// Determines if queries are dumped into files during execution
        /// </summary>
        private bool dumpSql;

        /// <summary>
        /// Cache for registry context
        /// </summary>
        [NonSerialized]
        private RegistryContext registryContext;

        /// <summary>
        /// Cache for the scheduler interface
        /// </summary>
        [NonSerialized]
        private IScheduler scheduler;

        /// <summary>
        /// The dataset to be assumed when no DATASET: part in
        /// table names appear.
        /// </summary>
        private SqlServerDataset defaultSourceDataset;

        /// <summary>
        /// The dataset to be assumes when no DATASET: part in
        /// SELECT INTO and CREATE TABLE statements appear
        /// </summary>
        private SqlServerDataset defaultOutputDataset;

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
        /// Holds table statistics gathered for all the tables in the query
        /// </summary>
        private Dictionary<ITableSource, TableStatistics> tableStatistics;

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
        /// Hold a reference to the server instance that was assigned
        /// by the scheduler to a given partition of the query.
        /// </summary>
        private EntityReference<ServerInstance> assignedServerInstanceReference;

        /// <summary>
        /// Holds a reference to temporary database registry object, once server is assigned
        /// </summary>
        private EntityReference<DatabaseInstance> temporaryDatabaseInstanceReference;

        /// <summary>
        /// Holds a reference to the code database registry object, once server is assigned
        /// </summary>
        private EntityReference<DatabaseInstance> codeDatabaseInstanceReference;

        #endregion
        #region Properties

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
        /// Gets or sets the query string of the query job.
        /// </summary>
        [DataMember]
        public string QueryString
        {
            get { return queryString; }
            set { queryString = value; }
        }

        /// <summary>
        /// Gets or sets the query details, including the query parsing tree and
        /// name resolution results.
        /// </summary>
        [DataMember]
        public QueryDetails QueryDetails
        {
            get { return queryDetails; }
            protected set { queryDetails = value; }
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

        [DataMember]
        public string SourceDatabaseVersionName
        {
            get { return sourceDatabaseVersionName; }
            set { sourceDatabaseVersionName = value; }
        }

        [DataMember]
        public string StatDatabaseVersionName
        {
            get { return statDatabaseVersionName; }
            set { statDatabaseVersionName = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of individual queries
        /// </summary>
        /// <remarks>
        /// The overall timeout period is enforced by the scheduler.
        /// </remarks>
        [DataMember]
        public int QueryTimeout
        {
            get { return queryTimeout; }
            set { queryTimeout = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of partitions
        /// </summary>
        [DataMember]
        public int MaxPartitions
        {
            get { return maxPartitions; }
            set { maxPartitions = value; }
        }

        /// <summary>
        /// Gets or sets whether SQL scripts are dumped to files during query execution.
        /// </summary>
        [DataMember]
        public bool DumpSql
        {
            get { return dumpSql; }
            set { dumpSql = value; }
        }

        /// <summary>
        /// Gets or sets the registry context
        /// </summary>
        [IgnoreDataMember]
        public RegistryContext RegistryContext
        {
            get { return registryContext; }
            set { UpdateContext(value); }
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
        /// Gets or sets the default dataset, i.e. the one that's assumed
        /// when no dataset part is specified in table names.
        /// </summary>
        [DataMember]
        public SqlServerDataset DefaultSourceDataset
        {
            get { return defaultSourceDataset; }
            set { defaultSourceDataset = value; }
        }

        /// <summary>
        /// Gets or sets the default dataset for table output
        /// </summary>
        [DataMember]
        public SqlServerDataset DefaultOutputDataset
        {
            get { return defaultOutputDataset; }
            set { defaultOutputDataset = value; }
        }

        /// <summary>
        /// Gets or sets the temporary dataset to be used to store temporary.
        /// tables.
        /// </summary>
        [IgnoreDataMember]
        public SqlServerDataset TemporaryDataset
        {
            get
            {
                SqlServerDataset tempds;

                if (executionMode == Query.ExecutionMode.SingleServer || temporaryDatabaseInstanceReference.IsEmpty)
                {
                    tempds = temporaryDataset;
                }
                else if (executionMode == ExecutionMode.Graywulf)
                {
                    tempds = temporaryDatabaseInstanceReference.Value.GetDataset();
                }
                else
                {
                    throw new NotImplementedException();
                }

                tempds.IsMutable = true;
                return tempds;
            }
            set { temporaryDataset = value; }
        }


        /// <summary>
        /// Gets or sets the code database to be used by default to resolve function calls.
        /// </summary>
        [IgnoreDataMember]
        public SqlServerDataset CodeDataset
        {
            get
            {
                SqlServerDataset codeds;

                if (executionMode == Query.ExecutionMode.SingleServer || codeDatabaseInstanceReference.IsEmpty)
                {
                    codeds = codeDataset;
                }
                else if (executionMode == ExecutionMode.Graywulf)
                {
                    codeds = codeDatabaseInstanceReference.Value.GetDataset();
                }
                else
                {
                    throw new NotImplementedException();
                }

                return codeds;
            }
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
        /// Gets or sets the reference to the assigned server instance registry object.
        /// </summary>
        [DataMember]
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
        }

        /// <summary>
        /// Holds a list of statistics computed for the tables accessed
        /// by the query
        /// </summary>
        [IgnoreDataMember]
        public Dictionary<ITableSource, TableStatistics> TableStatistics
        {
            get { return tableStatistics; }
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

        #endregion
        #region Constructors and initializers

        public QueryObject()
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryObject(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryObject(CancellationContext cancellationContext, RegistryContext context)
            : base(cancellationContext)
        {
            InitializeMembers(new StreamingContext());

            this.registryContext = context;
        }

        public QueryObject(QueryObject old)
            : base(old.CancellationContext)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.syncRoot = new object();

            this.queryFactoryTypeName = null;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.queryString = null;
            this.queryDetails = new QueryDetails();

            this.batchName = null;
            this.queryName = null;

            this.sourceDatabaseVersionName = String.Empty;
            this.statDatabaseVersionName = String.Empty;

            this.queryTimeout = 60;
            this.maxPartitions = 0;
            this.dumpSql = false;

            this.registryContext = null;
            this.scheduler = null;

            this.defaultSourceDataset = null;
            this.defaultOutputDataset = null;
            this.temporaryDataset = null;
            this.codeDataset = null;
            this.customDatasets = new List<DatasetBase>();

            this.executionMode = ExecutionMode.SingleServer;

            this.federationReference = new EntityReference<Federation>(this);
            this.assignedServerInstanceReference = new EntityReference<ServerInstance>(this);
            this.temporaryDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this);
            this.codeDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this);

            this.tableStatistics = new Dictionary<ITableSource, TableStatistics>();
            this.temporaryTables = new ConcurrentDictionary<string, Table>(SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(SchemaManager.Comparer);
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

            this.queryFactoryTypeName = old.queryFactoryTypeName;
            this.queryFactory = new Lazy<QueryFactory>(() => (QueryFactory)Activator.CreateInstance(Type.GetType(queryFactoryTypeName)), false);

            this.queryString = old.queryString;
            this.queryDetails = new QueryDetails(old.queryDetails);

            this.batchName = old.batchName;
            this.queryName = old.queryName;

            this.sourceDatabaseVersionName = old.sourceDatabaseVersionName;
            this.statDatabaseVersionName = old.statDatabaseVersionName;

            this.queryTimeout = old.queryTimeout;
            this.maxPartitions = old.maxPartitions;
            this.dumpSql = old.dumpSql;

            this.registryContext = old.registryContext;
            this.scheduler = old.scheduler;

            this.defaultSourceDataset = old.defaultSourceDataset;
            this.defaultOutputDataset = old.defaultOutputDataset;
            this.temporaryDataset = old.temporaryDataset;
            this.codeDataset = old.codeDataset;
            this.customDatasets = new List<DatasetBase>(old.customDatasets);

            this.executionMode = old.executionMode;

            this.federationReference = new EntityReference<Registry.Federation>(this, old.federationReference);
            this.assignedServerInstanceReference = new EntityReference<ServerInstance>(this, old.assignedServerInstanceReference);
            this.temporaryDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this, old.temporaryDatabaseInstanceReference);
            this.codeDatabaseInstanceReference = new EntityReference<DatabaseInstance>(this, old.codeDatabaseInstanceReference);

            this.tableStatistics = new Dictionary<ITableSource, TableStatistics>();
            this.temporaryTables = new ConcurrentDictionary<string, Table>(old.temporaryTables, SchemaManager.Comparer);
            this.temporaryViews = new ConcurrentDictionary<string, View>(old.temporaryViews, SchemaManager.Comparer);
        }

        public abstract object Clone();

        #endregion
        #region Query object initialization and parsing

        public void InitializeQueryObject(RegistryContext registryContext)
        {
            InitializeQueryObject(null, registryContext, null, true);
        }

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="context"></param>
        public void InitializeQueryObject(CancellationContext cancellationContext, RegistryContext registryContext)
        {
            InitializeQueryObject(cancellationContext, registryContext, null, true);
        }

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheduler"></param>
        public void InitializeQueryObject(CancellationContext cancellationContext, RegistryContext registryContext, IScheduler scheduler)
        {
            InitializeQueryObject(cancellationContext, registryContext, scheduler, false);
        }

        /// <summary>
        /// Initializes the query object by loading registry objects, if necessary.
        /// </summary>
        /// <param name="registryContext"></param>
        /// <param name="scheduler"></param>
        /// <param name="forceReinitialize"></param>
        public virtual void InitializeQueryObject(CancellationContext cancellationContext, RegistryContext registryContext, IScheduler scheduler, bool forceReinitialize)
        {
            if (cancellationContext != null)
            {
                ((ICancelableTask)this).CancellationContext = cancellationContext;
            }

            if (registryContext != null)
            {
                registryContext.EnsureContextEntitiesLoaded();
            }

            lock (syncRoot)
            {
                if (registryContext != null)
                {
                    UpdateContext(registryContext);

                    switch (executionMode)
                    {
                        case ExecutionMode.SingleServer:
                            break;
                        case ExecutionMode.Graywulf:
                            LoadAssignedServerInstance(forceReinitialize);
                            LoadDatasets(forceReinitialize);
                            LoadSystemDatabaseInstance(temporaryDatabaseInstanceReference, (GraywulfDataset)temporaryDataset, forceReinitialize);
                            LoadSystemDatabaseInstance(codeDatabaseInstanceReference, (GraywulfDataset)codeDataset, forceReinitialize);

                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                if (scheduler != null)
                {
                    this.scheduler = scheduler;
                }

                // TODO: try to take these out from lock
                ParseQuery(forceReinitialize);
                ResolveNames(forceReinitialize);
            }
        }

        /// <summary>
        /// Parses the query
        /// </summary>
        protected void ParseQuery(bool forceReinitialize)
        {
            // Reparse only if needed
            if (queryDetails.ParsingTree == null || forceReinitialize)
            {
                var parser = queryFactory.Value.CreateParser();
                queryDetails.ParsingTree = parser.Execute<StatementBlock>(queryString);
            }
        }

        /// <summary>
        /// Interprets the parsed query
        /// </summary>
        protected bool ResolveNames(bool forceReinitialize)
        {
            if (!queryDetails.IsResolved || forceReinitialize)
            {
                // --- Execute name resolution
                var nr = CreateNameResolver(forceReinitialize);
                nr.Execute(queryDetails);

                OnNamesResolved(forceReinitialize);

                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void OnNamesResolved(bool forceReinitialize)
        {
        }

        /// <summary>
        /// Returns a new name resolver to be used with the parsed query string.
        /// </summary>
        /// <param name="forceReinitialize"></param>
        /// <returns></returns>
        protected Sql.NameResolution.SqlNameResolver CreateNameResolver(bool forceReinitialize)
        {
            LoadDatasets(forceReinitialize);

            var nr = QueryFactory.CreateNameResolver();
            nr.SchemaManager = GetSchemaManager();

            nr.DefaultTableDatasetName = DefaultSourceDataset.Name;
            nr.DefaultOutputDatasetName = DefaultOutputDataset.Name;
            nr.DefaultFunctionDatasetName = CodeDataset.Name;

            return nr;
        }

        public virtual void Validate()
        {
        }

        #endregion
        #region Server assignment logic

        /// <summary>
        /// Returns local datasets that are required to execute the query.
        /// </summary>
        protected Dictionary<string, GraywulfDataset> FindRequiredGraywulfDatasets()
        {
            var sc = GetSchemaManager();
            var dss = new Dictionary<string, GraywulfDataset>(SchemaManager.Comparer);

            // TODO: add support for multiple statements
            // TODO: move this to name resolver and add support for in-query data sets

            foreach (var tr in queryDetails.ParsingTree.FindDescendantRecursive<QueryExpression>().EnumerateSourceTableReferences(true))
            {
                if (tr.Type != TableReferenceType.UserDefinedFunction &&
                    tr.Type != TableReferenceType.Subquery &&
                    tr.Type != TableReferenceType.CommonTable &&
                    !tr.IsComputed)
                {
                    if (!dss.ContainsKey(tr.DatasetName))
                    {
                        var ds = sc.Datasets[tr.DatasetName];
                        if (ds is GraywulfDataset)
                        {
                            dss.Add(tr.DatasetName, (GraywulfDataset)ds);
                        }
                    }
                }
            }

            return dss;
        }

        protected Dictionary<string, GraywulfDataset> FindMirroredGraywulfDatasets()
        {
            var dss = new Dictionary<string, GraywulfDataset>(SchemaManager.Comparer);

            foreach (var ds in FindRequiredGraywulfDatasets())
            {
                if (!ds.Value.IsSpecificInstanceRequired)
                {
                    dss.Add(ds.Key, (GraywulfDataset)ds.Value);
                }
            }

            return dss;
        }

        protected Dictionary<string, GraywulfDataset> FindServerSpecificGraywulfDatasets()
        {
            var dss = new Dictionary<string, GraywulfDataset>(SchemaManager.Comparer);

            foreach (var ds in FindRequiredGraywulfDatasets())
            {
                if (ds.Value.IsSpecificInstanceRequired)
                {
                    dss.Add(ds.Key, ds.Value);
                }
            }

            return dss;
        }


        public void AssignServerInstance()
        {
            // Assign a server that will run the queries
            // Try to find a server that contains all required datasets. This is true right now for
            // SkyQuery where all databases are mirrored but will have to be updated later

            var mirroredDatasets = FindMirroredGraywulfDatasets().Values.Select(i => i.DatabaseDefinitionReference.Value).ToArray();
            var specificDatasets = FindServerSpecificGraywulfDatasets().Values.Select(i => i.DatabaseInstanceReference.Value).ToArray();

            ServerInstance serverInstance;

            if (mirroredDatasets.Length == 0)
            {
                // If no graywulf datasets are used, get a server from the scheduler
                // that has an instance of the temp database and assume that it is
                // configured correctly

                var dd = ((GraywulfDataset)TemporaryDataset).DatabaseVersionReference.Value.DatabaseDefinition;
                serverInstance = GetNextServerInstance(dd, Registry.Constants.TempDbName);
            }
            else
            {
                // Assign new server instance based on database availability
                serverInstance = GetNextServerInstance(mirroredDatasets, sourceDatabaseVersionName, null, specificDatasets);
            }

            assignedServerInstanceReference.Value = serverInstance;

            LoadSystemDatabaseInstance(temporaryDatabaseInstanceReference, (GraywulfDataset)temporaryDataset, true);
            LoadSystemDatabaseInstance(codeDatabaseInstanceReference, (GraywulfDataset)codeDataset, true);
        }

        #endregion
        #region Cluster registry functions

        /// <summary>
        /// Returnes a new registry context when in Graywulf execution mode.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="activityContext"></param>
        /// <param name="connectionMode"></param>
        /// <param name="transactionMode"></param>
        /// <returns></returns>
        public Jhu.Graywulf.Registry.RegistryContext CreateContext()
        {
            switch (executionMode)
            {
                case Query.ExecutionMode.SingleServer:
                    return null;
                case Query.ExecutionMode.Graywulf:
                    return Jhu.Graywulf.Registry.ContextManager.Instance.CreateReadOnlyContext();
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void UpdateContext(RegistryContext context)
        {
            this.registryContext = context;
        }

        protected void LoadAssignedServerInstance(bool forceReinitialize)
        {
            if (!assignedServerInstanceReference.IsEmpty && forceReinitialize)
            {
                assignedServerInstanceReference.Value.GetConnectionString();
            }
        }

        protected void LoadDatasets(bool forceReinitialize)
        {
            switch (ExecutionMode)
            {
                case Query.ExecutionMode.Graywulf:

                    // Initialize temporary database
                    if (temporaryDataset == null || forceReinitialize)
                    {
                        var tempds = new GraywulfDataset(RegistryContext)
                        {
                            Name = Registry.Constants.TempDbName,
                            IsOnLinkedServer = false,
                            IsMutable = true,
                        };
                        tempds.DatabaseVersionReference.Value = FederationReference.Value.TempDatabaseVersion;
                        tempds.CacheSchemaConnectionString();
                        temporaryDataset = tempds;
                    }

                    // Initialize code database
                    if (codeDataset == null || forceReinitialize)
                    {
                        var codeds = new GraywulfDataset(RegistryContext)
                        {
                            Name = Registry.Constants.CodeDbName,
                            IsOnLinkedServer = false,
                            IsMutable = false,
                        };
                        codeds.DatabaseVersionReference.Value = FederationReference.Value.CodeDatabaseVersion;
                        codeds.CacheSchemaConnectionString();
                        codeDataset = codeds;
                    }

                    break;

                case Query.ExecutionMode.SingleServer:

                    // Initialize temporary database
                    if (temporaryDataset == null || forceReinitialize)
                    {
                        // TODO: implement this if necessary
                    }

                    // Initialize code database
                    if (codeDataset == null || forceReinitialize)
                    {
                        // TODO: implement this if necessary
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Checks whether the given dataset is remote to the assigned server
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public bool IsRemoteDataset(DatasetBase ds)
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
        #region System database functions

        protected void LoadSystemDatabaseInstance(EntityReference<DatabaseInstance> databaseInstance, GraywulfDataset dataset, bool forceReinitialize)
        {
            if (!AssignedServerInstanceReference.IsEmpty && (databaseInstance.IsEmpty || forceReinitialize))
            {
                dataset.RegistryContext = RegistryContext;
                var dd = dataset.DatabaseVersionReference.Value.DatabaseDefinition;

                dd.LoadDatabaseInstances(false);
                foreach (var di in dd.DatabaseInstances.Values)
                {
                    di.RegistryContext = RegistryContext;
                }

                // Find database instance that is on the same machine
                try
                {
                    // TODO: only server instance and database definition is checked here, maybe database version would be better
                    databaseInstance.Value = dd.DatabaseInstances.Values.FirstOrDefault(ddi => ddi.ServerInstanceReference.Guid == AssignedServerInstance.Guid);
                    databaseInstance.Value.GetConnectionString();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        String.Format(
                            "Cannot find instance of system database: {0} ({1}) on server {2}/{3} ({4}).",
                            dataset.Name, dataset.DatabaseName,
                            AssignedServerInstance.Machine.Name, AssignedServerInstance.Name, AssignedServerInstance.GetCompositeName()),
                        ex); // TODO ***
                }
            }
            else if (AssignedServerInstanceReference.IsEmpty)
            {
                databaseInstance.Value = null;
            }
        }

        /// <summary>
        /// Returns a connection string to a system database on a well-known server assigned
        /// to a query partition
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected internal SqlConnectionStringBuilder GetSystemDatabaseConnectionStringOnAssignedServer(CommandTarget target)
        {
            SqlServerDataset ds;

            switch (target)
            {
                case CommandTarget.Code:
                    ds = (SqlServerDataset)CodeDataset;
                    break;
                case CommandTarget.Temp:
                    ds = (SqlServerDataset)TemporaryDataset;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new SqlConnectionStringBuilder(ds.ConnectionString);
        }

        public abstract Table GetTemporaryTable(string tablename);

        protected Table GetTemporaryTableInternal(string tablename)
        {
            var tempds = TemporaryDataset;

            return new Table()
            {
                Dataset = tempds,
                DatabaseName = tempds.DatabaseName,
                SchemaName = tempds.DefaultSchemaName,
                TableName = tablename,
            };
        }

        #endregion
        #region Scheduler functions

        protected ServerInstance GetNextServerInstance(DatabaseDefinition databaseDefinition, string databaseVersion)
        {
            return GetNextServerInstance(databaseDefinition, databaseVersion, null);
        }

        protected ServerInstance GetNextServerInstance(DatabaseDefinition databaseDefinition, string databaseVersion, string surrogateDatabaseVersion)
        {
            Guid siguid;

            // Try with requested database version
            siguid = scheduler.GetNextServerInstance(new Guid[] { databaseDefinition.Guid }, databaseVersion, null);

            // If not found, try with surrogate
            if (surrogateDatabaseVersion != null && siguid == Guid.Empty)
            {
                siguid = scheduler.GetNextServerInstance(new Guid[] { databaseDefinition.Guid }, surrogateDatabaseVersion, null);
            }

            if (siguid == Guid.Empty)
            {
                throw new Exception("No server found with requested database.");  // *** TODO
            }

            var ef = new EntityFactory(RegistryContext);
            var si = ef.LoadEntity<ServerInstance>(siguid);

            return si;
        }

        protected ServerInstance GetNextServerInstance(IEnumerable<DatabaseDefinition> databaseDefinitions, string databaseVersion, string surrogateDatabaseVersion, IEnumerable<DatabaseInstance> specificDatabaseInstances)
        {
            Guid[] dds, dis;
            Guid siguid;

            if (databaseDefinitions != null)
            {
                dds = databaseDefinitions.Select(i => i.Guid).ToArray();
            }
            else
            {
                dds = null;
            }

            if (specificDatabaseInstances != null)
            {
                dis = specificDatabaseInstances.Select(i => i.Guid).ToArray();
            }
            else
            {
                dis = null;
            }

            // Try with requested database version
            siguid = scheduler.GetNextServerInstance(dds, databaseVersion, dis);

            // If not found, try with surrogate
            // If not found, try with surrogate
            if (surrogateDatabaseVersion != null && siguid == Guid.Empty)
            {
                siguid = scheduler.GetNextServerInstance(dds, surrogateDatabaseVersion, dis);
            }

            if (siguid == Guid.Empty)
            {
                throw new Exception("No server found with requested database.");  // *** TODO
            }

            var ef = new EntityFactory(RegistryContext);
            var si = ef.LoadEntity<ServerInstance>(siguid);

            return si;
        }

        protected ServerInstance[] GetAvailableServerInstances(IEnumerable<DatabaseDefinition> databaseDefinitions, string databaseVersion, string surrogateDatabaseVersion, IEnumerable<DatabaseInstance> specificDatabaseInstances)
        {
            Guid[] dds, dis;
            Guid[] siguid;

            if (databaseDefinitions != null)
            {
                dds = databaseDefinitions.Select(i => i.Guid).ToArray();
            }
            else
            {
                dds = null;
            }

            if (specificDatabaseInstances != null)
            {
                dis = specificDatabaseInstances.Select(i => i.Guid).ToArray();
            }
            else
            {
                dis = null;
            }

            // Try with requested database version
            siguid = scheduler.GetServerInstances(dds, databaseVersion, dis);

            // If not found, try with surrogate
            // If not found, try with surrogate
            if (surrogateDatabaseVersion != null && (siguid == null || siguid.Length == 0))
            {
                siguid = scheduler.GetServerInstances(dds, surrogateDatabaseVersion, dis);

            }

            if (siguid == null || siguid.Length == 0)
            {
                throw new Exception("No server found with requested database.");  // *** TODO
            }

            var ef = new EntityFactory(RegistryContext);
            var si = new ServerInstance[siguid.Length];
            for (int i = 0; i < siguid.Length; i++)
            {
                si[i] = ef.LoadEntity<ServerInstance>(siguid[i]);
            }

            return si;
        }

        protected DatabaseInstance[] GetAvailableDatabaseInstances(DatabaseDefinition databaseDefinition, string databaseVersion)
        {
            return GetAvailableDatabaseInstances(databaseDefinition, databaseVersion, null);
        }

        protected DatabaseInstance[] GetAvailableDatabaseInstances(DatabaseDefinition databaseDefinition, string databaseVersion, string surrogateDatabaseVersion)
        {
            Guid[] diguid;

            // Try with requested database version
            diguid = scheduler.GetDatabaseInstances(databaseDefinition.Guid, databaseVersion);

            // If not found, try with surrogate
            if (surrogateDatabaseVersion != null && (diguid == null || diguid.Length == 0))
            {
                diguid = scheduler.GetDatabaseInstances(databaseDefinition.Guid, surrogateDatabaseVersion);
            }

            if (diguid == null || diguid.Length == 0)
            {
                throw new Exception("No instance of the requested database found.");  // *** TODO
            }

            var ef = new EntityFactory(RegistryContext);
            var di = new DatabaseInstance[diguid.Length];
            for (int i = 0; i < diguid.Length; i++)
            {
                di[i] = ef.LoadEntity<DatabaseInstance>(diguid[i]);
            }

            return di;
        }

        public DatabaseInstance[] GetAvailableDatabaseInstances(ServerInstance serverInstance, DatabaseDefinition databaseDefinition, string databaseVersion)
        {
            return GetAvailableDatabaseInstances(serverInstance, databaseDefinition, null);
        }

        public DatabaseInstance[] GetAvailableDatabaseInstances(ServerInstance serverInstance, DatabaseDefinition databaseDefinition, string databaseVersion, string surrogateDatabaseVersion)
        {
            Guid[] diguid;

            // Try with requested database version
            diguid = scheduler.GetDatabaseInstances(serverInstance.Guid, databaseDefinition.Guid, databaseVersion);

            // If not found, try with surrogate
            if (surrogateDatabaseVersion != null && (diguid == null || diguid.Length == 0))
            {
                diguid = scheduler.GetDatabaseInstances(databaseDefinition.Guid, surrogateDatabaseVersion);
            }

            if (diguid == null || diguid.Length == 0)
            {
                throw new Exception("No instance of the requested database found.");  // *** TODO
            }

            var ef = new EntityFactory(RegistryContext);
            var di = new DatabaseInstance[diguid.Length];
            for (int i = 0; i < diguid.Length; i++)
            {
                di[i] = ef.LoadEntity<DatabaseInstance>(diguid[i]);
            }

            return di;
        }

        #endregion
        #region Name substitution

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
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return new Schema.SqlServer.SqlServerSchemaManager();
                case ExecutionMode.Graywulf:
                    return GraywulfSchemaManager.Create(new FederationContext(registryContext, null));
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns a schema manager, either the cached one, either a newly
        /// created one.
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        public virtual SchemaManager GetSchemaManager()
        {
            var sc = CreateSchemaManager();

            sc.Datasets[Jhu.Graywulf.Registry.Constants.CodeDbName] = CodeDataset;
            sc.Datasets[Jhu.Graywulf.Registry.Constants.TempDbName] = TemporaryDataset;

            // Add custom dataset defined by code
            foreach (var ds in customDatasets)
            {
                // *** TODO: check this
                // is this where mydb is added?
                sc.Datasets[ds.Name] = ds;
            }

            return sc;
        }

        #endregion
        #region Clean-up functions

        protected void DropTemporaryTables(bool suppressErrors)
        {
            foreach (var table in temporaryTables.Values)
            {
                // This function is called in the cancel branch of certain workflows
                // where it's not supposed to fail in any circumstances.

                try
                {
                    table.Drop();
                }
                catch (Exception)
                {
                    if (!suppressErrors)
                    {
                        throw;
                    }
                }
            }

            temporaryTables.Clear();
        }

        protected void DropTemporaryViews(bool suppressErrors)
        {
            foreach (var view in temporaryViews.Values)
            {
                // This function is called in the cancel branch of certain workflows
                // where it's not supposed to fail in any circumstances.

                try
                {
                    view.Drop();
                }
                catch (Exception)
                {
                    if (!suppressErrors)
                    {
                        throw;
                    }
                }
            }

            temporaryViews.Clear();
        }

        #endregion
        #region Generic query execution functions

        private async Task<SqlConnection> OpenConnectionForCommandAsync(SqlCommand cmd, string connectionString)
        {
            var cn = new SqlConnection(connectionString);
            await cn.OpenAsync(CancellationContext.Token);

            cmd.Connection = cn;
            cmd.CommandTimeout = queryTimeout;

            return cn;
        }

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="cmd"></param>
        protected async Task<int> ExecuteSqlAsync(SqlCommand cmd, string connectionString)
        {
            // TODO: dispatch exceptions

            using (var cn = await OpenConnectionForCommandAsync(cmd, connectionString))
            {
                DumpSqlCommand(cmd);
                return await cmd.ExecuteNonQueryAsync(CancellationContext.Token);
            }
        }

        /// <summary>
        /// Executes a long SQL command in cancelable mode.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected async Task<object> ExecuteSqlScalarAsync(SqlCommand cmd, string connectionString)
        {
            // TODO: dispatch exceptions

            using (var cn = await OpenConnectionForCommandAsync(cmd, connectionString))
            {
                DumpSqlCommand(cmd);
                return await cmd.ExecuteScalarAsync(CancellationContext.Token);
            }
        }

        protected async Task<int> ExecuteSqlReaderAsync(SqlCommand cmd, string connectionString, AsyncReaderAction asyncAction)
        {
            // TODO: dispatch exceptions

            using (var cn = await OpenConnectionForCommandAsync(cmd, connectionString))
            {
                DumpSqlCommand(cmd);

                using (var dr = await cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationContext.Token))
                {
                    await asyncAction(dr, CancellationContext.Token);
                    return dr.RecordsAffected;
                }
            }
        }

        protected internal virtual string GetDumpFileName(CommandTarget target)
        {
            var server = GetSystemDatabaseConnectionStringOnAssignedServer(target).DataSource;
            server = server.Replace('\\', '_').Replace('/', '_');
            return String.Format("dump_{0}.sql", server);
        }

        protected void DumpSqlCommand(string sql, CommandTarget target)
        {
            if (dumpSql)
            {
                string filename = GetDumpFileName(target);
                var sw = new StringWriter();

                // Time stamp
                sw.WriteLine("-- {0}\r\n", DateTime.Now);
                sw.WriteLine(sql);
                sw.WriteLine("GO");
                sw.WriteLine();

                lock (syncRoot)
                {
                    File.AppendAllText(filename, sw.ToString());
                }
            }
        }

        private void DumpSqlCommand(SqlCommand cmd)
        {
            if (dumpSql)
            {
                var filename = GetDumpFileName(CommandTarget.Temp);
                var sw = new StringWriter();

                // Time stamp
                sw.WriteLine("-- {0}\r\n", DateTime.Now);

                // Database name
                var csb = new SqlConnectionStringBuilder(cmd.Connection.ConnectionString);

                if (!String.IsNullOrWhiteSpace(csb.InitialCatalog))
                {
                    sw.WriteLine("USE [{0}]", csb.InitialCatalog);
                    sw.WriteLine("GO");
                    sw.WriteLine();
                }

                // Command parameters
                foreach (SqlParameter par in cmd.Parameters)
                {
                    sw.WriteLine(String.Format("DECLARE {0} {1} = {2}",
                        par.ParameterName,
                        par.SqlDbType.ToString(),
                        par.Value.ToString()));
                }

                sw.WriteLine(cmd.CommandText);
                sw.WriteLine("GO");

                lock (syncRoot)
                {
                    File.AppendAllText(filename, sw.ToString());
                }
            }
        }

        #endregion
        #region Execute SQL on specific data sets

        protected async Task<int> ExecuteSqlOnDatasetAsync(SqlCommand cmd, DatasetBase dataset)
        {
            return await ExecuteSqlAsync(cmd, dataset.ConnectionString);
        }

        protected async Task<int> ExecuteSqlOnAssignedServerAsync(SqlCommand cmd, CommandTarget target)
        {
            var csb = GetSystemDatabaseConnectionStringOnAssignedServer(target);
            return await ExecuteSqlAsync(cmd, csb.ConnectionString);
        }

        protected async Task<object> ExecuteSqlOnAssignedServerScalarAsync(SqlCommand cmd, CommandTarget target)
        {
            var csb = GetSystemDatabaseConnectionStringOnAssignedServer(target);
            return await ExecuteSqlScalarAsync(cmd, csb.ConnectionString);
        }

        protected async Task<int> ExecuteSqlOnAssignedServerReaderAsync(SqlCommand cmd, CommandTarget target, AsyncReaderAction asyncAction)
        {
            var csb = GetSystemDatabaseConnectionStringOnAssignedServer(target);
            return await ExecuteSqlReaderAsync(cmd, csb.ConnectionString, asyncAction);
        }

        #endregion
        #region Cancelable task execution

        protected override Task OnExecuteAsync()
        {
            // Required by cancelable interface
            throw new NotImplementedException();
        }

        #endregion
        #region Specialized SQL manipulation function

        /// <summary>
        /// Creates and initializes a remote or local table copy task
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="local"></param>
        /// <returns></returns>
        protected ServiceModel.ServiceProxy<ICopyTable> CreateTableCopyTask(SourceTableQuery source, DestinationTable destination, bool local)
        {
            var desthost = GetHostnameFromSqlConnectionString(destination.Dataset.ConnectionString);

            ServiceModel.ServiceProxy<ICopyTable> qi;

            if (local)
            {
                qi = new ServiceModel.ServiceProxy<ICopyTable>(new CopyTable(CancellationContext));
            }
            else
            {
                qi = RemoteServiceHelper.CreateObject<ICopyTable>(CancellationContext, desthost, true);
            }

            qi.Value.Source = source;
            qi.Value.Destination = destination;

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
                host = csb.DataSource.Substring(0, i);
            }
            else
            {
                host = csb.DataSource;
            }

            try
            {
                // Do a reverse-lookup to get host name
                host = System.Net.Dns.GetHostEntry(host).HostName;
            }
            catch (Exception)
            {

            }

            return host;
        }

        #endregion
    }
}
