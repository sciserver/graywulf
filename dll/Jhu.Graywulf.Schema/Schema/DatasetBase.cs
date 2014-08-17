using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;

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
    public abstract partial class DatasetBase : ICloneable, IDatasetSafe
    {
        #region Property storage member variables

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
            this.isCacheable = false;
            this.isMutable = false;
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
            this.isCacheable = old.isCacheable;
            this.isMutable = old.isMutable;
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

        private void OnAllObjectLoading<T>(object sender, AllItemsLoadingEventArgs<string, T> e)
            where T : DatabaseObject, new()
        {
            e.Items = LoadAllObjects<T>();
            e.IsCancelled = false;
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
        protected abstract IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>()
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
        }

        /// <summary>
        /// Creates a data type based on a schema table row
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected virtual DataType CreateDataType(DataRow dr)
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
        }

        /// <summary>
        /// Creates a data type based on its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual DataType CreateDataType(string name)
        {
            throw new NotImplementedException();
        }

        internal DataType CreateDataType(string name, int length, byte scale, byte precision, bool isNullable)
        {
            var dt = CreateDataType(name);

            if (dt.HasLength)
            {
                dt.Length = length;
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
        internal List<Column> DetectColumns(IDataReader reader)
        {
            // TODO: modify to accept command instead of data reader?

            var columns = new List<Column>();
            var st = reader.GetSchemaTable();

            for (int i = 0; i < st.Rows.Count; i++)
            {
                var dr = st.Rows[i];

                columns.Add(CreateColumn(dr));
            }

            return columns;
        }

        /// <summary>
        /// Returns a schema column based on a schema table row
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        internal virtual Column CreateColumn(DataRow dr)
        {
            var column = new Column()
            {
                ID = (int)dr[SchemaTableColumn.ColumnOrdinal],
                Name = (string)dr[SchemaTableColumn.ColumnName],
                IsIdentity = dr[SchemaTableColumn.IsUnique] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsUnique],  //
                IsKey = dr[SchemaTableColumn.IsKey] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsKey],  //
                IsHidden = dr[SchemaTableOptionalColumn.IsHidden] == DBNull.Value ? false : (bool)dr[SchemaTableOptionalColumn.IsHidden],

                DataType = CreateDataType(dr),
            };

            return column;
        }

        #endregion     

        public abstract IDbConnection OpenConnection();
    }
}
