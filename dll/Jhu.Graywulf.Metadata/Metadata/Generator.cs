using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;


namespace Jhu.Graywulf.Metadata
{
    public class Generator : IDisposable
    {
        private SqlConnectionStringBuilder connectionString;

        private XmlDocument xml;
        private SqlConnection databaseConnection;
        private SqlTransaction databaseTransaction;
        private Server server;
        private Database database;

        public Generator(SqlConnectionStringBuilder connectionString)
        {
            this.connectionString = connectionString;
            OpenConnection();
        }

        public void LoadXml(string input)
        {
            LoadXml(new StringReader(input));
        }

        public void LoadXml(TextReader input)
        {
            // Create Xml Dom object
            xml = new XmlDocument();
            xml.Load(input);
        }

        public void LoadXml(XmlDocument xml)
        {
            this.xml = xml;
        }

        private void OpenConnection()
        {
            if (connectionString.IntegratedSecurity)
            {
                server = new Server(connectionString.DataSource);
            }
            else
            {
                ServerConnection sc = new ServerConnection(connectionString.DataSource, connectionString.UserID, connectionString.Password);
                server = new Server(sc);
            }

            database = server.Databases[connectionString.InitialCatalog];

            databaseConnection = new SqlConnection(connectionString.ConnectionString);
            databaseConnection.Open();
            databaseTransaction = databaseConnection.BeginTransaction();
        }

        private void CloseConnection()
        {
            databaseTransaction.Commit();
            databaseTransaction.Dispose();

            databaseConnection.Close();
            databaseConnection.Dispose();
        }

        public void CreateMetadata()
        {
            if (xml == null)
            {
                throw new Exception("Xml document must be loaded first.");
            }

            var root = xml.DocumentElement;

            // Go through all top level tags - table, procedure, view, function
            foreach (XmlElement e in root.ChildNodes)
            {
                switch (e.Name.ToLower())
                {
                    case Constants.TagEnum:
                        //ProcessEnum(e);
                        break;
                    case Constants.TagVersion:
                        ProcessVersion(e);
                        break;
                    default:
                        var ot = (ObjectType)Enum.Parse(typeof(ObjectType), e.Name, true);
                        IExtendedProperties smoobject;

                        string schema, name;
                        ResolveCompositeName(e.GetAttribute(Constants.AttributeName), out schema, out name);
                        switch (ot)
                        {
                            case ObjectType.Table:
                                smoobject = database.Tables[name, schema];
                                break;
                            case ObjectType.View:
                                smoobject = database.Views[name, schema];
                                break;
                            case ObjectType.Procedure:
                                smoobject = database.StoredProcedures[name, schema];
                                break;
                            case ObjectType.Function:
                                smoobject = database.UserDefinedFunctions[name, schema];
                                break;
                            default:
                                throw new Exception("Invalid tag found.");
                        }

                        ProcessObject(e, ot, smoobject);
                        break;
                }
            }
        }

