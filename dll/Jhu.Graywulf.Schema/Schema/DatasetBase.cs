using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema
{
    public interface IDatasetSafe
    {
        string Name { get; }
    }

    /// <summary>
    /// Implements basic functionality to reflect datasets
    /// </summary>
    /// <remarks>
    /// This abstract class has to be overloaded in order to implement
    /// server product specific reflection logic
    /// </remarks>
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract partial class DatasetBase : ICloneable, ICacheable, IDatasetSafe
    {
        #region Property storage member variables

        [NonSerialized]
        private long cachedVersion;

        [NonSerialized]
        private bool isCacheable;

        [NonSerialized]
        private bool isMutable;

        [NonSerialized]
        private string name;

        [NonSerialized]
        private string defaultSchemaName;

        [NonSerialized]
        private string connectionString;

        [NonSerialized]
        private DatabaseObjectCollection<Table> tables;

        [NonSerialized]
        private DatabaseObjectCollection<View> views;

        [NonSerialized]
        private DatabaseObjectCollection<TableValuedFunction> tableValuedFunctions;

        [NonSerialized]
        private DatabaseObjectCollection<ScalarFunction> scalarFunctions;

        [NonSerialized]
        private DatabaseObjectCollection<StoredProcedure> storedProcedures;

        [NonSerialized]
        private LazyProperty<DatasetStatistics> statistics;

        [NonSerialized]
        private LazyProperty<DatasetMetadata> metadata;

        #endregion
        #region Properties

        /// <summary>
        /// Reserved for caching logic
        /// </summary>
        [IgnoreDataMember]
        public long CachedVersion
        {
            get { return cachedVersion; }
        }

        /// <summary>
        /// Gets or sets if description of this dataset is to be cached or
        /// has to be loaded every time the dataset is reflected.
        /// </summary>
        [DataMember]
        public bool IsCacheable
        {
            get { return isCacheable; }
            set { isCacheable = value; }
        }

        /// <summary>
        /// Reserved for future use
        /// </summary>
        [DataMember]
        public bool IsMutable
        {
            get { return isMutable; }
            set { isMutable = value; }
        }

        /// <summary>
        /// Gets or sets the name of the dataset
        /// </summary>
        /// <remarks>
        /// Dataset name is the part that appears before : in queries
        /// </remarks>
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the default schema name.
        /// </summary>
        /// <remarks>
        /// In case of no schema name is specified in queries referencing
        /// this data set, the default schema name will be used.
        /// The default value is 'dbo'.
        /// </remarks>
        [DataMember]
        public string DefaultSchemaName
        {
            get { return defaultSchemaName; }
            set { defaultSchemaName = value; }
        }

        /// <summary>
        /// Gets the provider invariant name of dataset
        /// </summary>
        [IgnoreDataMember]
        public abstract string ProviderName { get; }

        /// <summary>
        /// Gets or sets the database name associated with this dataset.
        /// </summary>
        [IgnoreDataMember]
        public abstract string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the connection string of the data set.
        /// </summary>
        [DataMember]
        public virtual string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// Gets the collection of tables
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectCollection<Table> Tables
        {
            get { return tables; }
        }

        /// <summary>
        /// Gets the collection of views
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectCollection<View> Views
        {
            get { return views; }
        }

        /// <summary>
        /// Gets the collection of table-values functions
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectCollection<TableValuedFunction> TableValuedFunctions
        {
            get { return tableValuedFunctions; }
        }

        /// <summary>
        /// Gets the collection of scalar functions
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectCollection<ScalarFunction> ScalarFunctions
        {
            get { return scalarFunctions; }
        }

        /// <summary>
        /// Gets the collection of stored procedures
        /// </summary>
        [IgnoreDataMember]
        public DatabaseObjectCollection<StoredProcedure> StoredProcedures
        {
            get { return storedProcedures; }
        }

        [IgnoreDataMember]
        public DatasetStatistics Statistics
        {
            get { return statistics.Value; }
        }

        [IgnoreDataMember]
        public DatasetMetadata Metadata
        {
            get { return metadata.Value; }
            set { metadata.Value = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Creates a dataset object and initializes member
        /// variables.
        /// </summary>
        protected DatasetBase()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Creates a copy of the dataset object
        /// </summary>
        /// <param name="old"></param>
        protected DatasetBase(DatasetBase old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variable to their default values
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.cachedVersion = DateTime.Now.Ticks;

            this.isCacheable = false;
            this.name = String.Empty;
            this.defaultSchemaName = String.Empty;

            this.connectionString = null;

            this.tables = new DatabaseObjectCollection<Table>(this);
            this.views = new DatabaseObjectCollection<View>(this);
            this.tableValuedFunctions = new DatabaseObjectCollection<TableValuedFunction>(this);
            this.scalarFunctions = new DatabaseObjectCollection<ScalarFunction>(this);
            this.storedProcedures = new DatabaseObjectCollection<StoredProcedure>(this);

            this.statistics = new LazyProperty<DatasetStatistics>(LoadDatasetStatistics);
            this.metadata = new LazyProperty<DatasetMetadata>(LoadDatasetMetadata);

            InitializeEventHandlers();
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(DatasetBase old)
        {
            this.cachedVersion = DateTime.Now.Ticks;

            this.isCacheable = old.isCacheable;
            this.name = old.name;
            this.defaultSchemaName = old.defaultSchemaName;

            this.connectionString = old.connectionString;

            // No deep copy here
            this.tables = new DatabaseObjectCollection<Table>(this);
            this.views = new DatabaseObjectCollection<View>(this);
            this.tableValuedFunctions = new DatabaseObjectCollection<TableValuedFunction>(this);
            this.scalarFunctions = new DatabaseObjectCollection<ScalarFunction>(this);
            this.storedProcedures = new DatabaseObjectCollection<StoredProcedure>(this);

            this.statistics = new LazyProperty<DatasetStatistics>(LoadDatasetStatistics);
            this.metadata = new LazyProperty<DatasetMetadata>(LoadDatasetMetadata);

            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            tables.ItemLoading += OnObjectLoading<Table>;
            tables.AllObjectsLoading += OnAllObjectLoading<Table>;

            views.ItemLoading += OnObjectLoading<View>;
            views.AllObjectsLoading += OnAllObjectLoading<View>;

            tableValuedFunctions.ItemLoading += OnObjectLoading<TableValuedFunction>;
            tableValuedFunctions.AllObjectsLoading += OnAllObjectLoading<TableValuedFunction>;

            scalarFunctions.ItemLoading += OnObjectLoading<ScalarFunction>;
            scalarFunctions.AllObjectsLoading += OnAllObjectLoading<ScalarFunction>;

            storedProcedures.ItemLoading += OnObjectLoading<StoredProcedure>;
            storedProcedures.AllObjectsLoading += OnAllObjectLoading<StoredProcedure>;
        }

        /// <summary>
        /// When overloaded in derived classes, creates a copy of the dataset.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Fully resolved names and keys

        /// <summary>
        /// Returns the indentifier quoted according to the server type.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        protected abstract string QuoteIdentifier(string identifier);

        /// <summary>
        /// When overloaded in derived classes, returns the fully resolved name of the dataset.
        /// </summary>
        /// <remarks>
        /// The fully resolved name uniquely identifies the dataset (as a database name)
        /// for the underlying server. It uses the same quoting format that what the server
        /// expects.
        /// </remarks>
        protected virtual string GetFullyResolvedName()
        {
            return QuoteIdentifier(DatabaseName);
        }

        /// <summary>
        /// When overloaded in derived classes, returns the fully resolved name of an object.
        /// </summary>
        /// <remarks>
        /// The fully resolved name uniquely identifies the object
        /// for the underlying server. It uses the same quoting format that what the server
        /// expects.
        /// </remarks>
        public abstract string GetObjectFullyResolvedName(DatabaseObject databaseObject);

        /// <summary>
        /// When overriden in derived classes, returns a unique key of a
        /// database object that can be used in web urls, etc.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="datasetName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public virtual string GetObjectUniqueKey(DatabaseObjectType objectType, string datasetName, string databaseName, string schemaName, string objectName)
        {
            return String.Format(
                "{0}|{1}|{2}|{3}|{4}",
                objectType,
                datasetName,
                databaseName,
                schemaName,
                objectName);
        }

        /// <summary>
        /// Returns a unique key for a database object that can be used
        /// in web urls, etc.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        public string GetObjectUniqueKey(DatabaseObject databaseObject)
        {
            return GetObjectUniqueKey(
                databaseObject.ObjectType,
                this.Name,
                databaseObject.DatabaseName,
                databaseObject.SchemaName,
                databaseObject.ObjectName);
        }

        /// <summary>
        /// Takes an object unique name and sets the objects appropriate properties
        /// to have this unique name.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <param name="objectKey"></param>
        public void GetNamePartsFromObjectUniqueKey(DatabaseObject databaseObject, string objectKey)
        {
            // TODO: compare object types

            string[] parts = objectKey.Split('|');
            databaseObject.DatabaseName = parts[2];
            databaseObject.SchemaName = parts[3];
            databaseObject.ObjectName = parts[4];
        }

        #endregion
        #region Type conversion function

        protected abstract DataType GetTypeFromProviderSpecificName(string name);

        protected DataType GetTypeFromProviderSpecificName(string name, int size, byte scale, byte precision, bool nullable)
        {
            var t = GetTypeFromProviderSpecificName(name);

            if (t.HasLength)
            {
                t.Length = size;
            }

            t.Scale = scale;
            t.Precision = precision;
            t.IsNullable = nullable;

            return t;
        }

        #endregion
        #region Object loading functions

        public DatabaseObject GetObject(string databaseName, string schemaName, string objectName)
        {
            if (tables.ContainsKey(databaseName, schemaName, objectName))
            {
                return tables[databaseName, schemaName, objectName];
            }
            else if (views.ContainsKey(databaseName, schemaName, objectName))
            {
                return views[databaseName, schemaName, objectName];
            }
            else if (scalarFunctions.ContainsKey(databaseName, schemaName, objectName))
            {
                return scalarFunctions[databaseName, schemaName, objectName];
            }
            else if (storedProcedures.ContainsKey(databaseName, schemaName, objectName))
            {
                return storedProcedures[databaseName, schemaName, objectName];
            }
            else if (tableValuedFunctions.ContainsKey(databaseName, schemaName, objectName))
            {
                return tableValuedFunctions[databaseName, schemaName, objectName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Touches the object so it won't get dropped from the cache.
        /// </summary>
        public void Touch()
        {
            cachedVersion = DateTime.Now.Ticks;
        }

        private void OnObjectLoading<T>(object sender, LazyItemLoadingEventArgs<string, T> e)
            where T : DatabaseObject, new()
        {
            T obj = new T();
            obj.Dataset = this;
            GetNamePartsFromObjectUniqueKey(obj, e.Key);

            try
            {
                LoadObject<T>(obj);

                e.Value = obj;
                e.Key = GetObjectUniqueKey(obj);
                e.IsFound = true;
            }
            catch (SchemaException)
            {
                e.IsFound = false;
            }
        }

        private void OnAllObjectLoading<T>(object sender, AllObjectsLoadingEventArgs<string, T> e)
            where T : DatabaseObject, new()
        {
            e.Collection = LoadAllObjects<T>(e.DatabaseName);
        }

        /// <summary>
        /// When overloaded in derived classes, loads the database object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseObject"></param>
        protected abstract void LoadObject<T>(T databaseObject)
            where T : DatabaseObject, new();

        internal abstract bool IsObjectExisting(DatabaseObject databaseObject);

        /// <summary>
        /// When overloaded in derived classes, loads all objects of a certain kind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>(string databaseName)
            where T : DatabaseObject, new();

        /// <summary>
        /// When overloaded in derived classes, loads all columns of a database object
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal abstract IEnumerable<KeyValuePair<string, Column>> LoadColumns(DatabaseObject databaseObject);

        /// <summary>
        /// When overloaded in derived classes, loads all indexes belonging to a table or view
        /// </summary>
        /// <param name="tableOrView"></param>
        /// <returns></returns>
        internal abstract IEnumerable<KeyValuePair<string, Index>> LoadIndexes(DatabaseObject databaseObject);

        /// <summary>
        /// When overloaded in derived classes, loads all columns covered by and index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal abstract IEnumerable<KeyValuePair<string, IndexColumn>> LoadIndexColumns(Index index);

        /// <summary>
        /// When overloaded in derived classes, loads parameters of a function or stored procedure
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal abstract IEnumerable<KeyValuePair<string, Parameter>> LoadParameters(DatabaseObject databaseObject);

        #endregion
        #region Metadata functions

        protected abstract DatasetMetadata LoadDatasetMetadata();

        internal abstract DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject);

        internal abstract void SaveDatabaseObjectMetadata(DatabaseObject databaseObject);

        internal abstract void DropDatabaseObjectMetadata(DatabaseObject databaseObject);

        /// <summary>
        /// Loads metadata of a variable (parameter, column, return value, etc.).
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        /// <remarks>Dataset is implemented to load all variable metadata of an
        /// object in a batch to reduce roundtrips between the client and the
        /// database server.</remarks>
        internal void LoadAllVariableMetadata(DatabaseObject databaseObject)
        {
            if (databaseObject is IColumns)
            {
                LoadAllColumnMetadata(databaseObject);
            }

            if (databaseObject is IParameters)
            {
                LoadAllParameterMetadata(databaseObject);
            }
        }

        protected abstract void LoadAllColumnMetadata(DatabaseObject databaseObject);

        protected abstract void LoadAllParameterMetadata(DatabaseObject databaseObject);

        internal abstract void SaveAllVariableMetadata(DatabaseObject databaseObject);

        internal abstract void DropAllVariableMetadata(DatabaseObject databaseObject);

        #endregion
        #region Statistics function

        protected abstract DatasetStatistics LoadDatasetStatistics();

        internal abstract TableStatistics LoadTableStatistics(TableOrView tableOrView);

        #endregion
        #region Object modification functions

        /// <summary>
        /// When overloaded in derived classes, renames an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="newName"></param>
        /// <remarks>
        /// Only works with mutable datasets, ie. myDBs
        /// </remarks>
        internal abstract void RenameObject(DatabaseObject obj, string name);

        internal abstract void CreateTable(Table table);

        /// <summary>
        /// When overloaded in derived classes, drops an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <remarks>
        /// Only works with mutable datasets, ie. myDBs
        /// </remarks>
        internal abstract void DropObject(DatabaseObject obj);

        internal abstract void TruncateTable(Table table);

        #endregion

        protected void ThrowInvalidObjectNameException(DatabaseObject databaseObject)
        {
            string message;

            switch (Constants.DatabaseObjectTypes[databaseObject.GetType()])
            {
                case DatabaseObjectType.Table:
                    message = ExceptionMessages.InvalidTableName;
                    break;
                case DatabaseObjectType.View:
                    message = ExceptionMessages.InvalidViewName;
                    break;
                case DatabaseObjectType.TableValuedFunction:
                    message = ExceptionMessages.InvalidTableValuedFunctionName;
                    break;
                case DatabaseObjectType.ScalarFunction:
                    message = ExceptionMessages.InvalidScalarFunctionName;
                    break;
                case DatabaseObjectType.StoredProcedure:
                    message = ExceptionMessages.InvalidStoredProcedureName;
                    break;
                default:
                    throw new NotImplementedException();
            }

            throw new SchemaException(String.Format(message, databaseObject.ToString()));
        }

        public abstract string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist);
    }
}
