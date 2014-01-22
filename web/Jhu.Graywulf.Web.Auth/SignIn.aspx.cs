using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.Security;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateAuthenticatorButtons();

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
                // Check if user was already activated. If not, redirect to activation page

                if (user.DeploymentState != DeploymentState.Deployed)
                {
                    Response.Redirect(Activate.GetUrl(ReturnUrl));
                }
                else
                {
                    FormsAuthentication.RedirectFromLoginPage(user.Name, Remember.Checked);
                }
            }
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl));
        }

        private void CreateAuthenticatorButtons()
        {
            var af = new AuthenticatorFactory();
            var aus = af.GetAuthenticators();

            for (int i = 0; i < aus.Length; i++)
            {
                if (aus[i].IsInteractive)
                {
                    var b = new ImageButton()
                    {
                        CausesValidation = false,
                        AlternateText = aus[i].DisplayName,
                        ToolTip = String.Format("Log on using {0}.", aus[i].DisplayName),
                        CommandArgument = String.Format("{0}|{1}", aus[i].Protocol, aus[i].Authority)
                    };

                    b.Click += new ImageClickEventHandler(AuthenticatorButton_Click);

                    Authenticators.Controls.Add(b);
                }
            }
        }

        void AuthenticatorButton_Click(object sender, ImageClickEventArgs e)
        {
            var parts = ((ImageButton) sender).CommandArgument.Split('|');

            var af = new AuthenticatorFactory();

            var a = af.GetAuthenticator(parts[0], parts[1]);

            a.RedirectToLoginPage();
        }
    }
}