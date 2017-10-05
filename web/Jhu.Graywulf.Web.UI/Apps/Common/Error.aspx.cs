using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.Common
{
    public partial class Error : PageBase
    {
        public static string GetUrl()
        {
            return Jhu.Graywulf.Web.UI.Constants.PageUrlError;
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

#if BREAKDEBUG
                if (ex != null && System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
#endif

                if (ex != null)
                {
                    Message.Text = ex.Message;
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/", false);
        }

        protected void Inquiry_Click(object sender, EventArgs e)
        {
            Response.Redirect(Feedback.GetErrorReportUrl(), false);
        }
    }
}