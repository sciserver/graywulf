using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;

namespace Jhu.Graywulf.Schema.MySql
{
    /// <summary>
    /// Implements schema reflection functions for MYSQL
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class MySqlDataset : DatasetBase
    {
        protected bool isOnLinkedServer;
        protected bool isRemoteDataset;
        /// <summary>
        /// Gets or sets the value determining if the data is available
        /// via a linked MYSQL.
        /// </summary>
        [DataMember]
        public bool IsOnLinkedServer
        {
            get { return isOnLinkedServer; }
            set { isOnLinkedServer = value; }
        }

        [DataMember]
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
            this.isOnLinkedServer = false;
            this.isRemoteDataset = false;
        }

        /// <summary>
        /// Copies private member variables form another instance
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(MySqlDataset old)
        {
            this.isOnLinkedServer = old.isOnLinkedServer;
            this.isRemoteDataset = old.isRemoteDataset;
        }

        public override object Clone()
        {
            return new MySqlDataset(this);
        }

        #endregion

        /// <summary>
        /// Returns the fully resolved name of the dataset.
        /// </summary>
        /// <returns></returns>
        public override string GetFullyResolvedName()
        {
            return String.Format("[{0}]", DatabaseName);
        }

        /// <summary>
        /// Returns the fully resolved name of a database object belonging to the dataset.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            string format = "{0}..[{1}]";

