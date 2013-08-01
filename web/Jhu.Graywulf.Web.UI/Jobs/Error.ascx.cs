using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class Error : System.Web.UI.UserControl
    {
        private JobDescription jobDescription;

        public JobDescription JobDescription
        {
            set
            {
                this.jobDescription = value;

                ExceptionMessage.Text = value.Job.ExceptionMessage;
            }
            get
            {
                return this.jobDescription;
            }
        }       

        protected void Page_Load(object sender, EventArgs e)
        {
            if (JobDescription != null)
            {
                SendInquiry.NavigateUrl = Jhu.Graywulf.Web.Feedback.GetJobErrorUrl(JobDescription.Guid);
            }
        }

        protected void Inquiry_Click(object sender, EventArgs e)
        {
            Response.Redirect(SendInquiry.NavigateUrl);
        }
    }
}