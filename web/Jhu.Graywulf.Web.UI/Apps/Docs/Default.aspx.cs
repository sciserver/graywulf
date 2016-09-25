using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.Docs
{
    public partial class Default : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Docs/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FederationNameLabel.Text = RegistryContext.Federation.ShortTitle;
        }
    }
}