using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using Npgsql;

namespace Jhu.Graywulf.Schema.PostgreSql
{ /// <summary>
    /// Implements schema reflection functions for Npgsql
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class PostgreSqlDataset : DatasetBase
    {
        protected string defaultSchemaName;
        protected bool isOnLinkedServer;
        protected bool isRemoteDataset;
        /// <summary>
        /// Gets or sets the value determining if the data is available
        /// via a linked Npgsql.
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
            get { return Constants.PostgreSqlProviderName; }
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
            this.defaultSchemaName = "public";
            this.isOnLinkedServer = false;
            this.isRemoteDataset = false;
        }

        /// <summary>
        /// Copies private member variables form another instance
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(PostgreSqlDataset old)
        {
            this.defaultSchemaName = old.defaultSchemaName;
            this.isOnLinkedServer = old.isOnLinkedServer;
            this.isRemoteDataset = old.isRemoteDataset;
        }

        public override object Clone()
        {
            return new PostgreSqlDataset(this);
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
            string format = "{0}.[{1}].[{2}]";

            if (!String.IsNullOrWhiteSpace(databaseObject.SchemaName))
            {
                return String.Format(format, this.GetFullyResolvedName(), databaseObject.SchemaName, databaseObject.ObjectName);
            }
            else
            {
                return String.Format(format, this.GetFullyResolvedName(), this.defaultSchemaName, databaseObject.ObjectName);
            }
        }

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
                    sql = @"
SELECT TABLE_SCHEMA,TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @schemaName AND TABLE_TYPE IN ({0}) AND TABLE_NAME = @objectName;";
                    break;
                case DatabaseObjectType.View:
                    sql = @"
SELECT TABLE_SCHEMA,TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @schemaName AND TABLE_TYPE IN ({0}) AND TABLE_NAME = @objectName;";
                    break;
                case DatabaseObjectType.StoredProcedure:
                    sql = @"
SELECT ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @schemaName AND  ROUTINE_NAME=@objectName AND ROUTINE_TYPE IN ({0})";
                    break;
                case DatabaseObjectType.ScalarFunction:
                    sql = @"
SELECT  ROUTINE_SCHEMA, ROUTINE_NAME, ROUTINE_TYPE
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @schemaName AND ROUTINE_NAME=@objectName AND ROUTINE_TYPE IN ({0})";
                    break;
            }


            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));
            using (NpgsqlConnection cn = OpenConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cn))
                {
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


        /// <summary>
        /// Loads all objects of a certain kind
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected override IEnumerable<KeyValuePair<string, T>> LoadAllObjects<T>(string databaseName)
        {
            string sql = @"
SELECT 
    INFORMATION_SCHEMA.ROUTINES.ROUTINE_NAME,
    INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE,
    INFORMATION_SCHEMA.ROUTINES.ROUTINE_SCHEMA
FROM   INFORMATION_SCHEMA.ROUTINES
WHERE INFORMATION_SCHEMA.ROUTINES.SPECIFIC_CATALOG = @database
AND INFORMATION_SCHEMA.ROUTINES.ROUTINE_TYPE IN({0}) AND INFORMATION_SCHEMA.ROUTINES.SPECIFIC_SCHEMA != 'information_schema' AND INFORMATION_SCHEMA.ROUTINES.SPECIFIC_SCHEMA != 'pg_catalog'
UNION
SELECT 
    INFORMATION_SCHEMA.TABLES.TABLE_NAME,
    INFORMATION_SCHEMA.TABLES.TABLE_TYPE,
    INFORMATION_SCHEMA.TABLES.TABLE_SCHEMA
FROM   INFORMATION_SCHEMA.TABLES
WHERE INFORMATION_SCHEMA.TABLES.TABLE_CATALOG = @database 
AND INFORMATION_SCHEMA.TABLES.TABLE_TYPE IN ({0})  AND INFORMATION_SCHEMA.TABLES.TABLE_SCHEMA != 'information_schema' AND INFORMATION_SCHEMA.TABLES.TABLE_SCHEMA != 'pg_catalog'";

            sql = String.Format(sql, GetObjectTypeIdListString(Schema.Constants.DatabaseObjectTypes[typeof(T)]));

            using (NpgsqlConnection cn = OpenConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@database", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = DatabaseName;

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
            if (String.IsNullOrWhiteSpace(schemaName))
            {
                schemaName = defaultSchemaName;
            }
            return String.Format("{0}|{1}|{2}|{3}|{4}", objectType, datasetName, databaseName, schemaName, objectName);
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
        //TODO SIZE
        /// <summary>
        /// Loads columns of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, Column> LoadColumns(DatabaseObject obj)
        {
            var res = new ConcurrentDictionary<string, Column>();

            string sql = @"
SELECT ordinal_position, column_name, data_type, COALESCE(character_maximum_length, -1) AS ""max_length"", COALESCE(numeric_scale, -1) AS ""scale"", COALESCE(numeric_precision, -1) AS ""precision"", is_nullable
FROM information_schema.columns
WHERE table_catalog = @databaseName and table_name= @tableName and table_schema=@schemaName;";

            using (NpgsqlConnection cn = OpenConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@databaseName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.DatabaseName;
                    cmd.Parameters.Add("@tableName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;

                    using (NpgsqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Column cd = new Column(obj)
                            {
                                ID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                                IsNullable = (StringComparer.InvariantCultureIgnoreCase.Compare(dr.GetString(6), "yes") == 0)
                            };

                            cd.DataType = GetTypeFromProviderSpecificName(
                                dr.GetString(2),
                                Convert.ToInt32(dr.GetInt32(3)),
                                Convert.ToInt16(dr.GetInt32(4)),
                                Convert.ToInt16(dr.GetInt32(5)));

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
select  
        kcu.ordinal_position,
        kcu.constraint_name,
	    tc.constraint_type,
	--c.non_unique,
        kcu.column_name,
        kcu.table_name
from information_schema.table_constraints tc 
inner join  information_schema.key_column_usage kcu 
on kcu.constraint_schema = tc.constraint_schema
where kcu.constraint_schema=@schemaName and kcu.table_name=@objectName GROUP BY 1,2,3,4,5;";

            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            var idx = new Index((TableOrView)obj);
                            if (dr.GetString(2).ToUpper() == "PRIMARY KEY")
                            {
                                idx.IsPrimaryKey = true;
                                idx.IndexId = dr.GetInt32(0);
                                idx.IsClustered = true;
                                //idx.IsUnique = dr.GetBoolean(3);
                                idx.IndexName = dr.GetString(1);

                            }
                            else
                            {
                                idx.IsPrimaryKey = false;
                                idx.IndexId = dr.GetInt32(0);
                                idx.IndexName = dr.GetString(2);
                                idx.IsClustered = true;
                                //idx.IsUnique = dr.GetBoolean(3);
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
#if false
            ConcurrentDictionary<string, IndexColumn> cols = new ConcurrentDictionary<string, IndexColumn>(SchemaManager.Comparer);

            using (NpgsqlConnection cn = OpenConnection())
            {
                //show columns from test100 from openlab;
                //+------------+----------+------+-----+---------+----------------+
                //| Field      | Type     | Null | Key | Default | Extra          |
                //+------------+----------+------+-----+---------+----------------+
                //| Id         | int(11)  | NO   | PRI | NULL    | auto_increment |
                //| Name       | char(35) | NO   |     |         |                |
                //| Country    | char(3)  | NO   | UNI |         |                |
                //| District   | char(20) | YES  | MUL |         |                |
                //| Population | int(11)  | NO   |     | 0       |                |
                //+------------+----------+------+-----+---------+----------------+
                foreach (IndexColumn ic in LoadIndexColumns(cn, index))
                {
                    cols.TryAdd(ic.ColumnName, ic);
                }
            }

            return cols;
#endif

            var res = new ConcurrentDictionary<string, IndexColumn>();
            var sql = @"
SELECT 
	kcu.COLUMN_NAME,
	kcu.ORDINAL_POSITION,
	c.IS_NULLABLE,
	c.IS_IDENTITY,
	c.DATA_TYPE,
	c.CHARACTER_MAXIMUM_LENGTH,
	c.NUMERIC_SCALE,
	c.NUMERIC_PRECISION
FROM information_schema.KEY_COLUMN_USAGE  kcu 
INNER JOIN information_schema.COLUMNS c
ON kcu.TABLE_NAME = c.TABLE_NAME AND kcu.COLUMN_NAME=c.COLUMN_NAME

where kcu.TABLE_NAME=@tableName;";
            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@tableName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = index.IndexName.Substring(3);

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
                                IsIdentity = dr.GetString(3).ToUpper() == "YES" ? true : false,
                                Ordering = IndexColumnOrdering.Ascending
                            };
                            ic.DataType = GetTypeFromProviderSpecificName(
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

        /// <summary>
        /// Loads parameters of a database object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal override ConcurrentDictionary<string, Parameter> LoadParameters(DatabaseObject obj)
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
INNER JOIN INFORMATION_SCHEMA.ROUTINES r ON r.SPECIFIC_NAME = p.SPECIFIC_NAME

WHERE r.ROUTINE_NAME = @objectName AND p.SPECIFIC_SCHEMA=@schemaName; ";

            //sql = String.Format(sql, DatabaseObjectType.Table);
            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@schemaName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.SchemaName;
                    cmd.Parameters.Add("@objectName", NpgsqlTypes.NpgsqlDbType.Varchar, 128).Value = obj.ObjectName;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var par = new Parameter(obj)
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
                            par.DataType = GetTypeFromProviderSpecificName(
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
            using (var cn = OpenConnection())
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
FROM pg_catalog.pg_statio_all_tables as st
  inner join pg_catalog.pg_description pgd on (pgd.objoid=st.relid)
  inner join information_schema.columns c on (pgd.objsubid=c.ordinal_position
    and  c.table_schema=@schemaName and c.table_name=@objectName);";
            LoadAllVariableMetadata(sql, databaseObject, ((IColumns)databaseObject).Columns);
        }

        protected override void LoadAllParameterMetadata(DatabaseObject databaseObject)
        {
            var sql = @"SELECT  pgd.description,proargnames
FROM    pg_catalog.pg_namespace n
inner JOIN    pg_catalog.pg_proc p ON p.pronamespace = n.oid
inner join pg_catalog.pg_description pgd on (pgd.objoid=p.oid)
WHERE   nspname = @schemaName and proname= @objectName;";

            var variables = ((IParameters)databaseObject).Parameters;

            foreach (Variable v in variables.Values)
            {
                v.Metadata = new VariableMetadata();
            }

            using (var cn = OpenConnection())
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
                            var paramname =dr.GetValue(1) as string[];
                            if (paramname.Count()>0) 
                            {
                                var paramlist = paramname;//.Replace("{", "").Replace("}", "").Split(',');

                                foreach (var v in paramlist)
                                {
                                    meta = ((Variable)variables[v]).Metadata;
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

            using (var cn = OpenConnection())
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
                using (var cmd = new NpgsqlCommand(sql, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@objtype", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = obj.GetType().ToString();
                    cmd.Parameters.Add("@objoldname", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = oldname;
                    cmd.Parameters.Add("@objnewname", NpgsqlTypes.NpgsqlDbType.Varchar, 776).Value = objectName;

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
                Constants.PostgreSqlObjectTypeNames[obj.ObjectType],
                obj.GetFullyResolvedName());

            using (var cn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(sql, cn))
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
        /// Opens a connection to the Npgsql database
        /// </summary>
        /// <returns></returns>
        protected NpgsqlConnection OpenConnection()
        {
            
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            csb.Enlist = false;

            NpgsqlConnection cn = new NpgsqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
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

        protected override DataType GetTypeFromProviderSpecificName(string name)
        {
            switch (name.ToLowerInvariant().Trim())
            {
                case Constants.TypeNameSmallInt:
                    return DataType.SmallInt;
                case Constants.TypeNameInt:
                    return DataType.Int;
                case Constants.TypeNameBigInt:
                    return DataType.BigInt;
                case Constants.TypeNameNumeric:
                    return DataType.Numeric;
                case Constants.TypeNameReal:
                    return DataType.Real;
                case Constants.TypeNameDoublePrecision:
                    return DataType.Real;
                case Constants.TypeNameMoney:
                    return DataType.Money;
                case Constants.TypeNameVarChar:
                    return DataType.VarChar;
                case Constants.TypeNameChar:
                    return DataType.Char;
                case Constants.TypeNameText:
                    return DataType.Text;
                case Constants.TypeNameBytea:
                    return DataType.Binary;
                case Constants.TypeNameTimestamp:
                    return DataType.DateTime;
                case Constants.TypeNameTimestampWithTimeZone:
                    return DataType.DateTime;
                case Constants.TypeNameDate:
                    return DataType.Date;
                case Constants.TypeNameTime:
                    return DataType.DateTime;
                case Constants.TypeNameTimeWithTimeZone:
                    return DataType.DateTime;
                case Constants.TypeNameInterval:
                    return DataType.VarChar;    // *** TODO converting to varchar, need to convert
                case Constants.TypeNameBoolean:
                    return DataType.Bit;
                case Constants.TypeNamePoint:
                    return DataType.VarChar;
                case Constants.TypeNameLine:
                    return DataType.VarChar;
                case Constants.TypeNameLseg:
                    return DataType.VarChar;
                case Constants.TypeNameBox:
                    return DataType.VarChar;
                case Constants.TypeNamePath:
                    return DataType.VarChar;
                case Constants.TypeNamePolygon:
                    return DataType.VarChar;
                case Constants.TypeNameCircle:
                    return DataType.VarChar;
                case Constants.TypeNameCidr:
                    return DataType.VarChar;
                case Constants.TypeNameInet:
                    return DataType.VarChar;
                case Constants.TypeNameMacaddr:
                    return DataType.VarChar;
                case Constants.TypeNameBit:
                    return DataType.Bit;
                case Constants.TypeNameBitVarying:
                    return DataType.Bit;            // *** TODO check is it works
                case Constants.TypeNameTsvector:
                    return DataType.VarChar;
                case Constants.TypeNameUuid:
                    return DataType.VarChar;
                case Constants.TypeNameXml:
                    return DataType.Xml;
                case Constants.TypeNameJson:
                    return DataType.Text;
                case Constants.TypeNameArray:
                    return DataType.VarChar;        // *** TODO need to modify
                case Constants.TypeNameInt4Range:
                    return DataType.Int;
                case Constants.TypeNameInt8Range:
                    return DataType.BigInt;
                case Constants.TypeNameNumRange:
                    return DataType.Numeric;
                case Constants.TypeNameTsRange:
                    return DataType.DateTime;
                case Constants.TypeNameTstzRange:
                    return DataType.DateTime;
                case Constants.TypeNameDateRange:
                    return DataType.Date;       // *** TODO
                case Constants.TypeNameOid:
                    return DataType.VarChar;
                default:
                    return DataType.Unknown;
            }
        }

    }
}


