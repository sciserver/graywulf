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
    public class Extractor
    {

        // **** This whole class works from SMO now but could be
        // rewritten later to use the graywulf schema browser objects
        // when ready

        private SqlConnectionStringBuilder connectionString;

        private XmlDocument xml;
        private SqlConnection databaseConnection;
        private SqlTransaction databaseTransaction;
        private Server server;
        private Database database;

        public Extractor(SqlConnectionStringBuilder connectionString)
        {
            this.connectionString = connectionString;
            OpenConnection();
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

        public string ExtractMetadata()
        {
            xml = new XmlDocument();

            XmlNode md = xml.AppendChild(xml.CreateElement(Constants.TagMetadata));

            List<ScriptSchemaObjectBase> objs = new List<ScriptSchemaObjectBase>();
            objs.AddRange(database.Tables.Cast<ScriptSchemaObjectBase>());
            objs.AddRange(database.Views.Cast<ScriptSchemaObjectBase>());
            objs.AddRange(database.StoredProcedures.Cast<ScriptSchemaObjectBase>());
            objs.AddRange(database.UserDefinedFunctions.Cast<ScriptSchemaObjectBase>());

            foreach (ScriptSchemaObjectBase o in objs)
            {
                XmlNode node = ExtractObject(o);

                if (node != null)
                {
                    md.AppendChild(node);
                }
            }

            return xml.InnerXml;
        }

        private XmlNode ExtractObject(ScriptSchemaObjectBase obj)
        {
            bool ext = true;

            ext &= Constants.ObjectTypeMap.ContainsKey(obj.GetType());

            // Avoid metadata tables
            ext &= String.Compare(obj.Schema, Constants.SchemaMeta, true) != 0;
            
            // Avoid system views and tables
            ext &= String.Compare(obj.Schema, Constants.SchemaSys, true) != 0;
            ext &= String.Compare(obj.Schema, Constants.SchemaInfoSch, true) != 0;

            if (ext)
            {
                XmlNode node = xml.CreateElement(Constants.ObjectTypeMap[obj.GetType()].ToString().ToLower());

                node.Attributes.Append(xml.CreateAttribute(Constants.AttributeName)).Value = String.Format("[{0}].[{1}]", obj.Schema, obj.Name);

                foreach (ExtendedProperty p in ((IExtendedProperties)obj).ExtendedProperties)
                {
                    string pname = p.Name.Replace(Constants.SchemaMeta.ToLowerInvariant() + ".", String.Empty);

                    if (Constants.ObjectElements.Contains(pname))
                    {
                        node.AppendChild(xml.CreateElement(pname)).InnerText = p.Value.ToString();
                    }
                    else
                    {
                        // Assume all the rest is attribute
                        node.Attributes.Append(xml.CreateAttribute(pname)).Value = p.Value.ToString();
                    }
                }

                ExtractColumnsAndParameters(obj, node);

                return node;
            }
            else
            {
                return null;
            }
        }

        private void ExtractColumnsAndParameters(ScriptSchemaObjectBase obj, XmlNode node)
        {
            List<NamedSmoObject> colpar = new List<NamedSmoObject>();

            if (obj is Table)
            {
                colpar.AddRange(((Table)obj).Columns.Cast<NamedSmoObject>());
            }
            else if (obj is View)
            {
                colpar.AddRange(((View)obj).Columns.Cast<NamedSmoObject>());
            }
            else if (obj is StoredProcedure)
            {
                colpar.AddRange(((StoredProcedure)obj).Parameters.Cast<NamedSmoObject>());
            }
            else if (obj is UserDefinedFunction)
            {
                colpar.AddRange(((UserDefinedFunction)obj).Parameters.Cast<NamedSmoObject>());
                colpar.AddRange(((UserDefinedFunction)obj).Columns.Cast<NamedSmoObject>());
            }

            foreach (NamedSmoObject cp in colpar)
            {
                ParameterType pt = Constants.ParameterTypeMap[cp.GetType()];

                XmlElement pn = xml.CreateElement(pt.ToString().ToLowerInvariant());
                pn.Attributes.Append(xml.CreateAttribute(Constants.AttributeName)).Value = String.Format("[{0}]", cp.Name);

                foreach (ExtendedProperty p in ((IExtendedProperties)obj).ExtendedProperties)
                {
                    string pname = p.Name.Replace(Constants.SchemaMeta.ToLowerInvariant() + ".", String.Empty);
                    pn.Attributes.Append(xml.CreateAttribute(pname)).Value = p.Value.ToString();
                }

                node.AppendChild(pn);
            }
        }

    }
}
