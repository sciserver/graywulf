using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using Npgsql;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema.PostgreSql
{ /// <summary>
    /// Implements schema reflection functions for Npgsql
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class PostgreSqlDataset : DatasetBase
    {
        #region Properties

        [IgnoreDataMember]
        public override string ProviderName
        {
            get { return Constants.PostgreSqlProviderName; }
        }

        /// <summary>
        /// Gets or sets the database name associated with this dataset.
        /// </summary>
        [IgnoreDataMember]
        public override string DatabaseName
        {
            get
            {
                var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
                return csb.Database;
            }
            set
            {
                var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
                csb.Database = value;
                ConnectionString = csb.ConnectionString;
            }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public PostgreSqlDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public PostgreSqlDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        public PostgreSqlDataset(string name, string connectionString)
        {
            InitializeMembers(new StreamingContext());

            Name = name;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public PostgreSqlDataset(PostgreSqlDataset old)
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
            this.DefaultSchemaName = "public";
        }

        /// <summary>
        /// Copies private member variables form another instance
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(PostgreSqlDataset old)
        {
        }

        public override object Clone()
        {
            return new PostgreSqlDataset(this);
        }

        #endregion
        #region Fully resolved names and keys

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("\"{0}\"", identifier);
        }

        /// <summary>
        /// Returns the fully resolved name of a database object belonging to the dataset.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        public override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            // If schema name is empty, use default schema name

            var databaseName = GetFullyResolvedName();
            var schemaName =
                String.IsNullOrWhiteSpace(databaseObject.SchemaName) ?
                    QuoteIdentifier(DefaultSchemaName) :
                    QuoteIdentifier(databaseObject.SchemaName);
            var objectName = databaseObject.ObjectName;

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

        /// <summary>
        /// Loads the schema of a database object belonging to the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        protected override void LoadObject<T>(T obj)
        {
            var sql = String.Empty;
            switch (Schema.Constants.DatabaseObjectTypes[typeof(T)])
            {
                case DatabaseObjectType.Table:
                case DatabaseObjectType.View:
                    sql = @"
SELECT table_schema,table_name, table_type
FROM information_schema.tables
WHERE table_type IN ({0}) AND
      table_catalog ILIKE @databaseName AND table_schema ILIKE @schemaName AND table_name ILIKE @objectName;";
                    break;
                case DatabaseObjectType.StoredProcedure:
                    sql = @"
SELECT routine_schema, routine_name, routine_type
FROM information_schema.routines
WHERE routine_type IN ({0}) AND
      routine_catalog ILIKE @databaseName AND routine_schema ILIKE @schemaName AND routine_name ILIKE @objectName;";
                    break;
                case DatabaseObjectType.ScalarFunction:
                case DatabaseObjectType.TableValuedFunction:
                    throw new SchemaException("PostgreSQL supports stored procedures only.");
                default:
                    // TODO: add scalar and table-valued functions
                    throw new NotImplementedException();
            }

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));
            
            using (NpgsqlConnection cn = OpenConnectionInternal())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {

                        int q = 0;
                        while (dr.Read())
                        {
                            obj.Dataset = this;
                            obj.DatabaseName = DatabaseName;
                            obj.SchemaName = dr.GetString(0);
                            obj.ObjectName = dr.GetString(1);
                            obj.ObjectType = Constants.PostgreSqlObjectTypeIds[dr.GetString(2).Trim()];

                            q++;
                        }

                        // No records
                        if (q == 0)
                        {
                            ThrowInvalidObjectNameException(obj);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads all objects of a certain kind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected override IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>()
        {
            string sql = @"
SELECT routine_name, routine_type, routine_schema
FROM information_schema.routines
WHERE routine_type IN({0}) AND specific_schema NOT IN ('information_schema', 'pg_catalog') AND
      specific_catalog = @databaseName

UNION

SELECT table_name, table_type, table_schema
FROM information_schema.tables
WHERE table_type IN ({0}) AND table_schema NOT IN ('information_schema', 'pg_catalog') AND
      table_catalog = @databaseName;";

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));

            using (NpgsqlConnection cn = OpenConnectionInternal())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = DatabaseName;

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            T obj = new T()
                            {
                                Dataset = this,
                                DatabaseName = DatabaseName,
                                SchemaName = dr.GetString(2),
                                ObjectName = dr.GetString(0),
                                ObjectType = Constants.PostgreSqlObjectTypeIds[dr.GetString(1).Trim()],
                            };

                            yield return new KeyValuePair<string, T>(GetObjectUniqueKey(obj), obj);
                        }
                    }
                }
            }
        }

        private string GetObjectTypeIdListString(DatabaseObjectType objectType)
        {
            string res = String.Empty;

            foreach (var t in Constants.PostgreSqlObjectTypeIds)
            {
                if (objectType.HasFlag(t.Key))
                {
                    res += String.Format(",'{0}'", Constants.PostgreSqlObjectTypeIds[t.Key]);
                }
            }

            return res.Substring(1);
        }

        /// <summary>
        /// Loads columns of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, Column>> LoadColumns(DatabaseObject obj)
        {
            string sql = @"
SELECT ordinal_position, 
       column_name, 
       udt_name, 
       COALESCE(character_maximum_length, -1) AS ""max_length"",
       COALESCE(numeric_scale, 0) AS ""scale"",
       COALESCE(numeric_precision, 0) AS ""precision"",
       is_nullable
FROM information_schema.columns
WHERE table_catalog ILIKE @databaseName AND table_name ILIKE @tableName AND table_schema ILIKE @schemaName;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@tableName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var cd = new Column(obj)
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                            };

                            cd.DataType = CreateDataType(
                                dr.GetString(2),
                                Convert.ToInt32(dr.GetValue(3)),
                                Convert.ToByte(dr.GetValue(4)),
                                Convert.ToByte(dr.GetValue(5)),
                                (StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(6), "yes") == 0));

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
        internal override IEnumerable<KeyValuePair<string, Index>> LoadIndexes(DatabaseObject obj)
        {
            // TODO: this is not perfect here, it returns all indices (including primary keys, but
            // not constraints), there is, however, no way to tell the type of the index
            // Also, querying index columns in only possible for keys and not for ordinary indices.

            var sql = @"  
SELECT indexname, indexdef
FROM pg_catalog.pg_indexes 
WHERE schemaname ILIKE @schemaName AND tablename ILIKE @objectName;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        int q = 0;
                        while (dr.Read())
                        {
                            var primary = false;    // No way to tell from this query
                            var unique = dr.GetString(1).StartsWith("CREATE UNIQUE");

                            var idx = new Index((TableOrView)obj)
                            {
                                IndexId = q++,
                                IndexName = dr.GetString(0),
                                IsUnique = primary | unique,
                                IsPrimaryKey = primary,
                                IsClustered = primary
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
            var sql = @"
SELECT 
	kcu.column_name,
	kcu.ordinal_position,
	c.is_nullable,
	c.is_identity,
	c.udt_name,
	COALESCE(c.character_maximum_length, -1),
	COALESCE(c.numeric_scale, -1),
	COALESCE(c.numeric_precision, -1)
FROM information_schema.key_column_usage  kcu 
INNER JOIN information_schema.columns c ON kcu.table_name = c.table_name AND kcu.column_name=c.column_name
WHERE constraint_catalog ILIKE @databaseName AND constraint_schema ILIKE @schemaName AND kcu.table_name ILIKE @objectName AND constraint_name ILIKE @indexName;";
            
            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = index.TableOrView.DatabaseName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = index.TableOrView.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = index.TableOrView.ObjectName;
                    cmd.Parameters.Add("@indexName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = index.IndexName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var nullable = (StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(2), "yes") == 0);
                            var identity = (StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(3), "yes") == 0);

                            var ic = new IndexColumn()
                            {
                                ID = 0,
                                Name = dr.GetString(0),
                                KeyOrdinal = dr.GetInt32(1),
                                IsIdentity = identity,
                                Ordering = IndexColumnOrdering.Ascending
                            };

                            ic.DataType = CreateDataType(
                                dr.GetString(4),
                                Convert.ToInt32(dr.GetValue(5)),
                                Convert.ToByte(dr.GetValue(6)),
                                Convert.ToByte(dr.GetValue(7)),
                                nullable);

                            yield return new KeyValuePair<string, IndexColumn>(ic.Name, ic);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads parameters of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override IEnumerable<KeyValuePair<string, Parameter>> LoadParameters(DatabaseObject obj)
        {
            var sql = @"
SELECT p.ordinal_position,
       p.parameter_name,
       p.parameter_mode,
       p.udt_name,
       COALESCE(p.character_maximum_length, -1),
       COALESCE(p.numeric_scale, 0),
       COALESCE(p.numeric_precision, 0)
FROM information_schema.parameters p
INNER JOIN information_schema.routines r ON r.specific_catalog = p.specific_catalog AND r.specific_schema = p.specific_schema AND r.specific_name = p.specific_name
WHERE p.specific_catalog ILIKE @databaseName AND p.specific_schema ILIKE @schemaName AND r.routine_name = @objectName";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ParameterDirection dir;
                            if (dr.IsDBNull(2))
                            {
                                dir = ParameterDirection.ReturnValue;
                            }
                            else
                            {
                                switch (dr.GetString(2).ToLowerInvariant())
                                {
                                    case "in":
                                        dir = ParameterDirection.Input;
                                        break;
                                    case "out":
                                        dir = ParameterDirection.Output;
                                        break;
                                    case "inout":
                                        dir = ParameterDirection.InputOutput;
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }

                            var par = new Parameter(obj)
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                                HasDefaultValue = false,
                                DefaultValue = null,
                                Direction = dir,
                            };

                            par.DataType = CreateDataType(
                                dr.GetString(3),
                                Convert.ToInt32(dr.GetValue(4)),
                                Convert.ToByte(dr.GetValue(5)),
                                Convert.ToByte(dr.GetValue(6)),
                                false); // nullable is not supported by postgres functions

                            yield return new KeyValuePair<string, Parameter>(par.Name, par);
                        }
                    }
                }
            }
        }

        #endregion
        #region Metadata

        internal override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            var sql = @"SELECT DISTINCT
