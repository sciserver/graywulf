using System;
using System.Drawing;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;

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
            var q = CreateQuery();
        }

        protected void ExecuteQuick_Click(object sender, EventArgs e)
        {
            string queuename = String.Format("{0}.{1}", Federation.ControllerMachine.GetFullyQualifiedName(), Jhu.Graywulf.Registry.Constants.QuickQueueName);

            var q = CreateQuery();
            if (q != null)
            {
                q.Destination = new IO.Tasks.DestinationTable(
                    MyDBDataset,
                    MyDBDataset.DatabaseName,
                    MyDBDataset.DefaultSchemaName,
                    "quickResults",
                    TableInitializationOptions.Drop | TableInitializationOptions.Create);

                var ji = ScheduleQuery(queuename, q);

                LastQueryJobGuid = ji.Guid;

                ResultsDiv.Visible = true;
                CloseResults.Visible = true;
            }
        }

        protected void ExecuteLong_Click(object sender, EventArgs e)
        {
            string queuename = String.Format("{0}.{1}", Federation.ControllerMachine.GetFullyQualifiedName(), Jhu.Graywulf.Registry.Constants.LongQueueName);

            var q = CreateQuery();
            if (q != null)
            {
                ScheduleQuery(queuename, q);
                Response.Redirect("~/jobs/");
            }
        }

        protected QueryBase CreateQuery()
        {
            try
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

                var q = QueryFactory.Create(RegistryContext).CreateQuery(query, ExecutionMode.Graywulf, OutputTable.Text);
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

        protected JobInstance ScheduleQuery(string queuename, QueryBase q)
        {
            var job = QueryFactory.Create(RegistryContext).ScheduleAsJob(q, queuename, Comments.Text);
            job.Save();

            return job;
        }

        protected void CloseResults_Click(object sender, EventArgs e)
        {
            ResultsDiv.Visible = false;
            CloseResults.Visible = false;
        }
    }
}