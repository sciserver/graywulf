using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
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

        protected void RenderOutput()
        {
            Response.Expires = -1;

            try
            {
                var ji = new JobInstance(RegistryContext);
                ji.Guid = LastQueryJobGuid;
                ji.Load();

                switch (ji.JobExecutionStatus)
                {
                    case JobExecutionState.Completed:
                        RenderResults(ji);
                        break;
                    case JobExecutionState.Scheduled:
                    case JobExecutionState.Starting:
                    case JobExecutionState.Executing:
                        RenderExecuting();
                        break;
                    default:
                        RenderFailed(ji);
                        break;
                }
            }
            catch (Exception ex)
            {
                RenderException(ex);
            }
        }

        private void RenderExecuting()
        {
            Response.Output.WriteLine("Executing query...");
            Response.Output.WriteLine("<script language=\"javascript\">refreshResults();</script>");
        }

        private void RenderResults(JobInstance ji)
        {
            var q = (QueryBase)ji.Parameters["Query"].GetValue();

            var codegen = new SqlServerCodeGenerator();

            string sql = codegen.GenerateTableSelectStarQuery(
                null,
                null,
                q.Destination.Table.SchemaName,
                q.Destination.Table.TableName,
                100);


            using (var cn = new SqlConnection())
            {
                cn.ConnectionString = SchemaManager.Datasets["MYDB"].ConnectionString;
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

        private void RenderTable(IDataReader dr)
        {
            var output = Response.Output;

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
        }

        private void RenderFailed(JobInstance ji)
        {
            Response.Output.WriteLine("<p>An exception occured: {0}</p>", ji.ExceptionMessage);
        }

        private void RenderException(Exception ex)
        {
            var error = LogError(ex);

            // Save exception to session for future use
            Session[Constants.SessionException] = ex;
            Session[Constants.SessionExceptionEventID] = error.EventId;

            Server.ClearError();

            Response.Output.WriteLine("<p>An exception occured: {0}</p>", ex.Message);
            Response.Output.WriteLine(
                "<p><a href=\"{0}\">Click here to report error.</a></p>",
                VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, Jhu.Graywulf.Web.Feedback.GetErrorReportUrl()));
        }
    }
}