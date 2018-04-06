using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.Schema
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
    public abstract partial class DatasetBase : ICloneable, IDatasetSafe, IMetadata
    {
        #region Property storage member variables

        [NonSerialized]
        private bool isCacheable;

        [NonSerialized]
        private bool isMutable;

        [NonSerialized]
        private bool isRestrictedSchema;

        [NonSerialized]
        private string name;

        [NonSerialized]
        private string defaultSchemaName;

        [NonSerialized]
        private string connectionString;

        [NonSerialized]
        private bool failOnError;

        [NonSerialized]
        private bool captureError;

        [NonSerialized]
        private bool isInError;

        [NonSerialized]
        private Exception lastException;

        [NonSerialized]
        private DatabaseObjectCollection<DataType> userDefinedTypes;

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
        /// Gets or sets whether the dataset is mutable by the user.
        /// </summary>
        /// <remarks>
        /// TODO: will need to change behavior once schema permissions are
        /// implemented
        /// </remarks>
        [DataMember]
        public bool IsMutable
        {
            get { return isMutable; }
            set { isMutable = value; }
        }

        /// <summary>
        /// Gets or sets whether the dataset is locked to a single schema
        /// and no object from other schemas are accessible. This is used when
        /// the database is mutable but shared among users.
        /// </summary>
        /// <remarks>
        /// TODO: Behavior will need to be updated once schema permissions are
        /// implemented.
        /// </remarks>
        [DataMember]
        public bool IsRestrictedSchema
        {
            get { return isRestrictedSchema; }
            set { isRestrictedSchema = value; }
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

        [IgnoreDataMember]
        public bool FailOnError
        {
            get { return failOnError; }
            set { failOnError = value; }
        }

        [IgnoreDataMember]
        public bool CaptureError
        {
            get { return captureError; }
            set { captureError = value; }
        }

        [IgnoreDataMember]
        public bool IsInError
        {
            get { return isInError; }
            set { isInError = value; }
        }

        [IgnoreDataMember]
        public Exception LastException
        {
            get { return lastException; }
            set { lastException = value; }
        }

        [IgnoreDataMember]
        public DatabaseObjectCollection<DataType> UserDefinedTypes
        {
            get { return userDefinedTypes; }
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

        [IgnoreDataMember]
        Metadata IMetadata.Metadata
        {
            get { return metadata.Value; }
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
            this.isCacheable = false;
            this.isMutable = false;
            this.isRestrictedSchema = false;
            this.name = String.Empty;
            this.defaultSchemaName = String.Empty;

            this.connectionString = null;

            this.failOnError = true;
            this.captureError = false;
            this.isInError = false;
            this.lastException = null;

            this.userDefinedTypes = new DatabaseObjectCollection<DataType>(this);
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
            this.isCacheable = old.isCacheable;
            this.isMutable = old.isMutable;
            this.isRestrictedSchema = old.isRestrictedSchema;
            this.name = old.name;
            this.defaultSchemaName = old.defaultSchemaName;

            this.connectionString = old.connectionString;

            this.failOnError = old.failOnError;
            this.captureError = old.captureError;
            this.isInError = old.IsInError;
            this.lastException = old.lastException;

            // No deep copy here
            this.userDefinedTypes = new DatabaseObjectCollection<DataType>(this);
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
            userDefinedTypes.ItemLoading += OnObjectLoading<DataType>;
            userDefinedTypes.AllItemsLoading += OnAllObjectLoading<DataType>;

            tables.ItemLoading += OnObjectLoading<Table>;
            tables.AllItemsLoading += OnAllObjectLoading<Table>;

            views.ItemLoading += OnObjectLoading<View>;
            views.AllItemsLoading += OnAllObjectLoading<View>;

            tableValuedFunctions.ItemLoading += OnObjectLoading<TableValuedFunction>;
            tableValuedFunctions.AllItemsLoading += OnAllObjectLoading<TableValuedFunction>;

            scalarFunctions.ItemLoading += OnObjectLoading<ScalarFunction>;
            scalarFunctions.AllItemsLoading += OnAllObjectLoading<ScalarFunction>;

            storedProcedures.ItemLoading += OnObjectLoading<StoredProcedure>;
            storedProcedures.AllItemsLoading += OnAllObjectLoading<StoredProcedure>;
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
        public abstract string QuoteIdentifier(string identifier);

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
                databaseName ?? DatabaseName,
                schemaName ?? DefaultSchemaName,
                objectName);
        }

        protected virtual void SplitObjectUniqueKey(string objectKey, out DatabaseObjectType objectType, out string datasetName, out string databaseName, out string schemaName, out string objectName)
        {
            string[] parts = objectKey.Split('|');

            objectType = (DatabaseObjectType)Enum.Parse(typeof(DatabaseObjectType), parts[0]);
            datasetName = parts[1];
            databaseName = parts[2];
            schemaName = parts[3];
            objectName = parts[4];
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
        #region Object loading functions

        public DatabaseObject GetObject(string objectKey)
        {
            DatabaseObjectType objectType;
            string datasetName, databaseName, schemaName, objectName;
            SplitObjectUniqueKey(objectKey, out objectType, out datasetName, out databaseName, out schemaName, out objectName);

            switch (Constants.SimpleDatabaseObjectTypes[objectType])
            {
                case DatabaseObjectType.DataType:
                    return userDefinedTypes[objectKey];
                case DatabaseObjectType.Table:
                    return tables[objectKey];
                case DatabaseObjectType.View:
                    return views[objectKey];
                case DatabaseObjectType.TableValuedFunction:
                    return tableValuedFunctions[objectKey];
                case DatabaseObjectType.ScalarFunction:
                    return scalarFunctions[objectKey];
                case DatabaseObjectType.StoredProcedure:
                    return storedProcedures[objectKey];
                default:
                    throw new NotImplementedException();
            }
        }

        public DatabaseObject GetObject(string databaseName, string schemaName, string objectName)
        {
            if (userDefinedTypes.ContainsKey(databaseName, schemaName, objectName))
            {
                return userDefinedTypes[databaseName, schemaName, objectName];
            }
            else if (tables.ContainsKey(databaseName, schemaName, objectName))
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

        private void OnObjectLoading<T>(object sender, LazyItemLoadingEventArgs<string, T> e)
            where T : DatabaseObject, new()
        {
            T obj = new T();
            obj.Dataset = this;
            GetNamePartsFromObjectUniqueKey(obj, e.Key);

            EnsureSchemaValid(obj);

            try
            {
                OnLoadDatabaseObject<T>(obj);

                e.Value = obj;
                e.Key = GetObjectUniqueKey(obj);
                e.IsFound = true;
            }
            catch (SchemaException)
            {
                e.IsFound = false;
            }
            catch (Exception ex)
            {
                HandleException(ex);

                if (!captureError)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// When overloaded in derived classes, loads the database object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseObject"></param>
        protected abstract void OnLoadDatabaseObject<T>(T databaseObject)
            where T : DatabaseObject, new();

        private void OnAllObjectLoading<T>(object sender, AllItemsLoadingEventArgs<string, T> e)
            where T : DatabaseObject, new()
        {
            TryOperation(() =>
            {
                e.Items = OnLoadAllObjects<T>();
                e.IsCancelled = false;
            });
        }

        /// <summary>
        /// When overloaded in derived classes, loads all objects of a certain kind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, T>> OnLoadAllObjects<T>()
            where T : DatabaseObject, new();

        public bool IsObjectExisting(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject.SchemaName);

            return TryOperation(() =>
            {
                return OnIsObjectExisting(databaseObject);
            });
        }

        protected abstract bool OnIsObjectExisting(DatabaseObject databaseObject);

        public void LoadAllObjects(DatabaseObjectType objectType, bool forceReload)
        {
            objectType = Constants.SimpleDatabaseObjectTypes[objectType];

            switch (objectType)
            {
                case DatabaseObjectType.DataType:
                    UserDefinedTypes.LoadAll(forceReload);
                    break;
                case DatabaseObjectType.Table:
                    Tables.LoadAll(forceReload);
                    break;
                case DatabaseObjectType.View:
                    Views.LoadAll(forceReload);
                    break;
                case DatabaseObjectType.TableValuedFunction:
                    TableValuedFunctions.LoadAll(forceReload);
                    break;
                case DatabaseObjectType.ScalarFunction:
                    ScalarFunctions.LoadAll(forceReload);
                    break;
                case DatabaseObjectType.StoredProcedure:
                    StoredProcedures.LoadAll(forceReload);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerable<DatabaseObject> GetAllObjects(DatabaseObjectType objectType)
        {
            objectType = Constants.SimpleDatabaseObjectTypes[objectType];

            switch (objectType)
            {
                case DatabaseObjectType.DataType:
                    return UserDefinedTypes.Values;
                case DatabaseObjectType.Table:
                    return Tables.Values;
                case DatabaseObjectType.View:
                    return Views.Values;
                case DatabaseObjectType.TableValuedFunction:
                    return TableValuedFunctions.Values;
                case DatabaseObjectType.ScalarFunction:
                    return ScalarFunctions.Values;
                case DatabaseObjectType.StoredProcedure:
                    return StoredProcedures.Values;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// When overloaded in derived classes, loads all columns of a database object
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, Column>> LoadColumns(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            return TryOperation(() =>
            {
                return OnLoadColumns(databaseObject);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, loads all columns of a database object
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, Column>> OnLoadColumns(DatabaseObject databaseObject);

        public IEnumerable<KeyValuePair<string, Index>> LoadIndexes(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            return TryOperation(() =>
            {
                return OnLoadIndexes(databaseObject);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, loads all indexes belonging to a table or view
        /// </summary>
        /// <param name="tableOrView"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, Index>> OnLoadIndexes(DatabaseObject databaseObject);

        public IEnumerable<KeyValuePair<string, IndexColumn>> LoadIndexColumns(Index index)
        {
            EnsureSchemaValid(index);

            return TryOperation(() =>
            {
                return OnLoadIndexColumns(index);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, loads all columns covered by and index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, IndexColumn>> OnLoadIndexColumns(Index index);

        public IEnumerable<KeyValuePair<string, Parameter>> LoadParameters(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            return TryOperation(() =>
            {
                return OnLoadParameters(databaseObject);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, loads parameters of a function or stored procedure
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        protected abstract IEnumerable<KeyValuePair<string, Parameter>> OnLoadParameters(DatabaseObject databaseObject);

        #endregion
        #region Metadata functions

        public DatasetMetadata LoadDatasetMetadata()
        {
            return TryOperation(() =>
            {
                return OnLoadDatasetMetadata();
            });
        }

        protected abstract DatasetMetadata OnLoadDatasetMetadata();

        public DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            return TryOperation(() =>
            {
                return OnLoadDatabaseObjectMetadata(databaseObject);
            });
        }

        protected abstract DatabaseObjectMetadata OnLoadDatabaseObjectMetadata(DatabaseObject databaseObject);

        public void SaveDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            EnsureMutable(databaseObject);
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnSaveDatabaseObjectMetadata(databaseObject);
            });
        }

        protected abstract void OnSaveDatabaseObjectMetadata(DatabaseObject databaseObject);

        public void DropDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            EnsureMutable(databaseObject);
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnDropDatabaseObjectMetadata(databaseObject);
            });
        }

        protected abstract void OnDropDatabaseObjectMetadata(DatabaseObject databaseObject);

        /// <summary>
        /// Loads metadata of a variable (parameter, column, return value, etc.).
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        /// <remarks>Dataset is implemented to load all variable metadata of an
        /// object in a batch to reduce roundtrips between the client and the
        /// database server.</remarks>
        public void LoadAllVariableMetadata(DatabaseObject databaseObject)
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

        public void LoadAllColumnMetadata(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnLoadAllColumnMetadata(databaseObject);
            });
        }

        protected abstract void OnLoadAllColumnMetadata(DatabaseObject databaseObject);

        public void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnLoadAllParameterMetadata(databaseObject);
            });
        }

        protected abstract void OnLoadAllParameterMetadata(DatabaseObject databaseObject);

        public void SaveAllVariableMetadata(DatabaseObject databaseObject)
        {
            EnsureMutable(databaseObject);
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnSaveAllVariableMetadata(databaseObject);
            });
        }

        protected abstract void OnSaveAllVariableMetadata(DatabaseObject databaseObject);

        public void DropAllVariableMetadata(DatabaseObject databaseObject)
        {
            EnsureMutable(databaseObject);
            EnsureSchemaValid(databaseObject);

            TryOperation(() =>
            {
                OnDropAllVariableMetadata(databaseObject);
            });
        }

        protected abstract void OnDropAllVariableMetadata(DatabaseObject databaseObject);

        #endregion
        #region Statistics function

        public DatasetStatistics LoadDatasetStatistics()
        {
            return TryOperation(() =>
            {
                return OnLoadDatasetStatistics();
            });
        }

        protected abstract DatasetStatistics OnLoadDatasetStatistics();

        public TableStatistics LoadTableStatistics(TableOrView tableOrView)
        {
            EnsureSchemaValid(tableOrView);

            return TryOperation(() =>
            {
                return OnLoadTableStatistics(tableOrView);
            });
        }

        protected abstract TableStatistics OnLoadTableStatistics(TableOrView tableOrView);

        #endregion
        #region Object modification functions

        protected virtual void EnsureMutable(DatabaseObject databaseObject)
        {
            if (!IsMutable)
            {
                throw new InvalidOperationException("Operation valid on mutable datasets only.");   // TODO ***
            }
        }

        protected virtual void EnsureSchemaValid(DatabaseObject databaseObject)
        {
            var schema = databaseObject.SchemaName ?? databaseObject.Dataset.DefaultSchemaName;
            EnsureSchemaValid(schema);
        }

        protected virtual void EnsureSchemaValid(string schemaName)
        {
        }

        public void RenameObject(DatabaseObject obj, string schemaName, string objectName)
        {
            EnsureMutable(obj);
            EnsureSchemaValid(schemaName);

            LogOperation(
                "Renaming {0} {1}.{2} to {3}.{4} in database {5}:{6}.",
                Constants.DatabaseObjectsName_Singular[obj.ObjectType],
                obj.SchemaName, obj.ObjectName,
                schemaName, objectName,
                obj.DatasetName, obj.DatabaseName);

            TryOperation(() =>
            {
                OnRenameObject(obj, schemaName, objectName);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, renames an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="newName"></param>
        /// <remarks>
        /// Only works with mutable datasets, ie. myDBs
        /// </remarks>
        protected abstract void OnRenameObject(DatabaseObject obj, string schemaName, string objectName);

        public void CreateTable(Table table, bool createPrimaryKey, bool createIndexes)
        {
            var name = table.SchemaName + "." + table.ObjectName;

            LogOperation(
                "Creating table {0}.{1} in database {2}:{3}.",
                table.SchemaName, table.ObjectName,
                table.DatasetName, table.DatabaseName);

            TryOperation(() =>
            {
                OnCreateTable(table, createPrimaryKey, createIndexes);
            });
        }
        
        protected abstract void OnCreateTable(Table table, bool createPrimaryKey, bool createIndexes);

        public void DropObject(DatabaseObject obj)
        {
            LogOperation(
                "Dropping {0} {1}.{2} from database {3}:{4}.",
                Constants.DatabaseObjectsName_Singular[obj.ObjectType],
                obj.SchemaName, obj.ObjectName,
                obj.DatasetName, obj.DatabaseName);

            TryOperation(() =>
            {
                OnDropObject(obj);
            });
        }

        /// <summary>
        /// When overloaded in derived classes, drops an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <remarks>
        /// Only works with mutable datasets, ie. myDBs
        /// </remarks>
        protected abstract void OnDropObject(DatabaseObject obj);

        public void CreateIndex(Index index)
        {
            LogOperation(
                "Creating index {0} on {1}.{2} in database {3}:{4}.",
                index.IndexName,
                index.DatabaseObject.SchemaName, index.DatabaseObject.ObjectName,
                index.DatabaseObject.DatasetName, index.DatabaseObject.DatabaseName);

            TryOperation(() =>
            {
                OnCreateIndex(index);
            });
        }

        protected abstract void OnCreateIndex(Index index);

        public void DropIndex(Index index)
        {
            LogOperation(
                "Dropping index {0} on {1}.{2} in database {3}:{4}.",
                index.IndexName,
                index.DatabaseObject.SchemaName, index.DatabaseObject.ObjectName,
                index.DatabaseObject.DatasetName, index.DatabaseObject.DatabaseName);

            TryOperation(() =>
            {
                OnDropIndex(index);
            });
        }

        protected abstract void OnDropIndex(Index index);

        public void TruncateTable(Table table)
        {
            LogOperation(
                "Truncating table {0}.{1} in database {2}:{3}.",
                table.SchemaName, table.ObjectName,
                table.DatasetName, table.DatabaseName);

            TryOperation(() =>
            {
                OnTruncateTable(table);
            });
        }

        protected abstract void OnTruncateTable(Table table);

        #endregion

        #region Column and data type mapping functions

        /// <summary>
        /// Gets data type details from a schema table row.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="isNullable"></param>
        protected void GetDataTypeDetails(DataRow dr, out Type type, out string name, out int length, out byte precision, out byte scale, out bool isNullable)
        {
            // Get .Net type and other parameters
            type = (Type)dr[SchemaTableColumn.DataType];
            name = (string)dr["DataTypeName"];
            length = Convert.ToInt32(dr[SchemaTableColumn.ColumnSize]);
            precision = Convert.ToByte(dr[SchemaTableColumn.NumericPrecision]);
            scale = Convert.ToByte(dr[SchemaTableColumn.NumericScale]);
            isNullable = Convert.ToBoolean(dr[SchemaTableColumn.AllowDBNull]);

            // TODO: support UDTs?
        }

        /// <summary>
        /// Creates a data type based on a schema table row
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected virtual DataType MapDataType(DataRow dr)
        {
            // This is just a fall-back implementation that uses .Net types.
            // As not all .Net types are supported by the database servers,
            // specific dataset types override this function

            Type type;
            string name;
            int length;
            byte precision, scale;
            bool isNullable;

            GetDataTypeDetails(dr, out type, out name, out length, out precision, out scale, out isNullable);

            return DataType.Create(type, length, precision, scale, isNullable);

            // TODO: support UDTs?
        }

        /// <summary>
        /// Creates a data type based on its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract DataType MapDataType(string name);

        protected DataType MapDataType(string name, int length, byte scale, byte precision, bool isNullable)
        {
            var dt = MapDataType(name);

            if (dt.HasLength)
            {
                dt.Length = length;
            }

            if (length < 0)
            {
                dt.Length = Int32.MaxValue;
                dt.MaxLength = -1;
            }

            if (dt.HasScale)
            {
                dt.Scale = scale;
            }

            if (dt.HasPrecision)
            {
                dt.Precision = precision;
            }

            dt.IsNullable = isNullable;

            return dt;
        }

        /// <summary>
        /// Returns a schema column describing the resultset
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<Column> DetectColumns(IDataReader reader)
        {
            // TODO: modify to accept command instead of data reader?
            // command then could be executed in schema-only mode without
            // actual query execution...
            // or just create an overload that takes a command...

            bool resetKey = false;
            var columns = new List<Column>();
            var st = reader.GetSchemaTable();

            if (st != null)
            {
                for (int i = 0; i < st.Rows.Count; i++)
                {
                    var dr = st.Rows[i];
                    var col = DetectColumn(dr);


                    // Skip hidden columns, for example key columns returned when
                    // CommandBehaviour.KeyInfo is set but the key column is
                    // not part of the select list.
                    if (!col.IsHidden)
                    {
                        columns.Add(col);
                    }
                    else
                    {
                        // If hidden column is a key, make sure all key flags are cleared
                        // because only full combinations of key columns are unique
                        if (col.IsKey)
                        {
                            resetKey = true;
                        }
                    }
                }

                if (resetKey)
                {
                    foreach (var c in columns)
                    {
                        c.IsKey = false;
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Returns a schema column based on a schema table row
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private Column DetectColumn(DataRow dr)
        {
            var column = new Column()
            {
                ID = (int)dr[SchemaTableColumn.ColumnOrdinal] + 1,  // To be the same as returned by the system views
                Name = (string)dr[SchemaTableColumn.ColumnName],
                IsIdentity = dr[SchemaTableColumn.IsUnique] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsUnique],  //
                IsKey = dr[SchemaTableColumn.IsKey] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsKey],  //
                IsHidden = dr[SchemaTableOptionalColumn.IsHidden] == DBNull.Value ? false : (bool)dr[SchemaTableOptionalColumn.IsHidden],

                DataType = MapDataType(dr),
            };

            return column;
        }

        #endregion

        public abstract string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist);

        // TODO: try to get rid of this
        public abstract DbConnection OpenConnection();

        public abstract Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken);

        public void FlushCache()
        {
            this.userDefinedTypes.Clear();
            this.tables.Clear();
            this.views.Clear();
            this.tableValuedFunctions.Clear();
            this.scalarFunctions.Clear();
            this.storedProcedures.Clear();
            this.statistics.Clear();
            this.metadata.Clear();
        }

        #region Logging and error handling

        private void TryOperation(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                HandleException(ex);

                if (!captureError)
                {
                    throw;
                }
            }
        }

        private T TryOperation<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                HandleException(ex);

                if (!captureError)
                {
                    throw;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private void HandleException(Exception ex)
        {
            // TODO: sort out permanent and transient errors here
            // and only set dataset into failing state if the
            // error is permanent.

            LogError(ex);

            this.lastException = ex;

            if (failOnError)
            {
                this.IsInError = true;
            }

#if BREAKDEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif
        }

        protected Logging.Event LogError(Exception ex)
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Error,
                Logging.EventSource.Schema,
                null,
                null,
                ex,
                null);

            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        protected Logging.Event LogDebug(string message, params object[] args)
        {
#if DEBUG
            var method = new StackFrame(1, true).GetMethod();

            var e = Logging.LoggingContext.Current.LogDebug(
                Logging.EventSource.Schema,
                String.Format(message, args),
                method.DeclaringType.FullName + "." + method.Name,
                null);

            return e;
#endif
        }

        protected Logging.Event LogOperation(string message, params object[] args)
        {
            var method = new StackFrame(1, true).GetMethod();

            var e = Logging.LoggingContext.Current.LogOperation(
                Logging.EventSource.Schema,
                String.Format(message, args),
                method.DeclaringType.FullName + "." + method.Name,
                null);

            return e;
        }

        #endregion
    }
}
