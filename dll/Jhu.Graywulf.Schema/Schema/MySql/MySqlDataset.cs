using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema.MySql
{
    /// <summary>
    /// Implements schema reflection functions for MYSQL
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class MySqlDataset : DatasetBase
    {
        [IgnoreDataMember]
        public override string ProviderName
        {
            get { return Constants.MySqlProviderName; }
        }

        /// <summary>
        /// Gets or sets the database name associated with this dataset.
        /// </summary>
        [IgnoreDataMember]
        public override string DatabaseName
        {
            get
            {
                var csb = new MySqlConnectionStringBuilder(ConnectionString);
                return csb.Database;
            }
            set
            {
                var csb = new MySqlConnectionStringBuilder(ConnectionString);
                csb.Database = value;
                ConnectionString = csb.ConnectionString;
            }
        }

        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public MySqlDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public MySqlDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        public MySqlDataset(string name, string connectionString)
        {
            InitializeMembers(new StreamingContext());

            Name = name;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public MySqlDataset(MySqlDataset old)
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
        }

        /// <summary>
        /// Copies private member variables form another instance
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(MySqlDataset old)
        {
        }

        public override object Clone()
        {
            return new MySqlDataset(this);
        }

        #endregion
        #region Fully resolved names and keys

        protected override string QuoteIdentifier(string identifier)
        {
            return String.Format("`{0}`", identifier);
        }

        /// <summary>
        /// Returns the fully resolved name of a database object belonging to the dataset.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        /// <remarks>
        /// MySQL doesn't have the equivalent of schemas of SQL Server.
        /// </remarks>
        public override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            var databaseName = GetFullyResolvedName();
            var objectName = QuoteIdentifier(databaseObject.ObjectName);

            return String.Format("{0}.{1}", databaseName, objectName);
        }

        public override string GetObjectUniqueKey(DatabaseObjectType objectType, string datasetName, string databaseName, string schemaName, string objectName)
        {
            // MySQL doesn't have the equivalent of schemas of SQL Server.
            schemaName = String.Empty;

            return base.GetObjectUniqueKey(objectType, datasetName, databaseName, schemaName, objectName);
        }

        #endregion

        /// <summary>
        /// Loads the schema of a database object belonging to the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        protected override void LoadObject<T>(T obj)
        {
            string sql;

            switch (Schema.Constants.DatabaseObjectTypes[typeof(T)])
            {
                case DatabaseObjectType.Table:
                case DatabaseObjectType.View:
                    sql = @"
SELECT table_name, table_type
FROM information_schema.tables
WHERE table_schema LIKE @databaseName AND table_name LIKE @objectName AND table_type IN ({0});";
                    break;
                case DatabaseObjectType.StoredProcedure:
                case DatabaseObjectType.ScalarFunction:
                    sql = @"
SELECT routine_name  as `object_name`, routine_type  as `object_type`
FROM information_schema.routines
WHERE routine_schema LIKE @databaseName AND routine_name LIKE @objectName AND routine_type IN ({0});";
                    break;
                // TODO: add table-valued function
                default:
                    throw new NotImplementedException();
            }

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));
            using (MySqlConnection cn = OpenConnectionInternal())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {

                        int q = 0;
                        while (dr.Read())
                        {
                            obj.Dataset = this;
                            obj.DatabaseName = DatabaseName;
                            obj.SchemaName = String.Empty;
                            obj.ObjectName = dr.GetString(0);
                            obj.ObjectType = Constants.MySqlObjectTypeIds[dr.GetString(1).Trim()];

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
            var sql = @"
SELECT routine_name AS `object_name`, routine_type AS `object_type`
FROM information_schema.routines
WHERE routine_schema LIKE @databaseName AND routine_type IN({0})
UNION
SELECT table_name as `object_name`, table_type as `object_type`
FROM information_schema.tables
WHERE table_schema LIKE @databaseName AND table_type IN({0})";

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));

            using (MySqlConnection cn = OpenConnectionInternal())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = DatabaseName;

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            T obj = new T()
                            {
                                Dataset = this,
                                DatabaseName = DatabaseName,
                                SchemaName = String.Empty,
                                ObjectName = dr.GetString(0),
                                ObjectType = Constants.MySqlObjectTypeIds[dr.GetString(1).Trim()],
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

            foreach (var t in Constants.MySqlObjectTypeIds)
            {
                if (objectType.HasFlag(t.Key))
                {
                    res += String.Format(",'{0}'", Constants.MySqlObjectTypeIds[t.Key]);
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
            var sql = @"
SELECT ordinal_position,
       column_name,
       data_type,
       COALESCE(character_maximum_length, -1) AS `max_length`,
       COALESCE(numeric_scale, 0) AS `scale`,
       COALESCE(numeric_precision, 0) AS `precision`,
       is_nullable
FROM information_schema.columns
WHERE table_schema LIKE @databaseName AND table_name LIKE @tableName;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@tableName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Column cd = new Column(obj)
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                            };

                            cd.DataType = CreateDataType(
                                dr.GetString(2),
                                dr.GetInt64(3) > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(dr.GetInt64(3)),
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
            var sql = @"  
SELECT s.index_name, s.non_unique
FROM information_schema.statistics s
WHERE s.table_schema LIKE @databaseName AND s.table_name LIKE @objectName
GROUP BY 1;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        int q = 0;
                        while (dr.Read())
                        {
                            // Primary keep is the same as clustered index
                            // http://dev.mysql.com/doc/refman/5.0/en/innodb-index-types.html
                            var primary = StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(0), "primary") == 0;

                            var idx = new Index((TableOrView)obj)
                            {
                                IndexId = q++,
                                IndexName = dr.GetString(0),
                                IsUnique = !dr.GetBoolean(1),
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
SELECT c.column_name,
       kcu.ordinal_position,
       t.auto_increment,
       c.data_type,
       COALESCE(character_maximum_length, -1) AS `max_length`,
       COALESCE(numeric_scale, -1) AS `scale`,
       COALESCE(numeric_precision, -1) AS `precision`,
       c.is_nullable
FROM information_schema.key_column_usage kcu 
INNER JOIN information_schema.columns c ON kcu.table_schema = c.table_schema AND kcu.table_name = c.table_name AND kcu.column_name = c.column_name
INNER JOIN information_schema.tables t ON kcu.table_schema = t.table_schema AND kcu.table_name = t.table_name
WHERE t.table_schema LIKE @databaseName AND kcu.table_name LIKE @tableName AND kcu.constraint_name LIKE @indexName;";
            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = index.DatabaseName;
                    cmd.Parameters.Add("@tableName", MySqlDbType.VarChar, 128).Value = index.TableOrView.ObjectName;
                    cmd.Parameters.Add("@indexName", MySqlDbType.VarChar, 128).Value = index.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var nullable = (StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(7), "yes") == 0);

                            var ic = new IndexColumn()
                            {
                                ID = 0,
                                Name = dr.GetString(0),
                                KeyOrdinal = dr.GetInt32(1),
                                Ordering = IndexColumnOrdering.Ascending,
                                IsIdentity = dr.IsDBNull(7) ? false : true
                            };

                            ic.DataType = CreateDataType(
                                dr.GetString(3),
                                dr.GetInt64(4) > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(dr.GetInt64(4)),
                                Convert.ToByte(dr.GetValue(5)),
                                Convert.ToByte(dr.GetValue(6)),
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
       COALESCE(p.parameter_name, 'RETVAL'),
	   p.parameter_mode,
	   p.data_type,
       COALESCE(p.character_maximum_length, -1),
       COALESCE(p.numeric_scale, 0),
       COALESCE(p.numeric_precision, 0)
FROM information_schema.parameters p
WHERE p.specific_schema LIKE @databaseName AND p.specific_name = @objectName
ORDER BY 1;";

            sql = String.Format(sql, DatabaseObjectType.Table);
            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;
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
                                Direction = dir,
                                HasDefaultValue = false,
                                DefaultValue = null,
                            };

                            par.DataType = CreateDataType(
                                dr.GetString(3),
                                Convert.ToInt32(dr.GetValue(4)),
                                Convert.ToByte(dr.GetValue(5)),
                                Convert.ToByte(dr.GetValue(6)),
                                false);

                            yield return new KeyValuePair<string, Parameter>(par.Name, par);
                        }
                    }
                }
            }
        }

        internal protected override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT table_comment comment 
