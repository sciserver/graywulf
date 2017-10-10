using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema.SqlServer
{
    /// <summary>
    /// Implements schema reflection functions for MS SQL Server
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class SqlServerDataset : DatasetBase
    {
        #region Private members

        [NonSerialized]
        protected bool isOnLinkedServer;

        [NonSerialized]
        protected bool isRemoteDataset;

        [NonSerialized]
        protected bool isRestrictedSchema;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the value determining if the data is available
        /// via a linked SQL server.
        /// </summary>
        [DataMember]
        public bool IsOnLinkedServer
        {
            get { return isOnLinkedServer; }
            set { isOnLinkedServer = value; }
        }

        [DataMember]
        public bool IsRestrictedSchema
        {
            get { return isRestrictedSchema; }
            set { isRestrictedSchema = value; }
        }

        [IgnoreDataMember]
        public override string ProviderName
        {
            get { return Constants.SqlServerProviderName; }
        }

        /// <summary>
        /// Gets or sets the database name associated with this dataset.
        /// </summary>
        /// <remarks>
        /// The database name only refers to the schema prototype, not
        /// the actual database instances!
        /// </remarks>
        [IgnoreDataMember]
        public override string DatabaseName
        {
            get
            {
                var csb = new SqlConnectionStringBuilder(ConnectionString);
                return csb.InitialCatalog;
            }
            set
            {
                var csb = new SqlConnectionStringBuilder(ConnectionString);
                csb.InitialCatalog = value;
                ConnectionString = csb.ConnectionString;
            }
        }

        [IgnoreDataMember]
        public string HostName
        {
            get
            {
                var csb = new SqlConnectionStringBuilder(ConnectionString);
                int i = csb.DataSource.IndexOf('\\');
                if (i > -1)
                {
                    return csb.DataSource.Substring(0, i);
                }
                else
                {
                    return csb.DataSource;
                }
            }
        }

        [IgnoreDataMember]
        public string InstanceName
        {
            get
            {
                var csb = new SqlConnectionStringBuilder(ConnectionString);
                int i = csb.DataSource.IndexOf('\\');
                if (i > -1)
                {
                    return csb.DataSource.Substring(i + 1);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public SqlServerDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public SqlServerDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        public SqlServerDataset(string name, string connectionString)
            : base()
        {
            InitializeMembers(new StreamingContext());

            Name = name;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public SqlServerDataset(SqlServerDataset old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes private member variables to their default values.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.DefaultSchemaName = Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName;

            this.isOnLinkedServer = false;
            this.isRemoteDataset = false;
            this.isRestrictedSchema = false;
        }

        /// <summary>
        /// Copies private member variables form another instance
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(SqlServerDataset old)
        {
            this.DefaultSchemaName = old.DefaultSchemaName;

            this.isOnLinkedServer = old.isOnLinkedServer;
            this.isRemoteDataset = old.isRemoteDataset;
            this.isRestrictedSchema = old.isRestrictedSchema;
        }

        public override object Clone()
        {
            return new SqlServerDataset(this);
        }

        #endregion
        #region Fully resolved names and keys

        /// <summary>
        /// Return a quoted version of an identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override string QuoteIdentifier(string identifier)
        {
            // TODO: verify identifier quoting everywhere, because
            // escaping within [] might not be covered
            // also check if it can be changed from [] to ""

            identifier = identifier.Replace("]", "]]");

            return String.Format("[{0}]", identifier);
        }

        /// <summary>
        /// Returns the fully resolved name of a database object
        /// belonging to the dataset.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        /// <remarks>
        /// The function returns the object name as it can be referenced
        /// 
        /// </remarks>
        public override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            // If schema name is empty, use default schema name

            var databaseName = GetFullyResolvedName();
            var schemaName =
                String.IsNullOrWhiteSpace(databaseObject.SchemaName) ?
                    QuoteIdentifier(DefaultSchemaName) :
                    QuoteIdentifier(databaseObject.SchemaName);
            var objectName = QuoteIdentifier(databaseObject.ObjectName);

            return String.Format("{0}.{1}.{2}", databaseName, schemaName, objectName);
        }

        /// <summary>
        /// Returns the unique string key of a database object belonging to the dataset.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="datasetName"></param>
        /// <param name="databaseName"></param>
        /// <param name="schemaName"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public override string GetObjectUniqueKey(DatabaseObjectType objectType, string datasetName, string databaseName, string schemaName, string objectName)
        {
            // If schema name is empty, use default schema name
            schemaName =
                String.IsNullOrWhiteSpace(schemaName) ?
                DefaultSchemaName :
                schemaName;

            return base.GetObjectUniqueKey(objectType, datasetName, databaseName, schemaName, objectName);
        }

        #endregion
        #region Schema objects

        protected internal override void EnsureMutable(DatabaseObject databaseObject)
        {
            base.EnsureMutable(databaseObject);

            EnsureSchemaValid(databaseObject.SchemaName ?? databaseObject.Dataset.DefaultSchemaName);
        }

        protected virtual void EnsureSchemaValid(string schemaName)
        {
            if (isRestrictedSchema && SchemaManager.Comparer.Compare(schemaName, DefaultSchemaName) != 0)
            {
                throw new InvalidOperationException("Operation valid on mutable schemas only.");    // *** TODO
            }
        }

        private void SetSchemaNameParameter(SqlCommand cmd, string schemaName)
        {
            if (isRestrictedSchema && String.IsNullOrWhiteSpace(schemaName))
            {
                schemaName = DefaultSchemaName;
            }

            EnsureSchemaValid(schemaName);

            cmd.Parameters.Add("@schemaName", SqlDbType.NVarChar, 128).Value = String.IsNullOrWhiteSpace(schemaName) ? (object)DBNull.Value : (object)schemaName;
        }

        private void SetObjectNameParameter(SqlCommand cmd, string objectName)
        {
            cmd.Parameters.Add("@objectName", SqlDbType.NVarChar, 128).Value = objectName;
        }

        private void SetTypeNameParameter(SqlCommand cmd, string typeName)
        {
            cmd.Parameters.Add("@typeName", SqlDbType.NVarChar, 128).Value = typeName;
        }

        private void SetIndexNameParameter(SqlCommand cmd, string indexName)
        {
            cmd.Parameters.Add("@indexName", SqlDbType.NVarChar, 128).Value = indexName;
        }

        private string GetDataTypeQuery(bool appendNameFilter)
        {
            var sql = @"
SELECT s.name, t.name, st.name,
	t.max_length, t.precision, t.scale, 
    t.is_nullable, t.is_table_type, t.is_assembly_type,
    at.is_binary_ordered, at.is_fixed_length, at.assembly_qualified_name
FROM sys.types t
INNER JOIN sys.schemas s
	ON s.schema_id = t.schema_id
LEFT OUTER JOIN sys.types st
	ON st.system_type_id = t.system_type_id AND st.user_type_id = t.system_type_id AND st.is_user_defined = 0
LEFT OUTER JOIN sys.assembly_types at
    ON at.system_type_id = t.system_type_id AND at.user_type_id = t.user_type_id
WHERE 
	t.is_user_defined = 1 
";

            if (appendNameFilter)
            {
                sql += "    AND (s.name = @schemaName OR @schemaName IS NULL) AND t.name = @typeName";
            }

            return sql;
        }

        private void LoadDataType(SqlDataReader dr, DataType dataType)
        {
            dataType.Dataset = this;
            dataType.DatabaseName = DatabaseName;
            dataType.SchemaName = dr.GetString(0);
            dataType.ObjectName = dr.GetString(1);
            dataType.ObjectType = DatabaseObjectType.DataType;

            dataType.IsUserDefined = true;
            dataType.IsNullable = dr.GetBoolean(6);
            dataType.IsTableType = dr.GetBoolean(7);
            dataType.IsAssemblyType = dr.GetBoolean(8);

            if (dataType.IsAlias)
            {
                // This is a type alias, so fill in base type info
                var dt = CreateDataType(dr.GetString(2), dr.GetInt16(3), dr.GetByte(4), dr.GetByte(5), dr.GetBoolean(6));

                dataType.Type = dt.Type;
                dataType.SqlDbType = dt.SqlDbType;
                dataType.ByteSize = dt.ByteSize;
                dataType.Precision = dt.Precision;
                dataType.Scale = dt.Scale;
                dataType.Length = dt.Length / dt.ByteSize;
                dataType.MaxLength = dt.MaxLength;
                dataType.ArrayLength = dt.ArrayLength;
            }
            else if (dataType.IsTableType)
            {
                dataType.Type = null;
                dataType.SqlDbType = SqlDbType.Structured;
                dataType.ByteSize = 1;
                dataType.Scale = 0;
                dataType.Precision = 0;
                dataType.Length = 1;
                dataType.MaxLength = dr.GetInt16(3);
                dataType.IsFixedLength = false;
                dataType.ArrayLength = 0;
            }
            else if (dataType.IsAssemblyType)
            {
                dataType.Type = null;   // TODO: could use udt type if available
                dataType.SqlDbType = SqlDbType.Udt;
                dataType.ByteSize = 1;
                dataType.Scale = 0;
                dataType.Precision = 0;
                dataType.Length = 1;
                dataType.MaxLength = dr.GetInt16(3);
                dataType.IsFixedLength = dr.GetBoolean(10);
                dataType.ArrayLength = 0;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Loads the schema of a database object belonging to the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseObject"></param>
        protected override void LoadDatabaseObject<T>(T databaseObject)
        {
            if (databaseObject is DataType)
            {
                LoadDataType((DataType)(DatabaseObject)databaseObject);
            }
            else
            {
                LoadDatabaseObjectImpl<T>(databaseObject);
            }
        }

        private void LoadDataType(DataType dataType)
        {
            var sql = GetDataTypeQuery(true);

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, dataType.SchemaName);
                    SetTypeNameParameter(cmd, dataType.TypeName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        int q = 0;
                        while (dr.Read())
                        {
                            LoadDataType(dr, dataType);
                            q++;
                        }

                        // No records
                        if (q == 0)
                        {
                            ThrowInvalidDataTypeNameException(dataType);
                        }
                        else if (q > 1)
                        {
                            throw new SchemaException("ambigous name"); // TODO
                        }
                    }
                }
            }
        }

        private void LoadDatabaseObjectImpl<T>(T databaseObject)
            where T : DatabaseObject, new()
        {
            var sql = @"
SELECT s.name, o.name, o.type, o.create_date, o.modify_date
FROM sys.objects o
INNER JOIN sys.schemas s
	ON s.schema_id = o.schema_id
WHERE o.type IN ({0}) AND
    (s.name = @schemaName OR @schemaName IS NULL) AND o.name = @objectName
";

            sql = String.Format(sql, GetObjectTypeIdListString(databaseObject.ObjectType));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        int q = 0;
                        while (dr.Read())
                        {
                            databaseObject.Dataset = this;
                            databaseObject.DatabaseName = DatabaseName;
                            databaseObject.SchemaName = dr.GetString(0);
                            databaseObject.ObjectName = dr.GetString(1);
                            databaseObject.ObjectType = Constants.SqlServerObjectTypeIds[dr.GetString(2).Trim()];
                            databaseObject.Metadata.DateCreated = dr.GetDateTime(3).ToUniversalTime();
                            databaseObject.Metadata.DateModified = dr.GetDateTime(4).ToUniversalTime();

                            q++;
                        }

                        // No records
                        if (q == 0)
                        {
                            ThrowInvalidObjectNameException(databaseObject);
                        }
                        else if (q > 1)
                        {
                            throw new SchemaException("ambigous name"); // TODO
                        }
                    }
                }
            }
        }

        internal override bool IsObjectExisting(DatabaseObject databaseObject)
        {
            EnsureSchemaValid(databaseObject.SchemaName);

            var sql = String.Format(
                @"SELECT OBJECT_ID('{0}')",
                GetObjectFullyResolvedName(databaseObject));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    return cmd.ExecuteScalar() != DBNull.Value;
                }
            }
        }

        /// <summary>
        /// Loads all database objects of type T belonging to the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        protected override IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>()
        {
            if (typeof(T) == typeof(DataType))
            {
                return (IEnumerable<KeyValuePair<string, T>>)LoadAllDataTypes();
            }
            else
            {
                return LoadAllObjectsImpl<T>();
            }
        }

        private IEnumerable<KeyValuePair<string, DataType>> LoadAllDataTypes()
        {
            var sql = GetDataTypeQuery(false);

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var dataType = new DataType();
                            LoadDataType(dr, dataType);
                            yield return new KeyValuePair<string, DataType>(GetObjectUniqueKey(dataType), dataType);
                        }
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<string, T>> LoadAllObjectsImpl<T>()
            where T : DatabaseObject, new()
        {
            var sql = @"
SELECT s.name, o.name, o.type, o.create_date, o.modify_date
FROM sys.objects o
INNER JOIN sys.schemas s
	ON s.schema_id = o.schema_id
WHERE o.type IN ({0}) 
    AND (s.name = @schemaName OR @schemaName IS NULL)
";

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, null);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            T obj = new T()
                            {
                                Dataset = this,
                                DatabaseName = DatabaseName,
                                SchemaName = dr.GetString(0),
                                ObjectName = dr.GetString(1),
                                ObjectType = Constants.SqlServerObjectTypeIds[dr.GetString(2).Trim()],
                            };

                            obj.Metadata.DateCreated = dr.GetDateTime(3).ToUniversalTime();
                            obj.Metadata.DateModified = dr.GetDateTime(4).ToUniversalTime();

                            yield return new KeyValuePair<string, T>(GetObjectUniqueKey(obj), obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads columns of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, Column>> LoadColumns(DatabaseObject databaseObject)
        {
            string sql;

            if (databaseObject is DataType)
            {
                sql = @"
SELECT c.column_id, c.name, 
	cts.name, ct.name, ct.is_user_defined,
	c.max_length, c.scale, c.precision, c.is_nullable, c.is_identity, CAST(0 AS bit) AS is_key
FROM sys.table_types t
INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
INNER JOIN sys.columns c ON c.object_id = t.type_table_object_id
LEFT OUTER JOIN sys.types ct ON ct.user_type_id = c.user_type_id AND ct.system_type_id = c.system_type_id
LEFT OUTER JOIN sys.schemas cts ON cts.schema_id = ct.schema_id
WHERE s.name = @schemaName AND t.name = @objectName
ORDER BY c.column_id
";
            }
            else if (databaseObject is View)
            {
                sql = @"
WITH refs AS
(
	SELECT d.referenced_id, 1 AS depth
	FROM sys.objects o
	INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = o.object_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
		AND s.name = @schemaName AND o.name = @objectName

	UNION ALL

	SELECT d.referenced_id, r.depth + 1 AS depth
	FROM refs r
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = r.referenced_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
)
SELECT c.column_id, c.name,
    cts.name, ct.name, ct.is_user_defined,
    c.max_length, c.scale, c.precision, c.is_nullable, 
	c.is_identity, CASE WHEN ic.index_column_id IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS is_key
FROM (SELECT TOP 1 * FROM refs ORDER BY depth DESC) refs
INNER JOIN sys.objects o ON o.object_id = refs.referenced_id
INNER JOIN sys.columns c ON c.object_id = o.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
LEFT OUTER JOIN sys.types ct ON ct.user_type_id = c.user_type_id AND ct.system_type_id = c.system_type_id
LEFT OUTER JOIN sys.schemas cts ON cts.schema_id = ct.schema_id
LEFT OUTER JOIN sys.indexes i ON i.object_id = o.object_id AND i.type = 1 AND i.is_unique = 1	-- primary key
LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = o.object_id AND ic.index_id = i.index_id AND ic.column_id = c.column_id
ORDER BY c.column_id";
            }
            else if (databaseObject is Table || databaseObject is TableValuedFunction)
            {
                sql = @"
SELECT c.column_id, c.name,
    cts.name, ct.name, ct.is_user_defined,
    c.max_length, c.scale, c.precision, c.is_nullable, 
	c.is_identity, CASE WHEN ic.index_column_id IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS is_key
FROM sys.columns c
INNER JOIN sys.objects o ON o.object_id = c.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
LEFT OUTER JOIN sys.types ct ON ct.user_type_id = c.user_type_id AND ct.system_type_id = c.system_type_id
LEFT OUTER JOIN sys.schemas cts ON cts.schema_id = ct.schema_id
LEFT OUTER JOIN sys.indexes i ON i.object_id = o.object_id AND i.type = 1 AND i.is_unique = 1	-- primary key
LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = o.object_id AND ic.index_id = i.index_id AND ic.column_id = c.column_id
WHERE s.name = @schemaName AND o.name = @objectName
ORDER BY c.column_id
";
            }
            else
            {
                throw new NotImplementedException();
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var cd = new Column(databaseObject)
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                                IsIdentity = dr.GetBoolean(9),
                                IsKey = dr.GetBoolean(10)
                            };

                            var udt = dr.GetBoolean(4);

                            if (udt)
                            {
                                var schema = dr.GetString(2);
                                var type = dr.GetString(3);
                                cd.DataType = databaseObject.Dataset.UserDefinedTypes[databaseObject.Dataset.DatabaseName, schema, type];
                            }
                            else
                            {
                                cd.DataType = CreateDataType(
                                    dr.GetString(3),
                                    Convert.ToInt32(dr.GetValue(5)),
                                    Convert.ToByte(dr.GetValue(6)),
                                    Convert.ToByte(dr.GetValue(7)),
                                    dr.GetBoolean(8));

                                // SQL Server reports column sizes in bytes for unicode columns whereas
                                // SchemaTable, when accessed via ADO.NET returns the number of characters.
                                // To account for this, column sizes need to be divided by the number of
                                // bytes per character here, and only here.

                                if (cd.DataType.HasLength && cd.DataType.ByteSize > 1)
                                {
                                    cd.DataType.Length /= cd.DataType.ByteSize;
                                }
                            }

                            yield return new KeyValuePair<string, Column>(cd.Name, cd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads indexes of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, Index>> LoadIndexes(DatabaseObject databaseObject)
        {
            string sql;

            if (databaseObject is DataType)
            {
                sql = @"
SELECT i.index_id, i.name, i.type, i.is_unique, i.is_primary_key
FROM sys.indexes i
INNER JOIN sys.table_types t ON t.type_table_object_id = i.object_id
INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE s.name = @schemaName AND t.name = @objectName";
            }
            else if (databaseObject is Table)
            {
                sql = @"
SELECT i.index_id, i.name, i.type, i.is_unique, i.is_primary_key
FROM sys.indexes i
INNER JOIN sys.objects o ON o.object_id = i.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
WHERE i.type IN (1, 2) AND
s.name = @schemaName AND o.name = @objectName";
            }
            else
            {
                // This is a trick to determine the primary key of an underlying table of a SELECT * view
                // Will not work with JOIN views but it's hard to figure out without executing the query.
                // Still helpful to support certain types of queries

                sql = @"
WITH refs AS
(
	SELECT d.referenced_id
	FROM sys.objects o
	INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = o.object_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
		AND s.name = @schemaName AND o.name = @objectName

	UNION ALL

	SELECT d.referenced_id
	FROM refs r
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = r.referenced_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
)
SELECT i.index_id, i.name, i.type, i.is_unique, i.is_primary_key
FROM refs
INNER JOIN sys.indexes i ON i.object_id = refs.referenced_id
WHERE   i.type IN (1, 2)";
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var idx = new Index((IIndexes)databaseObject)
                            {
                                IndexId = dr.GetInt32(0),
                                IndexName = dr.GetString(1),
                                IsClustered = (dr.GetByte(2) == 1),
                                IsUnique = dr.GetBoolean(3),
                                IsPrimaryKey = dr.GetBoolean(4),
                            };

                            yield return new KeyValuePair<string, Index>(idx.IndexName, idx);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads columns of an index of a database object.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, IndexColumn>> LoadIndexColumns(Index index)
        {
            // TODO: maybe add UDT support to index column types...

            string sql;

            if (index.DatabaseObject is DataType)
            {
                sql = @"
SELECT ic.column_id, ic.is_included_column, ic.key_ordinal,
    c.name, ic.is_descending_key, 
    cts.name, ct.name, ct.is_user_defined, c.max_length, c.scale, c.precision, c.is_nullable, c.is_identity
FROM sys.indexes AS i
INNER JOIN sys.table_types AS t
    ON i.object_id = t.type_table_object_id
INNER JOIN sys.schemas AS s
    ON s.schema_id = t.schema_id
INNER JOIN sys.index_columns AS ic 
    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns AS c 
    ON ic.object_id = c.object_id AND c.column_id = ic.column_id
INNER JOIN sys.types ct
    ON ct.user_type_id = c.user_type_id
LEFT OUTER JOIN sys.schemas cts
    ON cts.schema_id = ct.schema_id
WHERE s.name = @schemaName AND
    t.name = @objectName AND
    i.name = @indexName
ORDER BY ic.key_ordinal";
            }
            else if (index.DatabaseObject is Table)
            {
                sql = @"
SELECT ic.column_id, ic.is_included_column, ic.key_ordinal, 
    c.name, ic.is_descending_key, 
    cts.name, ct.name, ct.is_user_defined, c.max_length, c.scale, c.precision, c.is_nullable, c.is_identity
FROM sys.indexes AS i
INNER JOIN sys.objects AS o
    ON i.object_id = o.object_id
INNER JOIN sys.schemas AS s
    ON s.schema_id = o.schema_id
INNER JOIN sys.index_columns AS ic 
    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns AS c 
    ON ic.object_id = c.object_id AND c.column_id = ic.column_id
INNER JOIN sys.types ct
    ON ct.user_type_id = c.user_type_id
LEFT OUTER JOIN sys.schemas cts
    ON cts.schema_id = ct.schema_id
WHERE s.name = @schemaName AND
    o.name = @objectName AND
    i.name = @indexName
ORDER BY ic.key_ordinal";
            }
            else
            {
                sql = @"
WITH refs AS
(
	SELECT d.referenced_id
	FROM sys.objects o
	INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = o.object_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
		AND s.name = @schemaName AND o.name = @objectName

	UNION ALL

	SELECT d.referenced_id
	FROM refs r
	INNER JOIN sys.sql_expression_dependencies d ON d.referencing_id = r.referenced_id
	WHERE d.referenced_class = 1 AND d.referenced_minor_id = 0
)

SELECT ic.column_id, ic.is_included_column, ic.key_ordinal, 
    c.name, ic.is_descending_key, 
    cts.name, ct.name, ct.is_user_defined, c.max_length, c.scale, c.precision, c.is_nullable, c.is_identity
FROM sys.indexes AS i
INNER JOIN refs ON refs.referenced_id = i.object_id
INNER JOIN sys.index_columns AS ic 
    ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns AS c 
    ON ic.object_id = c.object_id AND c.column_id = ic.column_id
INNER JOIN sys.types ct
    ON ct.user_type_id = c.user_type_id
LEFT OUTER JOIN sys.schemas cts
    ON cts.schema_id = ct.schema_id
WHERE 
    i.name = @indexName
ORDER BY ic.key_ordinal";
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, index.DatabaseObject.SchemaName);
                    SetObjectNameParameter(cmd, index.DatabaseObject.ObjectName);
                    SetIndexNameParameter(cmd, index.IndexName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            IndexColumnOrdering ordering;
                            var isincluded = dr.GetBoolean(1);
                            var isdescending = dr.GetBoolean(4);
                            var isuserdefinedtype = dr.GetBoolean(7);

                            if (isincluded)
                            {
                                ordering = IndexColumnOrdering.Unknown;
                            }
                            else
                            {
                                ordering = isdescending ? IndexColumnOrdering.Descending : IndexColumnOrdering.Ascending;
                            }

                            var ic = new IndexColumn()
                            {
                                ID = dr.GetInt32(0),
                                IsIncluded = dr.GetBoolean(1),
                                KeyOrdinal = dr.GetByte(2),
                                Name = dr.GetString(3),
                                Ordering = ordering,
                                IsIdentity = dr.GetBoolean(12),
                            };

                            if (isuserdefinedtype)
                            {
                                ic.DataType = index.Dataset.UserDefinedTypes[index.Dataset.DatabaseName, dr.GetString(5), dr.GetString(6)];
                            }
                            else
                            {
                                ic.DataType = CreateDataType(
                                    dr.GetString(6),
                                    Convert.ToInt32(dr.GetValue(8)),
                                    Convert.ToByte(dr.GetValue(9)),
                                    Convert.ToByte(dr.GetValue(10)),
                                    dr.GetBoolean(11));
                            }

                            yield return new KeyValuePair<string, IndexColumn>(ic.Name, ic);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads parameters of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, Parameter>> LoadParameters(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT p.parameter_id, p.name, p.is_output, 
    ts.name, t.name, t.is_user_defined,
    p.max_length, p.scale, p.precision, p.has_default_value, p.default_value
FROM sys.parameters p
INNER JOIN sys.objects o ON o.object_id = p.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.types t ON t.user_type_id = p.user_type_id
LEFT OUTER JOIN sys.schemas ts ON ts.schema_id = t.schema_id
WHERE s.name = @schemaName AND o.name = @objectName
ORDER BY p.parameter_id";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // TODO: check this for input only parameters!
                            var dir = String.IsNullOrEmpty(dr.GetString(1)) ? ParameterDirection.ReturnValue : ParameterDirection.InputOutput;

                            var par = new Parameter()
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                                Direction = dir,
                                HasDefaultValue = dr.GetBoolean(9),
                                DefaultValue = dr.IsDBNull(10) ? null : dr.GetValue(10),
                            };

                            var isudt = dr.GetBoolean(5);

                            if (isudt)
                            {
                                var schema = dr.GetString(3);
                                var type = dr.GetString(4);
                                par.DataType = databaseObject.Dataset.UserDefinedTypes[databaseObject.Dataset.DatabaseName, schema, type];
                            }
                            else
                            {
                                par.DataType = CreateDataType(
                                    dr.GetString(4),
                                    Convert.ToInt32(dr.GetValue(6)),
                                    Convert.ToByte(dr.GetValue(7)),
                                    Convert.ToByte(dr.GetValue(8)),
                                    false);
                            }

                            yield return new KeyValuePair<string, Parameter>(par.Name, par);
                        }
                    }
                }
            }
        }
        #endregion
        #region Metadata

        /// <summary>
        /// Loads metadata of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal protected override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            var meta = new DatabaseObjectMetadata();

            if (databaseObject is DataType)
            {
                LoadDataTypeProperties((DataType)databaseObject, meta);
                LoadDataTypeExtendedProperties((DataType)databaseObject, meta);
            }
            else
            {
                LoadDatabaseObjectProperties(databaseObject, meta);
                LoadDatabaseObjectExtendedProperties(databaseObject, meta);
            }

            return meta;
        }

        private void LoadDatabaseObjectProperties(DatabaseObject databaseObject, DatabaseObjectMetadata metadata)
        {
            var sql = @"
SELECT is_ms_shipped, o.create_date, o.modify_date
FROM sys.objects o
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
WHERE o.type IN ({0}) AND
      (s.name = @schemaName OR @schemaName IS NULL) AND o.name = @objectName";

            sql = String.Format(sql, GetObjectTypeIdListString(databaseObject.ObjectType));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            metadata.System = dr.GetBoolean(0);
                            metadata.DateCreated = dr.GetDateTime(1).ToUniversalTime();
                            metadata.DateModified = dr.GetDateTime(2).ToUniversalTime();
                        }
                    }
                }
            }
        }

        private void LoadDataTypeProperties(DataType dataType, DatabaseObjectMetadata metadata)
        {
            var sql = @"
SELECT t.is_user_defined
FROM sys.types t
INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE (s.name = @schemaName OR @schemaName IS NULL) AND t.name = @objectName";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, dataType.SchemaName);
                    SetObjectNameParameter(cmd, dataType.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            metadata.System = dr.GetBoolean(0);
                        }
                    }
                }
            }
        }

        private void LoadDatabaseObjectExtendedProperties(DatabaseObject databaseObject, DatabaseObjectMetadata metadata)
        {
            var sql = @"
SELECT p.name, p.value
FROM sys.objects o
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.extended_properties p ON o.object_id = p.major_id
WHERE o.type IN ({0}) AND
      (s.name = @schemaName OR @schemaName IS NULL) AND o.name = @objectName AND
      p.class = 1 -- OBJECT_OR_COLUMN
      AND p.major_id = o.object_id
      AND p.minor_id = 0  -- only objects
      AND p.name LIKE 'meta.%'
ORDER BY 1";

            sql = String.Format(sql, GetObjectTypeIdListString(databaseObject.ObjectType));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var name = dr.GetString(0);
                            var value = dr.GetString(1);

                            SetMetadataProperty(metadata, name, value);
                        }
                    }
                }
            }
        }

        private void LoadDataTypeExtendedProperties(DataType dataType, DatabaseObjectMetadata metadata)
        {
            var sql = @"
SELECT p.name, p.value
FROM sys.types t
INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
INNER JOIN sys.extended_properties p ON t.user_type_id = p.major_id
WHERE (s.name = @schemaName OR @schemaName IS NULL) AND t.name = @objectName AND
      p.class = 1 -- OBJECT_OR_COLUMN
      AND p.major_id = t.user_type_id
      AND p.minor_id = 0  -- only objects
      AND p.name LIKE 'meta.%'
ORDER BY 1";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, dataType.SchemaName);
                    SetObjectNameParameter(cmd, dataType.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var name = dr.GetString(0);
                            var value = dr.GetString(1);

                            SetMetadataProperty(metadata, name, value);
                        }
                    }
                }
            }
        }

        private void SetMetadataProperty(DatabaseObjectMetadata metadata, string name, string value)
        {
            switch (name)
            {
                case Constants.MetaSummary:
                    metadata.Summary = value;
                    break;
                case Constants.MetaRemarks:
                    metadata.Remarks = value;
                    break;
                case Constants.MetaUrl:
                    metadata.Url = value;
                    break;
                case Constants.MetaIcon:
                    metadata.Icon = value;
                    break;
                case Constants.MetaDocPage:
                    metadata.DocPage = value;
                    break;
                case Constants.MetaExample:
                    metadata.Example = value;
                    break;
                case Constants.MetaClass:
                    metadata.Class = value;
                    break;
                default:
                    break;
            }
        }

        internal override void DropDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void SaveDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads column metadata for every column of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        protected override void LoadAllColumnMetadata(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT c.name, p.name metaname, p.value
FROM sys.columns c
INNER JOIN sys.objects o ON c.object_id = o.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.extended_properties p ON p.major_id = c.object_id AND p.minor_id = c.column_id
WHERE (s.name = @schemaName OR @schemaName IS NULL) AND o.name = @objectName
      AND p.name LIKE 'meta.%'
ORDER BY c.name, p.name";

            LoadAllVariableMetadata(sql, databaseObject, ((IColumns)databaseObject).Columns);
            // ****
        }

        /// <summary>
        /// Loads parameter metadata of all parameters belonging to an object.
        /// </summary>
        /// <param name="databaseObject"></param>
        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            // TODO: test this

            var sql = @"
