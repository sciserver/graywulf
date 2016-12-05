using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ExportDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (ExportJob)Job;

            source.Text = string.Format("{0}:{1}", job.Source.Dataset, job.Source.Table);

            if (job.Uri != null)
            {
                destination.Text = job.Uri.ToString();
            }

            if (job.FileFormat != null && job.FileFormat.MimeType != null)
            {
                fileFormat.Text = job.FileFormat.MimeType;
            }
        }
    }
}