using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class CopyDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (CopyJob)Job;

            source.Text = string.Format("{0}:{1}", job.Source.Dataset, job.Source.Table);
            destination.Text = string.Format("{0}:{1}", job.Destination.Dataset, job.Destination.Table);
        }
    }
}