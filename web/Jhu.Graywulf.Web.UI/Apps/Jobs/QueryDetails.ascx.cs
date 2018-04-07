using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class QueryDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (QueryJob)Job;

            // TODO: here we add links to all output tables
            output.Text = "";

            if (job.Output != null)
            {
                foreach (var table in job.Output)
                {
                    output.Text += table;
                }
            }

            if (string.IsNullOrWhiteSpace(job.Query))
            {
                edit.Visible = false;
                query.Text = "N/A";
            }
            else
            {
                edit.NavigateUrl = EditQuery.GetUrl(job.Guid);

                if (job.Query.Length < 100)
                {
                    query.Text = job.Query;
                }
                else
                {
                    query.Text = job.Query.Substring(0, 100) + "...";
                }
            }
        }
    }
}