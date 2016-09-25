using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.UI.Apps.Auth
{
    public partial class SignOut : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/Apps/Auth/SignOut.aspx?ReturnUrl={0}", returnUrl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (FormsAuthentication.IsEnabled)
            {
                FormsAuthentication.SignOut();
            }

            // Sign out from authenticators
            var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
            wam.DeleteAuthResponseHeaders();

            Session.Abandon();

            ShortTitle.Text = (string)Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle];
            Ok.Attributes.Add("onClick", Util.UrlFormatter.GetClientRedirect(ReturnUrl));
        }
    }
}