using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class QueryDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (QueryJob)Job;

            edit.NavigateUrl = EditQuery.GetUrl(job.Guid);
            output.Text = job.Output;

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