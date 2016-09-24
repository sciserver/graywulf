using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.Common
{
    public partial class Error : System.Web.UI.Page
    {
        public static string GetUrl()
        {
            return "~/Apps/Common/Error.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ex = Server.GetLastError();
                if (ex == null)
                {
                    ex = (Exception)Session[Jhu.Graywulf.Web.UI.Constants.SessionException];
                }

                if (ex != null)
                {
                    Message.Text = ex.Message;
                    FeedbackLink.NavigateUrl = Feedback.GetErrorReportUrl();
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/", false);
        }
    }
}