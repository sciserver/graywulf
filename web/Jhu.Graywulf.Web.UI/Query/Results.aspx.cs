using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using schema = Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.Query
{
    public partial class Results : PageBase
    {
        public static string GetUrl()
        {
            return "~/Query/Results.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var ji = new JobInstance(RegistryContext);
            ji.Guid = Guid.Parse(Request.QueryString["guid"]);
            ji.Load();

            var q = (QueryBase)ji.Parameters["Query"].GetValue();

            var codegen = new SqlServerCodeGenerator();

            string sql = codegen.GenerateTableSelectStarQuery(
                null,
                null,
                q.Destination.Table.SchemaName,
                q.Destination.Table.TableName,
                100);


            using (IDbConnection cn = new SqlConnection())
            {
                cn.ConnectionString = SchemaManager.Datasets["MYDB"].ConnectionString;
                cn.Open();

                using (IDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (IDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        RenderTable(dr);
                    }
                }
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