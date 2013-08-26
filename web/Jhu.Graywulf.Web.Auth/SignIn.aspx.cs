using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class SignIn : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/SignIn.aspx?ReturnUrl={0}", returnUrl);
        }

        private Jhu.Graywulf.Registry.User user;

        protected override void EnsureUserIdentified()
        {
            // Override default behavior to do nothing
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle]);

            RegisterLink.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            ActivateLink.NavigateUrl = Jhu.Graywulf.Web.Auth.Activate.GetUrl(ReturnUrl);
            ResetLink.NavigateUrl = Jhu.Graywulf.Web.Auth.RequestReset.GetUrl(ReturnUrl);
        }

        protected void PasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Attempt to log in with supplied credentials
            try
            {
                var uu = new UserFactory(RegistryContext);
                user = uu.LoginUser(Domain, Username.Text, Password.Text);

                RegistryContext.UserGuid = user.Guid;
                RegistryContext.UserName = user.Name;

                args.IsValid = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                args.IsValid = false;
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (user.DeploymentState != DeploymentState.Deployed)
                {
                    Response.Redirect(Activate.GetUrl(ReturnUrl));
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(user.Guid.ToString(), Remember.Checked);
                }
            }
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl));
        }
    }
}