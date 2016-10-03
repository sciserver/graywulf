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

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public partial class Default : FederationPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Query/Default.aspx";
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
                selectedOnly.Checked = selectedonly;
            }
            else
            {
                SetQueryInSession(Query.Text, Query.SelectionCoords, selectedOnly.Checked);
            }

            HideMessage();
        }

        /// <summary>
        /// Executes when the syntax check button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Check_Click(object sender, EventArgs e)
        {
            CreateQueryJob(JobQueue.Unknown);
        }

        protected void ExecuteQuick_Click(object sender, EventArgs e)
        {
            var q = CreateQueryJob(JobQueue.Quick);

            if (q != null)
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

            if (q != null)
            {
                ScheduleQuery(q);
                Response.Redirect(Jobs.Default.GetUrl(), false);
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
            if (selectedOnly.Checked)
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
            try
            {
                var queryJob = new QueryJob(GetQueryString(), queue);
                var query = queryJob.CreateQuery(FederationContext);

                query.Verify();

                ShowMessage("Query OK.", Color.Black);

                return queryJob;
            }
            /*catch (ValidatorException ex)
            {
            }
            catch (NameResolverException ex)
            {
            }
            catch (ParserException ex)
            {
            }*/
            catch (Exception ex)
            {
                // TODO: remove this case once all exceptions are handled correctly
                ShowMessage(String.Format("Query error: {0}", ex.Message), Color.Red);
            }
            
            return null;
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

        protected void HideMessage()
        {
            messagePanel.Visible = false;
        }

        protected void ShowMessage(string msg, Color color)
        {
            message.Text = msg;
            message.ForeColor = color;
            messagePanel.Visible = true;
        }
    }
}