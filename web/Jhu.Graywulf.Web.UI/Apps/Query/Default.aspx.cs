using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public partial class Default : QueryPageBase
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
            try
            {
                CreateQueryJob(GetQueryString(), JobQueue.Unknown);
                ShowMessage("Query OK.", Color.Black);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void ExecuteQuick_Click(object sender, EventArgs e)
        {
            try
            {
                var q = CreateQueryJob(GetQueryString(), JobQueue.Quick);
                if (q != null)
                {
                    var ji = ScheduleQuery(q);
                    Response.Redirect(Progress.GetUrl(ji.Guid));
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void ExecuteLong_Click(object sender, EventArgs e)
        {
            try
            {
                var q = CreateQueryJob(GetQueryString(), JobQueue.Long);

                if (q != null)
                {
                    ScheduleQuery(q);
                    Response.Redirect(Jobs.Default.GetUrl(), false);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
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

        private void HandleException(Exception ex)
        {
            // TODO: implement if necessary
            /*catch (ValidatorException ex)
            {
            }
            catch (NameResolverException ex)
            {
            }
            catch (ParserException ex)
            {
            }
            catch (Exception ex)
            {
            }*/


            // TODO: remove this case once all exceptions are handled correctly
            ShowMessage(String.Format("Query error: {0}", ex.Message), Color.Red);
        }

        protected void HideMessage()
        {
            messageBar.Visible = false;
        }

        protected void ShowMessage(string msg, Color color)
        {
            messageBar.Text = msg;
            messageBar.ForeColor = color;
            messageBar.Visible = true;
        }
    }
}