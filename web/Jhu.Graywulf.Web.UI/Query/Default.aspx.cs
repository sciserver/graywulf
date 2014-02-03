using System;
using System.Drawing;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.Api;

namespace Jhu.Graywulf.Web.UI.Query
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return "~/Query/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && HasQueryInSession())
            {
                string query;
                int[] selection;
                bool selectedonly;

                GetQueryFromSession(out query, out selection, out selectedonly);

                Query.Text = query;
                Query.SelectionCoords = selection;
                SelectedOnly.Checked = selectedonly;
            }
            else
            {
                SetQueryInSession(Query.Text, Query.SelectionCoords, SelectedOnly.Checked);
            }

            Message.Text = String.Empty;
            Message.ForeColor = Color.White;
        }

        protected void Check_Click(object sender, EventArgs e)
        {
            var q = CreateQuery(JobQueue.Unknown);
            VerifyQuery(q);
        }

        protected void ExecuteQuick_Click(object sender, EventArgs e)
        {
            var q = CreateQuery(JobQueue.Quick);

            if (VerifyQuery(q) != null)
            {
                var ji = ScheduleQuery(q);

                LastQueryJobGuid = ji.Guid;

                ResultsDiv.Visible = true;
                CloseResults.Visible = true;
            }
        }

        protected void ExecuteLong_Click(object sender, EventArgs e)
        {
            var q = CreateQuery(JobQueue.Long);
            
            if (VerifyQuery(q) != null)
            {
                ScheduleQuery(q);
                Response.Redirect(Jobs.Default.GetUrl());
            }
        }

        protected void CloseResults_Click(object sender, EventArgs e)
        {
            ResultsDiv.Visible = false;
            CloseResults.Visible = false;
        }

        private string GetQueryString()
        {
            string query;
            if (SelectedOnly.Checked)
            {
                query = Query.SelectedText;
            }
            else
            {
                query = Query.Text;
            }

            return query;
        }

        private QueryJob CreateQuery(JobQueue queue)
        {
            return new QueryJob(GetQueryString(), queue);
        }

        /// <summary>
        /// Create a query, verify it and display errors, if any
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private QueryBase VerifyQuery(QueryJob queryJob)
        {
            try
            {
                var q = queryJob.CreateQuery(FederationContext);
                q.Verify();

                Message.BackColor = Color.Green;
                Message.Text = "Query OK.";

                return q;
            }
            catch (ValidatorException ex)
            {
                Message.BackColor = Color.Red;
                Message.Text = String.Format("Query error: {0}", ex.Message);
            }
            catch (NameResolverException ex)
            {
                Message.BackColor = Color.Red;
                Message.Text = String.Format("Query error: {0}", ex.Message);
            }
            catch (ParserException ex)
            {
                Message.BackColor = Color.Red;
                Message.Text = String.Format("Query error: {0}", ex.Message);
            }

            return null;
        }

        protected JobInstance ScheduleQuery(QueryJob queryJob)
        {
            var jobInstance = queryJob.Schedule(FederationContext);
            return jobInstance;
        }
    }
}