        private void CreateMetaSchema()
        {
            if (!database.Schemas.Contains(Constants.SchemaMeta))
            {
                string sql = "CREATE SCHEMA [{0}]";
                sql = String.Format(sql, Constants.SchemaMeta);

                using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CreateEnumTable()
        {
            if (!database.Tables.Contains(Constants.TableEnum, Constants.SchemaMeta))
            {
                string sql = "CREATE TABLE [{0}].[{1}] ([Name] nvarchar(50), [Key] nvarchar(50), [Value] sql_variant, [Summary] ntext)";
                sql = String.Format(sql, Constants.SchemaMeta, Constants.TableEnum);

                using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ProcessEnum(XmlElement xmlElement)
        {
            string sql;

            // Delete old values
            sql = "DELETE meta.Enum WHERE [Name] = @Name";

            using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = xmlElement.Attributes[Constants.AttributeName].Value;
                cmd.ExecuteNonQuery();
            }

            // Insert new values
            sql = "INSERT meta.Enum ([Name], [Key], [Value], [Summary]) VALUES (@Name, @Key, @Value, @Summary)";

            using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = xmlElement.Attributes[Constants.AttributeName].Value;
                cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 50);
                cmd.Parameters.Add("@Value", SqlDbType.Variant);
                cmd.Parameters.Add("@Summary", SqlDbType.NVarChar);

                foreach (XmlElement e in xmlElement.ChildNodes)
                {
                    if (String.Compare(e.Name, Constants.TagKey, true) != 0)
                    {
                        throw new Exception("Invalid tag found.");
                    }

                    cmd.Parameters["@Key"].Value = e.Attributes[Constants.AttributeName].Value;
                    cmd.Parameters["@Value"].Value = ParseString(e.Attributes[Constants.AttributeValue].Value);
                    cmd.Parameters["@Summary"].Value = e.InnerXml;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ProcessVersion(XmlElement xmlElement)
        {
            // Drop version property if present
            if (database.ExtendedProperties.Contains(Constants.KeyVersion))
            {
                database.ExtendedProperties[Constants.KeyVersion].Drop();
            }

            ExtendedProperty exp = new ExtendedProperty(database, Constants.KeyVersion);
            exp.Value = xmlElement.InnerText;
            database.ExtendedProperties.Add(exp);
            exp.Create();
        }

        private void ProcessObject(XmlElement xmlElement, ObjectType objectType, IExtendedProperties smoObject)
        {
            // Delete old ExtendedProperties
            while (smoObject.ExtendedProperties.Count > 0)
            {
                smoObject.ExtendedProperties[0].Drop();
            }

            // Dictionary to hold new properties, required for mergin multiple tags
            var properties = new Dictionary<string, string>();

            foreach (XmlNode n in xmlElement.ChildNodes)
            {
                if (n is XmlElement)
                {
                    var e = (XmlElement)n;

                    // Try to parse as subitem (column, param)
                    var pt = ParameterType.Unknown;
                    bool subitem = Enum.TryParse<ParameterType>(e.Name, true, out pt);

                    if (subitem)
                    {
                        // Process tag as subitem
                        if (!Constants.TagPairs[objectType].Contains(pt))
                        {
                            throw new Exception(String.Format("Invalid tag found, '{0}' cannot be part of '{1}'", pt.ToString(), objectType.ToString()));
                        }

                        IExtendedProperties paramobject;

                        var name = e.Attributes[Constants.AttributeName].Value.Trim('[', ']');

                        switch (objectType)
                        {
                            case ObjectType.Table:
                                paramobject = ((Table)smoObject).Columns[name];
                                break;
                            case ObjectType.View:
                                paramobject = ((View)smoObject).Columns[name];
                                break;
                            case ObjectType.Procedure:
                                paramobject = ((StoredProcedure)smoObject).Parameters[name];
                                break;
                            case ObjectType.Function:
                                switch (pt)
                                {
                                    case ParameterType.Param:
                                        paramobject = ((UserDefinedFunction)smoObject).Parameters[name];
                                        break;
                                    case ParameterType.Column:
                                        paramobject = ((UserDefinedFunction)smoObject).Columns[name];
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                                break;
                            default:
                                throw new Exception("Invalid tag found.");
                        }
                        ProcessParameter(e, paramobject);
                    }
                    else
                    {
                        if (properties.Keys.Contains<string>(e.Name))
                        {
                            // merge
                            var s = properties[e.Name];
                            s += Environment.NewLine + e.InnerXml;
                            properties[e.Name] = s;
                        }
                        else
                        {
                            // new
                            properties.Add(e.Name, e.InnerXml);
                        }
                    }
                }
            }

            // Save extended properties
            foreach (string key in properties.Keys)
            {
                AddExtendedProperty(key, properties[key], smoObject);
            }
        }

        private void ProcessParameter(XmlElement xmlElement, IExtendedProperties smoObject)
        {
            // Delete old ExtendedProperties
            while (smoObject.ExtendedProperties.Count > 0)
            {
                smoObject.ExtendedProperties[0].Drop();
            }

            foreach (var node in xmlElement.ChildNodes)
            {
                if (node is XmlElement)
                {
                    var e = (XmlElement)node;

                    AddExtendedProperty(e.Name, e.InnerText, smoObject);
                }
            }
            
            foreach (XmlAttribute attribute in xmlElement.Attributes)
            {
                if (attribute.Name != Constants.AttributeName)
                {
                    if (Constants.ParameterAttributes.Contains(attribute.Name.ToLower()))
                    {
                        AddExtendedProperty(attribute, smoObject);
                    }
                    else
                    {
                        throw new Exception(String.Format("Invalid attribute: {0}", attribute.Name));
                    }
                }
            }
        }

        private void AddExtendedProperty(XmlAttribute xmlAttribute, IExtendedProperties smoObject)
        {
            AddExtendedProperty(xmlAttribute.Name, xmlAttribute.Value, smoObject);
        }

        private void AddExtendedProperty(XmlElement xmlElement, IExtendedProperties smoObject)
        {
            AddExtendedProperty(xmlElement.Name, xmlElement.InnerXml, smoObject);
        }

        private void AddExtendedProperty(string name, string strValue, IExtendedProperties smoObject)
        {
            name = Constants.SchemaMeta + "." + name;

            object value = ParseString(strValue);

            ExtendedProperty exp;

            if (smoObject.ExtendedProperties.Contains(name))
            {
                exp = smoObject.ExtendedProperties[name];
                exp.Value = value;
                exp.Alter();
            }
            else
            {
                exp = new ExtendedProperty((SqlSmoObject)smoObject, name, value);
                smoObject.ExtendedProperties.Add(exp);
                exp.Create();
            }

        }

        // TODO use regex instead of this hack
        private void ResolveCompositeName(string compositeName, out string schema, out string name)
        {
            string[] parts = compositeName.Split(new string[] { "].[" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                schema = "dbo";
                name = parts[0].Trim('[', ']');
            }
            else if (parts.Length == 2)
            {
                schema = parts[0].Trim('[', ']');
                name = parts[1].Trim('[', ']');
            }
            else
            {
                throw new Exception("Invalid object name specified.");
            }
        }

        private object ParseString(string strValue)
        {
            object value;
            long lvalue;
            double dvalue;
            if (long.TryParse(strValue, out lvalue))
            {
                value = lvalue;
            }
            else if (double.TryParse(strValue, out dvalue))
            {
                value = dvalue;
            }
            else
            {
                value = strValue.Trim();
            }

            return value;
        }

        /*
        private void CreateIndexMapTable()
        {
            if (!database.Tables.Contains(TableIndexMap, SchemaMeta))
            {
                string sql = @"
CREATE TABLE [{0}].[{1}] (
    [Name] nvarchar(128),
    [IndexGroup] nvarchar(50),
    [Type] char(1),
    [TableSchema] nvarchar(128),
    [TableName] nvarchar(128),
    [ColumnList] nvarchar(1024),
    [ReferencedTableSchema] nvarchar(128),
    [ReferencedTableName] nvarchar(128),
    [ReferencedKey] nvarchar(128))";
                sql = String.Format(sql, SchemaMeta, TableIndexMap);

                using (SqlCommand cmd = new SqlCommand(sql, databaseConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        
        }

        public void PopulateIndexMapTable()
        {
            ClearIndexMapTable();

            string sql = 
@"INSERT [{0}].[{1}]
	([Name], [IndexGroup], [Type], [TableSchema], [TableName], [ColumnList],
    [ReferencedTableSchema], [ReferencedTableName], [ReferencedKey])
VALUES
    (@Name, @IndexGroup, @Type, @TableSchema, @TableName, @ColumnList,
    @ReferencedTableSchema, @ReferencedTableName, @ReferencedKey)";
            sql = String.Format(sql, SchemaMeta, TableIndexMap);

            using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
            {
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@IndexGroup", SqlDbType.NVarChar, 50).Value = "DEFAULT";    // **** TODO
                cmd.Parameters.Add("@Type", SqlDbType.Char, 1);
                cmd.Parameters.Add("@TableSchema", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@ColumnList", SqlDbType.NVarChar, 1024);
                cmd.Parameters.Add("@ReferencedTableSchema", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@ReferencedTableName", SqlDbType.NVarChar, 128);
                cmd.Parameters.Add("@ReferencedKey", SqlDbType.NVarChar, 128);

                foreach (Table t in database.Tables)
                {
                    cmd.Parameters["@TableSchema"].Value = t.Schema;
                    cmd.Parameters["@TableName"].Value = t.Name;

                    foreach (Index i in t.Indexes)
                    {
                        cmd.Parameters["@Name"].Value = i.Name;
                        cmd.Parameters["@ReferencedTableSchema"].Value = string.Empty;
                        cmd.Parameters["@ReferencedTableName"].Value = string.Empty;
                        cmd.Parameters["@ReferencedKey"].Value = string.Empty;

                        switch (i.IndexKeyType)
                        {
                            case IndexKeyType.DriPrimaryKey:
                                cmd.Parameters["@Type"].Value = "P"; break;
                            case IndexKeyType.DriUniqueKey:
                                cmd.Parameters["@Type"].Value = "U"; break;
                            default:
                                cmd.Parameters["@Type"].Value = "I"; break;
                        }

                        string cols = string.Empty;
                        for (int k = 0; k < i.IndexedColumns.Count; k++)
                        {
                            IndexedColumn c = i.IndexedColumns[k];
                            if (k > 0) cols += ", ";
                            cols += c.Name;
                        }

                        cmd.Parameters["@ColumnList"].Value = cols;

                        cmd.ExecuteNonQuery();
                    }

                    foreach (ForeignKey f in t.ForeignKeys)
                    {
                        cmd.Parameters["@Name"].Value = f.Name;
                        cmd.Parameters["@ReferencedTableSchema"].Value = f.ReferencedTableSchema;
                        cmd.Parameters["@ReferencedTableName"].Value = f.ReferencedTable;
                        cmd.Parameters["@ReferencedKey"].Value = f.ReferencedKey;

                        cmd.Parameters["@Type"].Value = "F";

                        string cols = string.Empty;
                        for (int k = 0; k < f.Columns.Count; k++)
                        {
                            ForeignKeyColumn c = f.Columns[k];
                            if (k > 0) cols += ", ";
                            cols += c.Name;
                        }

                        cmd.Parameters["@ColumnList"].Value = cols;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void ClearIndexMapTable()
        {
            string sql = "DELETE meta.IndexMap";

            using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
            {
                cmd.ExecuteNonQuery();
            }
        }
         * */

        /*
         * TODO: remove, implementation uses SMO
        private void SetExtendedProperty(SmoObjectBase smoObject, string name, string value)
        {
            string sql = "meta.spSetExtendedProperty";

            using (SqlCommand cmd = new SqlCommand(sql, databaseConnection, databaseTransaction))
            {
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 128).Value = name;
                cmd.Parameters.Add("@value", SqlDbType.Variant).Value = value;
                cmd.Parameters.Add("@level0type", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@level0name", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@level1type", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@level1name", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@level2type", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("@level2name", SqlDbType.NVarChar, 50).Value = DBNull.Value;
                cmd.Parameters.Add("RETVAL", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                if (smoObject is Table)
                {
                    Table t = smoObject as Table;
                    cmd.Parameters["@level0type"].Value = "SCHEMA";
                    cmd.Parameters["@level0name"].Value = t.Schema;
                    cmd.Parameters["@level1type"].Value = "TABLE";
                    cmd.Parameters["@level1name"].Value = t.Name;
                }
                else if (smoObject is View)
                {
                    View v = smoObject as View;
                    cmd.Parameters["@level0type"].Value = "SCHEMA";
                    cmd.Parameters["@level0name"].Value = v.Schema;
                    cmd.Parameters["@level1type"].Value = "VIEW";
                    cmd.Parameters["@level1name"].Value = v.Name;
                }
                else if (smoObject is StoredProcedure)
                {
                    StoredProcedure sp = smoObject as StoredProcedure;
                    cmd.Parameters["@level0type"].Value = "SCHEMA";
                    cmd.Parameters["@level0name"].Value = sp.Schema;
                    cmd.Parameters["@level1type"].Value = "TABLE";
                    cmd.Parameters["@level1name"].Value = sp.Name;
                }
                else if (smoObject is UserDefinedFunction)
                {
                    UserDefinedFunction f = smoObject as UserDefinedFunction;
                    cmd.Parameters["@level0type"].Value = "SCHEMA";
                    cmd.Parameters["@level0name"].Value = t.Schema;
                    cmd.Parameters["@level1type"].Value = "TABLE";
                    cmd.Parameters["@level1name"].Value = t.Name;
                }

                cmd.ExecuteNonQuery();

                int retval = (int)cmd.Parameters["RETVAL"].Value;

                if (retval == 0)
                {
                    throw new Exception(String.Format("Invalid extended property: {0}.", name));
                }
            }
        }
         * */

        #region IDisposable Members

        public void Dispose()
        {
            CloseConnection();
        }

        #endregion
    }
}
