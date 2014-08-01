using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class Error : System.Web.UI.UserControl
    {
        private Job job;

        public Job Job
        {
            set { this.job = value; }
            get { return this.job; }
        }

        protected void Inquiry_Click(object sender, EventArgs e)
        {
            Response.Redirect(SendInquiry.NavigateUrl);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            UpdateForm();
        }

        private void UpdateForm()
        {
            SendInquiry.NavigateUrl = Jhu.Graywulf.Web.Feedback.GetJobErrorUrl(job.Guid);
            ExceptionMessage.Text = job.Error;
        }
    }
}