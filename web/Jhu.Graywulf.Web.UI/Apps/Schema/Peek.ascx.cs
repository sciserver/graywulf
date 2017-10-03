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
using System.Web.UI;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Peek : SchemaItemView<TableOrView>
    {
        public static string GetUrl(string objid)
        {
            return Default.GetUrl(Default.SchemaView.Peek, objid);
        }

        public override void UpdateView()
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderTable(writer);
        }

        protected void RenderTable(TextWriter writer)
        {
            var tableOrView = Item;
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
                    RenderTable(writer, dr);
                }
            }

            cn.Dispose();
        }

        private void RenderTable(TextWriter writer, ISmartDataReader dr)
        {
            writer.WriteLine("<div class=\"dock-fill dock-scroll\">");
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
            writer.WriteLine("</div>");
        }
    }
}