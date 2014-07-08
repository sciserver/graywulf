using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Jhu.Graywulf.Schema;
using schema = Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Data;

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
        }

        /* TODO: delete?
        private void GetServerSettings(schema::DatasetBase ds, out string connectionString, out DbProviderFactory dbf)
        {           
            EntityFactory ef = new EntityFactory(RegistryContext);
            Entity db = ef.LoadEntity(EntityType.Unknown, Jhu.Graywulf.Registry.AppSettings.FederationName, ds.Name);

            if (StringComparer.InvariantCultureIgnoreCase.Compare(ds.Name, FederationContext.MyDBDatabaseDefinition.Name) == 0)
            {
                // In case of myDB
                connectionString = ((schema::SqlServer.SqlServerDataset)FederationContext.SchemaManager.Datasets[FederationContext.MyDBDatabaseDefinition.Name]).ConnectionString;
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
        }*/

        protected void RenderTable()
        {
            var tableOrView = (schema::TableOrView)FederationContext.SchemaManager.GetDatabaseObjectByKey(Request.QueryString["objid"]);

            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(tableOrView.Dataset);

            var sql = codegen.GenerateSelectStarQuery(tableOrView, 100);

            DbProviderFactory dbf;
            string cstr;

            using (var cn = tableOrView.Dataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(tableOrView.Dataset, cn.CreateCommand()))
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        RenderTable(Response.Output, dr);
                    }
                }
            }
        }

        private void RenderTable(TextWriter writer, ISmartDataReader dr)
        {
            writer.WriteLine("<table border=\"1\" cellspacing=\"0\" style=\"border-collapse:collapse\">");

            // header
            writer.WriteLine("<tr>");

            var columns = dr.GetColumns();

            for (int i = 0; i < columns.Count; i++)
            {
                writer.WriteLine(
                    "<td class=\"header\" nowrap>{0}<br />{1}</td>",
                    columns[i].Name,
                    columns[i].DataType.NameWithLength);
            }

            writer.WriteLine("</tr>");

            // Rows
            while (dr.Read())
            {
                writer.WriteLine("<tr>");

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    writer.WriteLine("<td nowrap>{0}</td>", dr.GetValue(i).ToString());
                }

                writer.WriteLine("</tr>");
            }

            // Footer
            writer.WriteLine("</table>");
        }
    }
}