using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Jhu.Graywulf.Schema;
using schema = Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.CodeGeneration;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Peek : FederationPageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/Schema/Peek.aspx?objid={0}", objid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RenderTable()
        {
            var tableOrView = (schema::TableOrView)FederationContext.SchemaManager.GetDatabaseObjectByKey(Request.QueryString["objid"]);
            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(tableOrView.Dataset);
            var sql = codegen.GenerateSelectStarQuery(tableOrView, 100);
            IDbConnection cn = null;

            // To peek into data, pick a server
            if (tableOrView.Dataset is GraywulfDataset)
            {
                var ds = (GraywulfDataset)tableOrView.Dataset;

                if (!ds.DatabaseDefinitionReference.IsEmpty)
                {
                    var dd = ds.DatabaseDefinitionReference.Value;
                    dd.RegistryContext = RegistryContext;

                    var di = dd.GetRandomDatabaseInstance(Registry.Constants.ProdDatabaseVersionName);
                    var cstr = di.GetConnectionString();

                    cn = new SqlConnection(cstr.ConnectionString);
                    cn.Open();
                }
            }

            if (cn == null)
            {
                cn = tableOrView.Dataset.OpenConnection();
            }

            using (var cmd = new SmartCommand(tableOrView.Dataset, cn.CreateCommand()))
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    RenderTable(Response.Output, dr);
                }
            }

            cn.Dispose();
        }

        private void RenderTable(TextWriter writer, ISmartDataReader dr)
        {
            writer.WriteLine("<table border=\"1\" cellspacing=\"0\" style=\"border-collapse:collapse\">");

            // header
            writer.WriteLine("<tr>");

            var columns = dr.Columns;

            for (int i = 0; i < columns.Count; i++)
            {
                writer.WriteLine(
                    "<td class=\"header\" nowrap>{0}<br />{1}</td>",
                    columns[i].Name,
                    columns[i].DataType.TypeNameWithLength);
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