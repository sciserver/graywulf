using System;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class JobDetails : System.Web.UI.UserControl
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

        protected virtual void UpdateForm()
        {
            cancel.Visible = job.CanCancel;
            cancel.NavigateUrl = Util.UrlFormatter.ToRelativeUrl(CancelJob.GetUrl(job.Guid));
        }
    }
}