FROM information_schema.tables t
WHERE t.table_schema LIKE @databaseName AND t.table_name LIKE @objectName
UNION
SELECT routine_comment comment
FROM information_schema.routines r
WHERE r.routine_schema LIKE @databaseName AND r.routine_name LIKE @objectName ;";

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar).Value = databaseObject.DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar).Value = databaseObject.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        var meta = new DatabaseObjectMetadata();

                        while (dr.Read())
                        {
                            meta.Summary = dr.GetString(0);
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
SELECT column_name, column_comment
FROM information_schema.columns 
WHERE table_schema LIKE @databaseName AND table_name LIKE @objectName ;";

            LoadAllVariableMetadata(sql, databaseObject, ((IColumns)databaseObject).Columns);
        }

        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT p.parameter_name, r.routine_comment
FROM information_schema.routines r
INNER JOIN information_schema.parameters p ON p.specific_name = r.routine_name
WHERE r.routine_schema LIKE @databaseName AND r.routine_name LIKE @objectName ;";

            LoadAllVariableMetadata(sql, databaseObject, ((IParameters)databaseObject).Parameters);
        }

        private void LoadAllVariableMetadata(string sql, DatabaseObject databaseObject, IDictionary variables)
        {
            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnectionInternal())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar).Value = databaseObject.DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar).Value = databaseObject.ObjectName;

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

        internal override TableStatistics LoadTableStatistics(TableOrView tableOrView)
        {
            throw new NotImplementedException();
        }

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
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@objtype", MySqlDbType.VarChar, 776).Value = obj.GetType().ToString();
                    cmd.Parameters.Add("@objoldname", MySqlDbType.VarChar, 776).Value = oldname;
                    cmd.Parameters.Add("@objnewname", MySqlDbType.VarChar, 776).Value = objectName;

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
                Constants.MySqlObjectTypeNames[obj.ObjectType],
                obj.GetFullyResolvedName());

            using (var cn = OpenConnection())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }*/
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
            var csb = new MySqlConnectionStringBuilder(connectionString);
            csb.IntegratedSecurity = integratedSecurity;
            csb.UserID = username;
            csb.Password = password;
            csb.AutoEnlist = enlist;

            return csb.ConnectionString;
        }

        #region Type conversion functions

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
                case Constants.TypeNameTinyInt:
                    return DataTypes.SqlTinyInt;
                case Constants.TypeNameSmallInt:
                    return DataTypes.SqlSmallInt;
                case Constants.TypeNameInt:
                    return DataTypes.SqlInt;
                case Constants.TypeNameMediumInt:
                    return DataTypes.SqlBigInt;
                case Constants.TypeNameBigInt:
                    return DataTypes.SqlBigInt;
                case Constants.TypeNameFloat:
                    return DataTypes.SqlFloat;
                case Constants.TypeNameDouble:
                    return DataTypes.SqlReal;
                case Constants.TypeNameDecimal:
                    return DataTypes.SqlDecimal;
                case Constants.TypeNameDate:
                    return DataTypes.SqlDate;
                case Constants.TypeNameYear:
                    return DataTypes.SqlTinyInt;
                case Constants.TypeNameTime:
                    return DataTypes.SqlTime;
                case Constants.TypeNameDateTime:
                    return DataTypes.SqlDateTime;
                case Constants.TypeNameTimestamp:
                    return DataTypes.SqlTimestamp;
                case Constants.TypeNameTinyText:
                    return DataTypes.SqlText;
                case Constants.TypeNameText:
                    return DataTypes.SqlText;
                case Constants.TypeNameMediumText:
                    return DataTypes.SqlText;
                case Constants.TypeNameLongText:
                    return DataTypes.SqlText;
                case Constants.TypeNameTinyBlob:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameBlob:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameMediumBlob:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameLongBlob:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameBit:
                    return DataTypes.SqlBit;
                case Constants.TypeNameSet:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameEnum:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameBinary:
                    return DataTypes.SqlBinary;
                case Constants.TypeNameVarBinary:
                    return DataTypes.SqlVarBinary;
                case Constants.TypeNameGeometry:
                    return DataTypes.SqlNVarChar;
                case Constants.TypeNameChar:
                    return DataTypes.SqlChar;
                case Constants.TypeNameVarChar:
                    return DataTypes.SqlVarChar;
                case Constants.TypeNameXml:
                    return DataTypes.SqlXml;
                default:
                    return base.CreateDataType(name);
            }
        }

        #endregion

        private MySqlConnection OpenConnectionInternal()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString);
            csb.AutoEnlist = false;

            var cn = new MySqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
        }

        /// <summary>
        /// Opens a connection to the MySQL database
        /// </summary>
        /// <returns></returns>
        public override IDbConnection OpenConnection()
        {
            return OpenConnectionInternal();
        }
    }
}
