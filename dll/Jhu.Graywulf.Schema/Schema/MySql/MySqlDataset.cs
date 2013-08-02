using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using MySql.Data.MySqlClient;

namespace Jhu.Graywulf.Schema.MySql
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class MySqlDataset : DatasetBase
    {
        [DataMember]
        public override string ProviderName
        {
            get { return Constants.MySqlProviderName; }
        }

        [IgnoreDataMember]
        public override string DatabaseName
        {
            get
            {
                MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(ConnectionString);
                return csb.Database;
            }
            set
            {
                MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(ConnectionString);
                csb.Database = value;
                ConnectionString = csb.ConnectionString;
            }
        }

        #region Constructors and initializers

        public MySqlDataset()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public MySqlDataset(DatasetBase old)
            : base(old)
        {
            InitializeMembers(new StreamingContext());
        }

        public MySqlDataset(string name, string connectionString)
        {
            InitializeMembers(new StreamingContext());

            Name = name;
            ConnectionString = connectionString;
        }

        public MySqlDataset(MySqlDataset old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(MySqlDataset old)
        {
        }

        #endregion

        public override string GetFullyResolvedName()
        {
            return String.Format("[{0}]", DatabaseName);
        }

        internal override string GetObjectFullyResolvedName(DatabaseObject databaseObject)
        {
            string format = "{0}..[{1}]";

            return String.Format(format, this.GetFullyResolvedName(), databaseObject.SchemaName, databaseObject.ObjectName);
        }


        protected override void LoadObject<T>(T obj)
        {
            // TODO: rewrite this query to conform with mysql
            string sql = @"
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @database AND TABLE_TYPE = @objectType AND TABLE_NAME = @objectName;";

            using (MySqlConnection cn = OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {

                    cmd.Parameters.Add("@database", MySqlDbType.VarChar, 128).Value = DatabaseName;
                    cmd.Parameters.Add("@objectType", MySqlDbType.VarChar, 50).Value = Constants.MySqlObjectTypeIds[obj.ObjectType];
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
            string sql;

            switch (Schema.Constants.DatabaseObjectTypes[typeof(T)])
            {
                case DatabaseObjectType.Table:
                    sql = @"
SELECT TABLE_NAME, TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @database AND TABLE_TYPE = @objectType;";
                    break;
                default:
                    throw new NotImplementedException();
            }

            using (MySqlConnection cn = OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@database", MySqlDbType.VarChar, 128).Value = DatabaseName;
                    cmd.Parameters.Add("@objectType", MySqlDbType.VarChar, 50).Value = Constants.MySqlObjectTypeIds[Schema.Constants.DatabaseObjectTypes[typeof(T)]];

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

        internal override ConcurrentDictionary<string, Column> LoadColumns(DatabaseObject obj)
        {
            var res = new ConcurrentDictionary<string, Column>();

            string sql = @"
SELECT ordinal_position, column_name, data_type
FROM information_schema.columns
WHERE table_schema = @databaseName and table_name= @tableName;";

            using (MySqlConnection cn = OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
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
                                DataType = DataType.GetType(dr.GetString(2)),
                                // TODO: data size
                            };

                            res.TryAdd(cd.Name, cd);
                        }
                    }
                }
            }

            return res;
        }

        internal override ConcurrentDictionary<string, Index> LoadIndexes(DatabaseObject obj)
        {
            return new ConcurrentDictionary<string, Index>();
        }

        internal override ConcurrentDictionary<string, IndexColumn> LoadIndexColumns(Index index)
        {
#if false
            ConcurrentDictionary<string, IndexColumn> cols = new ConcurrentDictionary<string, IndexColumn>(SchemaManager.Comparer);

            using (MySqlConnection cn = OpenConnection())
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

            return new ConcurrentDictionary<string, IndexColumn>();
        }

        internal override ConcurrentDictionary<string, Parameter> LoadParameters(DatabaseObject obj)
        {
            return new ConcurrentDictionary<string, Parameter>();
        }

        internal override DatabaseObjectMetadata LoadDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void DropDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void SaveDatabaseObjectMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
        }

        internal override void LoadAllVariableMetadata(DatabaseObject databaseObject)
        {
            throw new NotImplementedException();
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
        }

        internal override void DropObject(DatabaseObject obj)
        {
            throw new NotImplementedException();
        }

        protected override DatasetStatistics LoadDatasetStatistics()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a connection to the mySQL database
        /// </summary>
        /// <returns></returns>
        protected MySqlConnection OpenConnection()
        {
            MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(ConnectionString);
            csb.AutoEnlist = false;

            MySqlConnection cn = new MySqlConnection(csb.ConnectionString);
            cn.Open();
            return cn;
        }

        public override string GetSpecializedConnectionString(string connectionString, bool integratedSecurity, string username, string password, bool enlist)
        {
            MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(connectionString);
            csb.IntegratedSecurity = integratedSecurity;
            csb.UserID = username;
            csb.Password = password;
            csb.AutoEnlist = enlist;

            return csb.ConnectionString;
        }

    }
}