            return String.Format(format, this.GetFullyResolvedName(), databaseObject.ObjectName);
        }

        /// <summary>
        /// Loads the schema of a database object belonging to the dataset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        protected override void LoadObject<T>(T obj)
        {
            // TODO: rewrite this query to conform with mysql
            var sql = String.Empty;
            switch (Schema.Constants.DatabaseObjectTypes[typeof(T)])
            {
                case DatabaseObjectType.Table:
                    sql = @"
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @database AND TABLE_TYPE IN ({0}) AND TABLE_NAME = @objectName;";
                    break;
                case DatabaseObjectType.View:
                    sql = @"
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @database AND TABLE_TYPE IN ({0}) AND TABLE_NAME = @objectName;";
                    break;
                case DatabaseObjectType.StoredProcedure:
                    sql = @"
SELECT 
INFORMATION_SCHEMA.ROUTINES.ROUTINE_NAME  as `object_name`,
INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE  as `object_type`
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_SCHEMA = @database
AND INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE IN ({0});";
                    break;
                case DatabaseObjectType.ScalarFunction:
                    sql = @"
SELECT 
INFORMATION_SCHEMA.ROUTINES.ROUTINE_NAME,
INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_SCHEMA = @database
AND INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE IN ({0});";
                    break;
            }

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));
            using (MySqlConnection cn = OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@database", MySqlDbType.VarChar, 128).Value = DatabaseName;
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


        /// <summary>
        /// Loads all objects of a certain kind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected override IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>(string databaseName)
        {
            var sql = @"
SELECT 
    INFORMATION_SCHEMA.ROUTINES.ROUTINE_NAME as `object_name`,
    INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE as `object_type`
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE INFORMATION_SCHEMA.ROUTINES.ROUTINE_SCHEMA = @database
AND INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE IN({0})
UNION
SELECT 
    INFORMATION_SCHEMA.TABLES.TABLE_NAME as `object_name`
    ,INFORMATION_SCHEMA.TABLES.TABLE_TYPE as `object_type`
FROM   INFORMATION_SCHEMA.TABLES
WHERE INFORMATION_SCHEMA.TABLES.TABLE_SCHEMA = @database 
AND INFORMATION_SCHEMA.TABLES.TABLE_TYPE IN({0})";

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));

            using (MySqlConnection cn = OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@database", MySqlDbType.VarChar, 128).Value = DatabaseName;

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

                            yield return new KeyValuePair<string, T>(obj.ObjectKey, obj);
                        }
                    }
                }
            }
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
        public override string GetObjectKeyFromParts(DatabaseObjectType objectType, string datasetName, string databaseName, string schemaName, string objectName)
        {
            return String.Format("{0}|{1}|{2}|{3}|{4}", objectType, datasetName, databaseName, schemaName, objectName);
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
        //TODO SIZE
        /// <summary>
        /// Loads columns of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, Column> LoadColumns(DatabaseObject obj)
        {
            var res = new ConcurrentDictionary<string, Column>();

            var sql = @"
SELECT ordinal_position, column_name, data_type
FROM information_schema.columns
WHERE table_schema = @databaseName and table_name= @tableName;";

            using (var cn = OpenConnection())
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
                                DataType = DataType.GetMySqlType(dr.GetString(2)),       // TODO: implement mapping to SQL Server types here
                            };

                            res.TryAdd(cd.Name, cd);
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Loads indexes of a database object.
        /// </summary>
        /// <param name="databaseObject"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, Index> LoadIndexes(DatabaseObject obj)
        {
            var res = new ConcurrentDictionary<string, Index>();
            var sql = @"  
SELECT  c.ordinal_position,
        s.index_name,
		s.index_type,
		s.non_unique,
        s.column_name,
        s.table_name
FROM information_schema.statistics s
INNER JOIN information_schema.columns c
   ON s.TABLE_NAME=c.TABLE_NAME
WHERE s.table_schema =  @schemaName and s.TABLE_NAME=@objectName
GROUP BY 1,2;";

            using (var cn = OpenConnection())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", MySqlDbType.VarChar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@objectName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var idx = new Index((TableOrView)obj);
                            if (dr.GetString(1).ToUpper() == "PRIMARY")
                            {
                                idx.IsPrimaryKey = true;
                                idx.IndexId = dr.GetInt32(0);
                                idx.IsClustered = true;       //http://dev.mysql.com/doc/refman/5.0/en/innodb-index-types.html
                                idx.IsUnique = dr.GetBoolean(3);
                                idx.IndexName = "PK_" + dr.GetString(5);
                            }
                            else
                            {
                                idx.IsPrimaryKey = false;
                                idx.IndexId = dr.GetInt32(0);
                                idx.IndexName = dr.GetString(1);
                                idx.IsClustered = false;
                                idx.IsUnique = dr.GetBoolean(3);
                            }

                            res.TryAdd(idx.IndexName, idx);
                        }
                    }
                }
            }

            return res;
        }
        //TODO SIZE
        /// <summary>
        /// Loads columns of an index of a database object.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, IndexColumn> LoadIndexColumns(Index index)
        {
            var res = new ConcurrentDictionary<string, IndexColumn>();
            var sql = @"
SELECT 
	kcu.COLUMN_NAME,
	kcu.ORDINAL_POSITION,
	c.IS_NULLABLE,
	t.AUTO_INCREMENT,
	c.DATA_TYPE,
	c.CHARACTER_MAXIMUM_LENGTH,
	c.NUMERIC_SCALE,
	c.NUMERIC_PRECISION
FROM information_schema.KEY_COLUMN_USAGE  kcu 
INNER JOIN information_schema.COLUMNS c
ON kcu.TABLE_NAME = c.TABLE_NAME AND kcu.COLUMN_NAME=c.COLUMN_NAME
INNER JOIN information_schema.TABLES t 
ON kcu.TABLE_NAME = t.TABLE_NAME

where kcu.TABLE_NAME LIKE @tableName AND kcu.CONSTRAINT_NAME=@indexName;";
            using (var cn = OpenConnection())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    if (index.IsPrimaryKey)
                    {
                        cmd.Parameters.Add("@tableName", MySqlDbType.VarChar, 128).Value = "%" + index.IndexName.Substring(3) + "%";
                        cmd.Parameters.Add("@indexName", MySqlDbType.VarChar, 128).Value = "PRIMARY";
                    }
                    else
                    {
                        cmd.Parameters.Add("@tableName", MySqlDbType.VarChar, 128).Value = "%" + "" + "%";
                        cmd.Parameters.Add("@indexName", MySqlDbType.VarChar, 128).Value = index.IndexName;
                    }

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var ic = new IndexColumn()
                            {
                                ID = 0,
                                Name = dr.GetString(0),
                                KeyOrdinal = dr.GetInt32(1),
                                IsNullable = dr.GetString(2).ToUpper() == "YES" ? true : false,
                                Ordering = IndexColumnOrdering.Ascending
                            };
                            ic.IsIdentity = dr.GetValue(3).ToString() != "0" ? true : false;
                            ic.DataType = DataType.GetMySqlType(
                                dr.GetString(4)/*,
                                dr.IsDBNull(5) ? dr.GetInt16(5)  : new short(),
                                dr.IsDBNull(6) ? dr.GetByte(6) : Byte.MinValue,
                                dr.IsDBNull(7) ? dr.GetByte(7) : Byte.MinValue*/);
                            // ic.DataType.Size = dr.GetValue(3).ToString() != "null" ?  : 0;
                            /*ic.DataType.Size = dr.GetValue(5).ToString() != "0" ? (short)dr.GetValue(5) : (short)0;
                            ic.DataType.Scale = dr.GetValue(6).ToString() != "0" ? (short)dr.GetValue(6) : (short)0;
                            ic.DataType.Precision = dr.GetValue(7).ToString() != "0" ? (short)dr.GetValue(7) : (short)0;*/
                            res.TryAdd(ic.Name, ic);
                        }
                    }
                }
            }

            return res;
        }

        // it works only from   INFORMATION_SCHEMA -> MySQL 5.5 Manual
        /// <summary>
        /// Loads parameters of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, Parameter> LoadParameters(DatabaseObject obj)
        {
            try
            {

                var res = new ConcurrentDictionary<string, Parameter>();
                var sql = @"
SELECT  

p.ORDINAL_POSITION/*P.PARAMETER_ID*/,
p.PARAMETER_NAME,
p.PARAMETER_MODE,
p.DATA_TYPE /* sys.types NAME*/,
p.CHARACTER_MAXIMUM_LENGTH,
p.NUMERIC_SCALE,
p.NUMERIC_PRECISION
/*p.has_default_value*/
/*p.default_value*/
FROM

INFORMATION_SCHEMA.PARAMETERS p

WHERE p.SPECIFIC_NAME=@objectName AND p.SPECIFIC_SCHEMA=@databaseName; ";

                sql = String.Format(sql, DatabaseObjectType.Table);
                using (var cn = OpenConnection())
                {
                    using (var cmd = new MySqlCommand(sql, cn))
                    {
                        cmd.Parameters.Add("@databaseName", MySqlDbType.VarChar, 128).Value = obj.DatabaseName;
                        cmd.Parameters.Add("@objectName", MySqlDbType.VarChar, 128).Value = obj.ObjectName;
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var par = new Parameter()
                                {
                                    ID = dr.GetInt32(0),
                                    Name = dr.GetString(1),
                                    HasDefaultValue = false,
                                    DefaultValue = null,
                                };
                                if (!String.IsNullOrEmpty(dr.GetString(2)))
                                {
                                    if (dr.GetString(2) == "IN") { par.Direction = ParameterDirection.Input; }
                                    else if (dr.GetString(2) == "OUT") { par.Direction = ParameterDirection.Output; }
                                    else if (dr.GetString(2) == "INOUT") { par.Direction = ParameterDirection.InputOutput; }
                                }
                                else { par.Direction = ParameterDirection.ReturnValue; }
                                par.DataType = DataType.GetMySqlType(
                                    dr.GetString(3)/*,
                                    dr.GetInt16(4),
                                    dr.GetByte(5),
                                    dr.GetByte(6)*/);

                                res.TryAdd(par.Name, par);
                            }
                        }
                    }
                }

                return res;
            }
            catch
            {
                return new ConcurrentDictionary<string, Parameter>();
            }
        }

        internal override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            var sql = @"SELECT table_comment comment 