pg_catalog.obj_description(c.oid) AS table_comment,
c.relname AS table_name
FROM pg_class c
LEFT JOIN pg_attribute a ON a.attrelid = c.oid
LEFT JOIN pg_namespace d ON d.oid = c.relnamespace        
WHERE c.relname = @objectName AND d.nspname= @schemaName
;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        var meta = new DatabaseObjectMetadata();

                        while (dr.Read())
                        {
                            meta.Summary = dr.IsDBNull(0) ? "" : dr.GetString(0);
                        }

                        return meta;
                    }
                }
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

        protected override void LoadAllColumnMetadata(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT c.column_name,pgd.description
FROM pg_catalog.pg_statio_all_tables AS st
  INNER JOIN pg_catalog.pg_description pgd ON (pgd.objoid=st.relid)
  INNER JOIN information_schema.columns c ON (pgd.objsubid=c.ordinal_position AND  c.table_schema=@schemaName AND c.table_name=@objectName);";
            LoadAllVariableMetadata(sql, databaseObject, ((IColumns)databaseObject).Columns);
        }

        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            var sql = @"SELECT  pgd.description,proargnames
FROM pg_catalog.pg_namespace n
INNER JOIN pg_catalog.pg_proc p ON p.pronamespace = n.oid
INNER JOIN pg_catalog.pg_description pgd ON (pgd.objoid=p.oid)
WHERE nspname = @schemaName and proname= @objectName;";

            var variables = ((IParameters)databaseObject).Parameters;

            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        VariableMetadata meta = null;

                        while (dr.Read())
                        {
                            string value = dr.GetString(0);
                            var paramname = dr.GetValue(1) as string[];
                            if (paramname.Count() > 0)
                            {
                                var paramlist = paramname;

                                foreach (var v in paramlist)
                                {
                                    meta = (VariableMetadata)((Variable)variables[v]).Metadata;
                                    meta.Summary = value;
                                }
                            }


                        }
                    }
                }
            }
        }

        private void LoadAllVariableMetadata(string sql, DatabaseObject databaseObject, IDictionary variables)
        {
            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar).Value = databaseObject.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        string variablename = null;
                        VariableMetadata meta = null;

                        while (dr.Read())
                        {
                            string name = dr.GetString(0);
                            string value = dr.GetString(1);

                            if (name != variablename)
                            {
                                meta = (VariableMetadata)((Variable)variables[name]).Metadata;
                                variablename = name;
                            }
                            meta.Summary = value;
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

        internal override TableStatistics LoadTableStatistics(TableOrView tableOrView)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal override void RenameObject(DatabaseObject obj, string objectName)
        {
            throw new NotImplementedException();

            /*
            if (!IsMutable)
            {
                throw new InvalidOperationException();
            }

            var sql = @"RENAME @objtype @objoldname TO @objnewname;";
            string oldname = String.Empty;

            oldname = String.Format("[{0}].[{1}]", obj.DatabaseName, obj.ObjectName);
            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@objtype", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = obj.GetType().ToString();
                    cmd.Parameters.Add("@objoldname", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = oldname;
                    cmd.Parameters.Add("@objnewname", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = objectName;

                    cmd.ExecuteNonQuery();
                }
            }
            */
        }

        internal override void DropObject(DatabaseObject obj)
        {
            throw new NotImplementedException();

            /*
            if (!IsMutable)
            {
                throw new InvalidOperationException();
            }

            var sql = String.Format(
                "DROP {0} {1}",
                Constants.PostgreSqlObjectTypeNames[obj.ObjectType],
                obj.GetFullyResolvedName());

            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
             * */
        }

        internal override void CreateTable(Table table)
        {
            throw new NotImplementedException();
        }

        internal override void TruncateTable(Table table)
        {
            throw new NotImplementedException();
        }

        protected override DatasetStatistics LoadDatasetStatistics()
        {
            throw new NotImplementedException();
        }

        protected override DatasetMetadata LoadDatasetMetadata()
        {
            // *** TODO: implement
            // Where to get metadata from? Registry?
            return new DatasetMetadata();
        }

        public override string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist)
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(connectionString);
            csb.IntegratedSecurity = integratedSecurity;
            csb.UserName = username;
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
            switch (name.ToLowerInvariant().Trim())
            {
                case Constants.TypeNameOidVector:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameRefCursor:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameChar:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameBpChar:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameVarChar:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameText:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameName:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameBytea:
                    return DataTypes.SqlBinary;
                case Constants.TypeNameBit:
                    return DataTypes.SqlBit;       //  need to check is it correct
                case Constants.TypeNameVarBit:
                    return DataTypes.SqlBit;       //  need to check is it correct, at documentation its an object
                case Constants.TypeNameBoolean:
                    return DataTypes.SqlBit;
                case Constants.TypeNameInt16:
                    return DataTypes.SqlSmallInt;
                case Constants.TypeNameInt32:
                    return DataTypes.SqlInt;
                case Constants.TypeNameInt64:
                    return DataTypes.SqlBigInt;
                case Constants.TypeNameOid:
                    return DataTypes.SqlBigInt;
                case Constants.TypeNameReal:
                    return DataTypes.SqlReal;
                case Constants.TypeNameDouble:
                    return DataTypes.SqlReal;
                case Constants.TypeNameNumeric:
                    return DataTypes.SqlDecimal;
                case Constants.TypeNameInet:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameMacaddr:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameMoney:
                    return DataTypes.SqlMoney;
                case Constants.TypeNamePoint:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameLine:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameLseg:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNamePath:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameBox:
                    return DataTypes.SqlNVarChar;       //  it's an object, need to check
                case Constants.TypeNameCircle:
                    return DataTypes.SqlNVarChar;        //  it's an object, need to check
                case Constants.TypeNamePolygon:
                    return DataTypes.SqlNVarChar;        //  it's an object, need to check
                case Constants.TypeNameUuid:
                    return DataTypes.SqlNVarChar;        //  it's an object, need to check
                case Constants.TypeNameXml:
                    return DataTypes.SqlXml;
                case Constants.TypeNameInterval:
                    return DataTypes.SqlNVarChar;        //  it's an object, need to check
                case Constants.TypeNameDate:
                    return DataTypes.SqlDate;
                case Constants.TypeNameTime:
                    return DataTypes.SqlTime;
                case Constants.TypeNameTimeWithTimeZone:
                    return DataTypes.SqlTime;
                case Constants.TypeNameTimestamp:
                    return DataTypes.SqlDateTime;
                case Constants.TypeNameTimestampWithTimeZone:
                    return DataTypes.SqlDateTime;
                case Constants.TypeNameTsRange:
                    return DataTypes.SqlDateTime;       //check is it correct
                case Constants.TypeNameTstzRange:
                    return DataTypes.SqlDateTime;       //check is it correct
                case Constants.TypeNameDateRange:
                    return DataTypes.SqlDate;           //check is it correct
                case Constants.TypeNameNumRange:
                    return DataTypes.SqlNumeric;        //check is it correct
                case Constants.TypeNameInt4Range:
                    return DataTypes.SqlInt;            //check is it correct
                case Constants.TypeNameInt8Range:
                    return DataTypes.SqlBigInt;        //check is it correct
                case Constants.TypeNameJson:
                    return DataTypes.SqlNVarChar;       //check is it correct
                case Constants.TypeNameTsVector:
                    return DataTypes.SqlNVarChar;       //check is it correct
                case Constants.TypeNameCidr:
                    return DataTypes.SqlNVarChar;       //check is it correct
                case Constants.TypeNameCString:
                    return DataTypes.SqlNVarChar;       //check is it correct
                default:
                    return base.CreateDataType(name);
            }
        }

        #endregion

        public override SqlCodeGen.SqlCodeGeneratorBase CreateCodeGenerator()
        {
            return new SqlCodeGen.PostgreSql.PostgreSqlCodeGenerator();
        }

        private NpgsqlConnection OpenConnectionInternal()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            csb.Enlist = false;

            var cn = new NpgsqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
        }

        /// <summary>
        /// Opens a connection to the Npgsql database
        /// </summary>
        /// <returns></returns>
        public override  IDbConnection OpenConnection()
        {
            return OpenConnectionInternal();
        }
    }
}

