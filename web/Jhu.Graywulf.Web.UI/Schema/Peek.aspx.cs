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
    public partial class Peek : CustomPageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Schema/Peek.aspx?objid={0}", objid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RenderTable()
        {
            var tableOrView = (schema::TableOrView)FederationContext.SchemaManager.GetDatabaseObjectByKey(Request.QueryString["objid"]);

            var codegen = SqlCodeGeneratorFactory.CreateCodeGenerator(tableOrView.Dataset);

            var sql = codegen.GenerateSelectStarQuery(tableOrView, 100);

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