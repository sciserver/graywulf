using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class JobForm : FederationUserControlBase
    {
        private Job job;

        public Job Job
        {
            get { return job; }
            set
            {
                job = value;
                UpdateForm();
            }
        }

        public void UpdateForm()
        {
            name.Text = job.Name;
            comments.Text = job.Comments;
            dateCreated.Text = Util.DateFormatter.Format(job.DateCreated);
            dateStarted.Text = Util.DateFormatter.Format(job.DateStarted);
            dateFinished.Text = Util.DateFormatter.Format(job.DateFinished);
            status.Status = job.Status;
        }
    }
}