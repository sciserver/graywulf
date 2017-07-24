using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return Constants.PageUrlDefault;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WelcomeForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle]);
        }
    }
}