using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ImportDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (ImportJob)Job;

            destination.Text = string.Format("{0}:{1}", job.Destination.Dataset, job.Destination.Table);

            if (job.Uri != null)
            {
                source.Text = job.Uri.ToString();
            }

            if (job.FileFormat != null && job.FileFormat.MimeType != null)
            {
                fileFormat.Text = job.FileFormat.MimeType;
            }
        }
    }
}