FROM information_schema.tables t
WHERE t.table_schema = @schemaName AND t.table_name = @objectName
UNION
SELECT routine_comment comment
FROM information_schema.routines r
WHERE r.routine_schema = @schemaName AND r.routine_name = @objectName ;" ;
           
            using (var cn = OpenConnection())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", MySqlDbType.VarChar).Value = databaseObject.DatabaseName;
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
WHERE table_schema= @schemaName and table_name=@objectName ;" ;
            LoadAllVariableMetadata(sql, databaseObject, ((IColumns)databaseObject).Columns);
        }

        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            var sql = @"
SELECT p.parameter_name, r.routine_comment
FROM information_schema.routines r
INNER JOIN information_schema.parameters p ON p.specific_name = r.routine_name
WHERE r.routine_schema = @schemaName and r.routine_name = @objectName ;";

            LoadAllVariableMetadata(sql, databaseObject, ((IParameters)databaseObject).Parameters);
        }

        private void LoadAllVariableMetadata(string sql, DatabaseObject databaseObject, IDictionary variables)
        {
            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnection())
            {
                using (var cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", MySqlDbType.VarChar).Value = databaseObject.DatabaseName;
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
                                meta = ((Variable)variables[name]).Metadata;
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

                    cmd.Parameters.Add("@objtype", MySqlDbType.VarChar, 776).Value = obj.GetMySqlType().ToString();
                    cmd.Parameters.Add("@objoldname", MySqlDbType.VarChar, 776).Value = oldname;
                    cmd.Parameters.Add("@objnewname", MySqlDbType.VarChar, 776).Value = objectName;

                    cmd.ExecuteNonQuery();
                }
            }

        }

        internal override void DropObject(DatabaseObject obj)
        {
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
            }
        }

        protected override DatasetStatistics LoadDatasetStatistics()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a connection to the MySQL database
        /// </summary>
        /// <returns></returns>
        protected MySqlConnection OpenConnection()
        {
            var csb = new MySqlConnectionStringBuilder(ConnectionString);
            csb.AutoEnlist = false;

            var cn = new MySqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
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

    }
}
