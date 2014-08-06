using System;
using System.Drawing;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Query
{
    public partial class Default : UserPageBase
    {
        public static string GetUrl()
        {
            return "~/Query/Default.aspx";
        }

        #region Event handlers

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

        /// <summary>
        /// Executes when the syntax check button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Check_Click(object sender, EventArgs e)
        {
            var q = CreateQueryJob(JobQueue.Unknown);
            VerifyQuery(q);
        }

        protected void ExecuteQuick_Click(object sender, EventArgs e)
        {
            var q = CreateQueryJob(JobQueue.Quick);

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
            var q = CreateQueryJob(JobQueue.Long);

            if (VerifyQuery(q) != null)
            {
                ScheduleQuery(q);
                Response.Redirect(Jobs.Default.GetUrl());
            }
        }

        // TODO: use pop-up windows instead for results
        protected void CloseResults_Click(object sender, EventArgs e)
        {
            ResultsDiv.Visible = false;
            CloseResults.Visible = false;
        }

        #endregion

        /// <summary>
        /// Returns the entire or only selected portion of the query.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a query job from the query string.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private QueryJob CreateQueryJob(JobQueue queue)
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
            var q = queryJob.CreateQuery(FederationContext);
            string message;

            if (q.Verify(out message))
            {
                Message.BackColor = Color.Green;
                Message.Text = message;
                return q;
            }
            else
            {
                Message.BackColor = Color.Red;
                Message.Text = message;
                return null;
            }
        }

        /// <summary>
        /// Schedules a query for execution.
        /// </summary>
        /// <param name="queryJob"></param>
        /// <returns></returns>
        protected JobInstance ScheduleQuery(QueryJob queryJob)
        {
            queryJob.Schedule(FederationContext);
            return queryJob.JobInstance;
        }
    }
}