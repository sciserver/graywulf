using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class ErrorForm : System.Web.UI.UserControl
    {
        private Job job;

        public Job Job
        {
            get { return this.job; }
            set
            {
                this.job = value;
                UpdateForm();
            }
        }

        private void UpdateForm()
        {
            SendInquiry.NavigateUrl = Jhu.Graywulf.Web.UI.Feedback.GetJobErrorUrl(job.Guid);
            ExceptionMessage.Text = job.Error;
        }

        protected void Inquiry_Click(object sender, EventArgs e)
        {
            Response.Redirect(SendInquiry.NavigateUrl, false);
        }
    }
}