SELECT c.name, p.name metaname, p.value
FROM sys.columns c
INNER JOIN sys.objects o ON c.object_id = o.object_id
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.extended_properties p ON p.major_id = c.object_id AND p.minor_id = c.column_id
WHERE (s.name = @schemaName OR @schemaName IS NULL) AND o.name = @objectName
      AND p.name LIKE 'meta.%'
ORDER BY c.name, p.name";

            LoadAllVariableMetadata(sql, databaseObject, ((IParameters)databaseObject).Parameters);
        }

        private void LoadAllVariableMetadata(string sql, DatabaseObject databaseObject, IDictionary variables)
        {
            // Make sure all will be marked as loaded
            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        string variablename = null;
                        VariableMetadata meta = null;

                        while (dr.Read())
                        {
                            string name = dr.GetString(0);
                            string metaname = dr.GetString(1);
                            string value = dr.GetValue(2).ToString();

                            if (name != variablename)
                            {
                                meta = (VariableMetadata)((Variable)variables[name]).Metadata;
                                variablename = name;
                            }

                            switch (metaname)
                            {
                                case Constants.MetaSummary:
                                    meta.Summary = value;
                                    break;
                                case Constants.MetaRemarks:
                                    meta.Remarks = value;
                                    break;
                                case Constants.MetaClass:
                                    meta.Class = value;
                                    break;
                                case Constants.MetaQuantity:
                                    meta.Quantity = Quantity.Parse(value);
                                    break;
                                case Constants.MetaUnit:
                                    meta.Unit = Unit.Parse(value);
                                    break;
                                case Constants.MetaFormat:
                                    meta.Format = value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        internal override void DropAllVariableMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void SaveAllVariableMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Statistics


        /// <summary>
        /// Loads statistics of the dataset.
        /// </summary>
        /// <returns></returns>
        protected override DatasetStatistics LoadDatasetStatistics()
        {
            var sql = @"
-- Raw data space in 8K pages
SELECT SUM(CASE f.growth WHEN 0 THEN f.size ELSE f.max_size END)
FROM sys.database_files f
WHERE f.type = 0

-- Total used space
SELECT SUM(a.total_pages), SUM(a.used_pages), SUM(a.data_pages)
FROM sys.allocation_units a

-- Log space
SELECT SUM(CASE f.growth WHEN 0 THEN f.size ELSE f.max_size END)
FROM sys.database_files f
WHERE f.type = 1
";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        var stats = new DatasetStatistics();

                        // Data space
                        dr.Read();
                        stats.DataSpace = dr.IsDBNull(0) ? 0L : (long)dr.GetInt32(0) * 0x2000;    // 8K pages

                        // Index space
                        dr.NextResult();
                        dr.Read();
                        stats.UsedSpace = dr.IsDBNull(0) ? 0L : dr.GetInt64(0) * 0x2000;    // 8K pages

                        // Row count
                        dr.NextResult();
                        dr.Read();
                        stats.LogSpace = dr.IsDBNull(0) ? 0L : (long)dr.GetInt32(0) * 0x2000;    // 8K pages

                        if (stats.LogSpace == 2199023255552L)
                        {
                            // In this special case the log file can grow to a maximum of 2TB
                            // https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-database-files-transact-sql
                            stats.LogSpace = -1;
                        }

                        return stats;
                    }
                }
            }
        }

        protected override DatasetMetadata LoadDatasetMetadata()
        {
            var meta = new DatasetMetadata();

            LoadDatasetProperties(meta);
            LoadDatasetExtendedProperties(meta);

            return meta;
        }

        private void LoadDatasetProperties(DatasetMetadata metadata)
        {
            var sql = @"
SELECT create_date
FROM sys.databases
WHERE name = @name";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = DatabaseName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            metadata.DateCreated = dr.GetDateTime(0).ToUniversalTime();
                        }
                    }
                }
            }
        }

        private void LoadDatasetExtendedProperties(DatasetMetadata metadata)
        {
            var sql = @"
SELECT p.name, p.value
FROM sys.extended_properties p
WHERE p.class = 0 -- DATABASE
      AND p.name LIKE 'meta.%'";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var name = dr.GetString(0);
                            var value = dr.GetString(1);

                            switch (name)
                            {
                                case Constants.MetaSummary:
                                    metadata.Summary = value;
                                    break;
                                case Constants.MetaRemarks:
                                    metadata.Remarks = value;
                                    break;
                                case Constants.MetaUrl:
                                    metadata.Url = value;
                                    break;
                                case Constants.MetaIcon:
                                    metadata.Icon = value;
                                    break;
                                case Constants.MetaDocPage:
                                    metadata.DocPage = value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads statistics of a table.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override TableStatistics LoadTableStatistics(TableOrView databaseObject)
        {
            var sql = @"
-- Data space
SELECT SUM(a.total_pages), SUM(a.used_pages), SUM(a.data_pages)
FROM sys.objects o
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.indexes i ON i.object_id = o.object_id AND i.type IN (0, 1)  -- heap and clustered
INNER JOIN sys.partitions p	ON p.object_id = i.object_id AND p.index_id = i.index_id
INNER JOIN sys.allocation_units a ON a.container_id = p.hobt_id
WHERE s.name = @schemaName AND o.name = @objectName

-- Index space
SELECT SUM(a.total_pages), SUM(a.used_pages), SUM(a.data_pages)
FROM sys.objects o
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.indexes i ON i.object_id = o.object_id AND i.type IN (2)  -- non-clustered
INNER JOIN sys.partitions p	ON p.object_id = i.object_id AND p.index_id = i.index_id
INNER JOIN sys.allocation_units a ON a.container_id = p.hobt_id
WHERE s.name = @schemaName AND o.name = @objectName

-- Row count
SELECT SUM(p.rows)
FROM sys.objects o
INNER JOIN sys.schemas s ON s.schema_id = o.schema_id
INNER JOIN sys.indexes i ON i.object_id = o.object_id AND i.type IN (0, 1)
INNER JOIN sys.partitions p	ON p.object_id = i.object_id AND p.index_id = i.index_id
WHERE s.name = @schemaName AND o.name = @objectName
";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    SetSchemaNameParameter(cmd, databaseObject.SchemaName);
                    SetObjectNameParameter(cmd, databaseObject.ObjectName);

                    using (var dr = cmd.ExecuteReader())
                    {
                        var stat = new TableStatistics();

                        // Data space
                        dr.Read();
                        stat.DataSpace = dr.IsDBNull(0) ? 0L : dr.GetInt64(0) * 0x2000;    // 8K pages

                        // Index space
                        dr.NextResult();
                        dr.Read();
                        stat.IndexSpace = dr.IsDBNull(0) ? 0L : dr.GetInt64(0) * 0x2000;    // 8K pages

                        // Row count
                        dr.NextResult();
                        dr.Read();
                        stat.RowCount = dr.IsDBNull(0) ? 0L : dr.GetInt64(0);

                        return stat;
                    }
                }
            }
        }

        #endregion

        internal override void OnRenameObject(DatabaseObject databaseObject, string schemaName, string objectName)
        {
            EnsureMutable(databaseObject);
            EnsureSchemaValid(schemaName);

            // The stored procedure sp_name expects the old name
            // the the schema.objectname or objectname format.
            // No database name should be specified.

            // FullyQualifiedName cannot be used here because that contains DB name.
            string oldname;

            if (String.IsNullOrEmpty(databaseObject.SchemaName))
            {
                oldname = QuoteIdentifier(databaseObject.ObjectName);
            }
            else
            {
                oldname = String.Format(
                    "{0}.{1}",
                    QuoteIdentifier(databaseObject.SchemaName),
                    QuoteIdentifier(databaseObject.ObjectName));
            }

            // TODO: sp_rename only support renaming object, not moving between
            // schemas. Use ALTER SCHEMA here if schema name is to be changed.

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(@"sp_rename", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@objname", SqlDbType.NVarChar, 776).Value = oldname;
                    cmd.Parameters.Add("@newname", SqlDbType.NVarChar, 776).Value = objectName;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal override void OnDropObject(DatabaseObject databaseObject)
        {
            EnsureMutable(databaseObject);

            var sql = String.Format(@"
IF (OBJECT_ID('{1}') IS NOT NULL)
BEGIN
DROP {0} {1}
END",
                Constants.SqlServerObjectTypeNames[databaseObject.ObjectType],
                GetObjectFullyResolvedName(databaseObject));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal override void OnCreateTable(Table table, bool createPrimaryKey, bool createIndexes)
        {
            EnsureMutable(table);

            var codegen = new Jhu.Graywulf.Sql.CodeGeneration.SqlServer.SqlServerCodeGenerator();
            var sql = codegen.GenerateCreateTableScript(table, createPrimaryKey, createIndexes);

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>
        /// Do not use this function to build large indices.
        /// </remarks>
        internal override void OnCreateIndex(Index index)
        {
            EnsureMutable(index);

            string sql;
            var codegen = new Jhu.Graywulf.Sql.CodeGeneration.SqlServer.SqlServerCodeGenerator();

            if (index.IsPrimaryKey)
            {
                sql = codegen.GenerateCreatePrimaryKeyScript((TableOrView)index.DatabaseObject);
            }
            else
            {
                sql = codegen.GenerateCreateIndexScript(index);
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal override void OnDropIndex(Index index)
        {
            EnsureMutable(index);

            string sql;
            var codegen = new Jhu.Graywulf.Sql.CodeGeneration.SqlServer.SqlServerCodeGenerator();

            if (index.IsPrimaryKey)
            {
                sql = codegen.GenerateDropPrimaryKeyScript((TableOrView)index.DatabaseObject);
            }
            else
            {
                sql = codegen.GenerateDropIndexScript(index);
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal override void OnTruncateTable(Table table)
        {
            EnsureMutable(table);

            var sql = String.Format(
                "TRUNCATE TABLE {0}",
                GetObjectFullyResolvedName(table));

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string GetObjectTypeIdListString(DatabaseObjectType objectType)
        {
            string res = String.Empty;

            foreach (var t in Constants.SqlServerObjectTypeIds)
            {
                if (objectType.HasFlag(t.Key))
                {
                    res += String.Format(",'{0}'", Constants.SqlServerObjectTypeIds[t.Key]);
                }
            }

            return res.Substring(1);
        }

        public override string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            csb.IntegratedSecurity = integratedSecurity;
            csb.UserID = username;
            csb.Password = password;
            csb.Enlist = enlist;

            return csb.ConnectionString;
        }

        #region Data type mapping functions

        /// <summary>
        /// Creates a data type based on a schema table row
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected override DataType CreateDataType(DataRow dr)
        {
            Type type;
            string name;
            int length;
            byte precision, scale;
            bool isNullable;

            GetDataTypeDetails(dr, out type, out name, out length, out precision, out scale, out isNullable);

            return CreateDataType(name, length, scale, precision, isNullable);
        }

        protected override DataType CreateDataType(string name)
        {
            // Try to interpret as SQL Server type. Certain aliases are not in the
            // enum, so handle them separately.

            // TODO: this doesn't work with complex types and special types

            SqlDbType sqltype;
            if (Enum.TryParse<SqlDbType>(name, true, out sqltype))
            {
                // This can be interpreted as a SQL Server type
                return CreateDataType(sqltype);
            }
            else if (StringComparer.InvariantCultureIgnoreCase.Compare("numeric", name) == 0)
            {
                return CreateDataType(SqlDbType.Decimal);
            }
            else if (StringComparer.InvariantCultureIgnoreCase.Compare("sysname", name) == 0)
            {
                return CreateDataType(SqlDbType.NVarChar, 128, 0, 0, false);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static DataType CreateDataType(SqlDbType type)
        {
            DataType dt;

            switch (type)
            {
                case System.Data.SqlDbType.Bit:
                    dt = DataTypes.SqlBit;
                    break;
                case System.Data.SqlDbType.TinyInt:
                    dt = DataTypes.SqlTinyInt;
                    break;
                case System.Data.SqlDbType.SmallInt:
                    dt = DataTypes.SqlSmallInt;
                    break;
                case System.Data.SqlDbType.Int:
                    dt = DataTypes.SqlInt;
                    break;
                case System.Data.SqlDbType.BigInt:
                    dt = DataTypes.SqlBigInt;
                    break;
                case System.Data.SqlDbType.Real:
                    dt = DataTypes.SqlReal;
                    break;
                case System.Data.SqlDbType.Float:
                    dt = DataTypes.SqlFloat;
                    break;
                case System.Data.SqlDbType.Image:
                    dt = DataTypes.SqlImage;
                    break;
                case System.Data.SqlDbType.Binary:
                    dt = DataTypes.SqlBinary;
                    break;
                case System.Data.SqlDbType.VarBinary:
                    dt = DataTypes.SqlVarBinary;
                    break;
                case System.Data.SqlDbType.Text:
                    dt = DataTypes.SqlText;
                    break;
                case System.Data.SqlDbType.Char:
                    dt = DataTypes.SqlChar;
                    break;
                case System.Data.SqlDbType.VarChar:
                    dt = DataTypes.SqlVarChar;
                    break;
                case System.Data.SqlDbType.NText:
                    dt = DataTypes.SqlNText;
                    break;
                case System.Data.SqlDbType.NChar:
                    dt = DataTypes.SqlNChar;
                    break;
                case System.Data.SqlDbType.NVarChar:
                    dt = DataTypes.SqlNVarChar;
                    break;
                case System.Data.SqlDbType.Date:
                    dt = DataTypes.SqlDate;
                    break;
                case System.Data.SqlDbType.DateTime:
                    dt = DataTypes.SqlDateTime;
                    break;
                case System.Data.SqlDbType.DateTime2:
                    dt = DataTypes.SqlDateTime2;
                    break;
                case System.Data.SqlDbType.DateTimeOffset:
                    dt = DataTypes.SqlDateTimeOffset;
                    break;
                case System.Data.SqlDbType.SmallDateTime:
                    dt = DataTypes.SqlSmallDateTime;
                    break;
                case System.Data.SqlDbType.Time:
                    dt = DataTypes.SqlTime;
                    break;
                case System.Data.SqlDbType.Timestamp:
                    dt = DataTypes.SqlTimestamp;
                    break;
                case System.Data.SqlDbType.Decimal:
                    dt = DataTypes.SqlDecimal;
                    break;
                case System.Data.SqlDbType.SmallMoney:
                    dt = DataTypes.SqlSmallMoney;
                    break;
                case System.Data.SqlDbType.Money:
                    dt = DataTypes.SqlMoney;
                    break;
                case System.Data.SqlDbType.UniqueIdentifier:
                    dt = DataTypes.SqlUniqueIdentifier;
                    break;
                case System.Data.SqlDbType.Variant:
                    dt = DataTypes.SqlVariant;
                    break;
                case System.Data.SqlDbType.Xml:
                    dt = DataTypes.SqlXml;
                    break;
                case System.Data.SqlDbType.Structured:
                    throw new NotImplementedException();
                case System.Data.SqlDbType.Udt:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            return dt;
        }

        public static DataType CreateDataType(SqlDbType type, int length, byte precision, byte scale, bool isNullable)
        {
            var dt = CreateDataType(type);

            if (dt.HasLength)
            {
                dt.Length = length;
            }

            if (dt.HasPrecision)
            {
                dt.Precision = precision;
            }

            if (dt.HasScale)
            {
                dt.Scale = scale;
            }

            dt.IsNullable = isNullable;

            return dt;
        }

        public static DataType CreateDataType(smo::DataType type)
        {
            DataType dt = null;

            switch (type.SqlDataType)
            {
                case smo.SqlDataType.BigInt:
                    dt = DataTypes.SqlBigInt;
                    break;
                case smo.SqlDataType.Binary:
                    dt = DataTypes.SqlBinary;
                    break;
                case smo.SqlDataType.Bit:
                    dt = DataTypes.SqlBit;
                    break;
                case smo.SqlDataType.Char:
                    dt = DataTypes.SqlChar;
                    break;
                case smo.SqlDataType.Date:
                    dt = DataTypes.SqlDate;
                    break;
                case smo.SqlDataType.DateTime:
                    dt = DataTypes.SqlDateTime;
                    break;
                case smo.SqlDataType.DateTime2:
                    dt = DataTypes.SqlDateTime2;
                    break;
                case smo.SqlDataType.DateTimeOffset:
                    dt = DataTypes.DateTimeOffset;
                    break;
                case smo.SqlDataType.Decimal:
                    dt = DataTypes.SqlDecimal;
                    break;
                case smo.SqlDataType.Float:
                    dt = DataTypes.SqlFloat;
                    break;
                case smo.SqlDataType.Geography:
                case smo.SqlDataType.Geometry:
                case smo.SqlDataType.HierarchyId:
                    throw new NotImplementedException();
                case smo.SqlDataType.Image:
                    dt = DataTypes.SqlImage;
                    break;
                case smo.SqlDataType.Int:
                    dt = DataTypes.SqlInt;
                    break;
                case smo.SqlDataType.Money:
                    dt = DataTypes.SqlMoney;
                    break;
                case smo.SqlDataType.NChar:
                    dt = DataTypes.SqlNChar;
                    break;
                case smo.SqlDataType.None:
                    throw new NotImplementedException();
                case smo.SqlDataType.NText:
                    dt = DataTypes.SqlNText;
                    break;
                case smo.SqlDataType.Numeric:
                    dt = DataTypes.SqlNumeric;
                    break;
                case smo.SqlDataType.NVarChar:
                    dt = DataTypes.SqlNVarChar;
                    break;
                case smo.SqlDataType.NVarCharMax:
                    dt = DataTypes.SqlNVarCharMax;
                    break;
                case smo.SqlDataType.Real:
                    dt = DataTypes.SqlReal;
                    break;
                case smo.SqlDataType.SmallDateTime:
                    dt = DataTypes.SqlSmallDateTime;
                    break;
                case smo.SqlDataType.SmallInt:
                    dt = DataTypes.SqlSmallInt;
                    break;
                case smo.SqlDataType.SmallMoney:
                    dt = DataTypes.SqlSmallMoney;
                    break;
                case smo.SqlDataType.SysName:
                    throw new NotImplementedException();
                case smo.SqlDataType.Text:
                    dt = DataTypes.SqlText;
                    break;
                case smo.SqlDataType.Time:
                    dt = DataTypes.SqlTime;
                    break;
                case smo.SqlDataType.Timestamp:
                    dt = DataTypes.SqlTimestamp;
                    break;
                case smo.SqlDataType.TinyInt:
                    dt = DataTypes.SqlTinyInt;
                    break;
                case smo.SqlDataType.UniqueIdentifier:
                    dt = DataTypes.SqlUniqueIdentifier;
                    break;
                case smo.SqlDataType.UserDefinedDataType:
                case smo.SqlDataType.UserDefinedTableType:
                case smo.SqlDataType.UserDefinedType:
                    throw new NotImplementedException();
                case smo.SqlDataType.VarBinary:
                    dt = DataTypes.SqlVarBinary;
                    break;
                case smo.SqlDataType.VarBinaryMax:
                    dt = DataTypes.SqlVarBinaryMax;
                    break;
                case smo.SqlDataType.VarChar:
                    dt = DataTypes.SqlVarChar;
                    break;
                case smo.SqlDataType.VarCharMax:
                    dt = DataTypes.SqlVarCharMax;
                    break;
                case smo.SqlDataType.Variant:
                    dt = DataTypes.SqlVariant;
                    break;
                case smo.SqlDataType.Xml:
                    dt = DataTypes.SqlXml;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!dt.IsMaxLength && dt.HasLength)
            {
                dt.Length = type.MaximumLength;
            }

            if (dt.HasScale)
            {
                dt.Scale = (byte)type.NumericScale;
            }

            if (dt.HasPrecision)
            {
                dt.Precision = (byte)type.NumericPrecision;
            }

            return dt;
        }

        #endregion

        private SqlConnection OpenConnectionInternal()
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);
            csb.Enlist = false;

            var cn = new SqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
        }

        public override IDbConnection OpenConnection()
        {
            return OpenConnectionInternal();
        }

    }
}
