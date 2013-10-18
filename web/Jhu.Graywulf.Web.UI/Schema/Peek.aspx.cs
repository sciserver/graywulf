using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using schema = Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlCodeGen;

namespace Jhu.Graywulf.Web.UI.Schema
{
    public partial class Peek : PageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Schema/Peek.aspx?objid={0}", objid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var tableOrView = (schema::TableOrView)SchemaManager.GetDatabaseObjectByKey(Request.QueryString["objid"]);

            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(tableOrView.Dataset);

            var sql = codegen.GenerateSelectStarQuery(tableOrView, 100);

            DbProviderFactory dbf;
            string cstr;

            GetServerSettings(tableOrView.Dataset, out cstr, out dbf);

            using (var cn = dbf.CreateConnection())
            {
                cn.ConnectionString = tableOrView.Dataset.ConnectionString;
                cn.Open();

                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        RenderTable(dr);
                    }
                }
            }
        }

        private void GetServerSettings(schema::DatasetBase ds, out string connectionString, out DbProviderFactory dbf)
        {           
            EntityFactory ef = new EntityFactory(RegistryContext);
            Entity db = ef.LoadEntity(Jhu.Graywulf.Registry.Federation.AppSettings.FederationName, ds.Name);
            

            if (StringComparer.InvariantCultureIgnoreCase.Compare(ds.Name, MyDBDatabaseDefinition.Name) == 0)
            {
                // In case of myDB
                connectionString = ((schema::SqlServer.SqlServerDataset)SchemaManager.Datasets[MyDBDatabaseDefinition.Name]).ConnectionString;
                dbf = DbProviderFactories.GetFactory(ds.ProviderName);
            }
            else if (db is DatabaseDefinition)
            {
                // In case of a Graywulf database definition
                DatabaseDefinition dd = (DatabaseDefinition)db;

                dd.LoadDatabaseVersions(false);
                DatabaseVersion rs = dd.DatabaseVersions.Values.First(r => r.Name == "HOT");        // ***** TODO: this should come from the job settings...

                dd.LoadDatabaseInstances(false);
                List<DatabaseInstance> dis = new List<DatabaseInstance>(dd.DatabaseInstances.Values.Where(dii => dii.DatabaseVersionReference.Guid == rs.Guid));

                // Pick a random server
                Random rnd = new Random();
                DatabaseInstance di = dis[rnd.Next(dis.Count)];

                connectionString = di.GetConnectionString().ConnectionString;
                dbf = System.Data.SqlClient.SqlClientFactory.Instance;
            }
            else if (db is RemoteDatabase)
            {
                RemoteDatabase rd = (RemoteDatabase)db;

                connectionString = ds.GetSpecializedConnectionString(rd.ConnectionString, rd.IntegratedSecurity, rd.Username, rd.Password, false);
                dbf = DbProviderFactories.GetFactory(ds.ProviderName);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void RenderTable(IDataReader dr)
        {
            StringWriter output = new StringWriter();

            output.WriteLine("<table border=\"1\" cellspacing=\"0\" style=\"border-collapse:collapse\">");



            // header
            output.WriteLine("<tr>");

            for (int i = 0; i < dr.FieldCount; i++)
            {
                output.WriteLine("<td class=\"header\" nowrap>{0}<br />{1}</td>", dr.GetName(i), dr.GetDataTypeName(i));
            }

            output.WriteLine("</tr>");

            while (dr.Read())
            {
                output.WriteLine("<tr>");

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    output.WriteLine("<td nowrap>{0}</td>", dr.GetValue(i).ToString());
                }

                output.WriteLine("</tr>");
            }


            output.WriteLine("</table>");

            dataTable.Text = output.ToString();
        }
    }
}