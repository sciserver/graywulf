using System;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ErrorDetails : System.Web.UI.UserControl
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

        protected void UpdateForm()
        {
            this.Visible = job.HasError;
            errorInquiry.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Common.Feedback.GetJobErrorUrl(job.Guid);
            error.Text = job.Error;
        }
    }
}