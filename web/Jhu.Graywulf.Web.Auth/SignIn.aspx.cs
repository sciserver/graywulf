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
    public partial class SignIn : Jhu.Graywulf.Web.Auth.PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/SignIn.aspx?ReturnUrl={0}", returnUrl);
        }

        private Jhu.Graywulf.Registry.User user;

        protected void Page_Load(object sender, EventArgs e)
        {
            var unknownId = TemporaryPrincipal == null;

            SignInIntroPanel.Visible = !unknownId;
            UnknownIdentityIntroPanel.Visible = unknownId;
            SignInDetailsPanel.Visible = !unknownId;
            AuthenticatorPanel.Visible = unknownId;

            if (!unknownId)
            {
                CreateAuthenticatorButtons();
            }
            else
            {
                var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                AuthorityName.Text = identity.Authority;
            }

            SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle]);

            RegisterLink.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            ActivateLink.NavigateUrl = Jhu.Graywulf.Web.Auth.Activate.GetUrl(ReturnUrl);
            ResetLink.NavigateUrl = Jhu.Graywulf.Web.Auth.RequestReset.GetUrl(ReturnUrl);

            // Try all authentication methods.
            Authenticate();
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
                RedirectAuthenticatedUser(user);
            }
        }

        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl));
        }

        void AuthenticatorButton_Click(object sender, ImageClickEventArgs e)
        {
            // Redirect to interactive page
            var af = new AuthenticatorFactory();
            var parts = ((ImageButton)sender).CommandArgument.Split('|');
            var a = af.CreateInteractiveAuthenticator(parts[0], parts[1]);

            a.RedirectToLoginPage();
        }

        private void CreateAuthenticatorButtons()
        {
            var af = new AuthenticatorFactory();
            var aus = af.CreateInteractiveAuthenticators();

            for (int i = 0; i < aus.Length; i++)
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

            // Focus on the 'sign in' button
            Ok.Focus();
        }

        private void Authenticate()
        {
            // Try authenticate with all interactive authenticators
            var af = new AuthenticatorFactory();
            var aus = af.CreateInteractiveAuthenticators();
            for (int i = 0; i < aus.Length; i++)
            {
                var principal = aus[i].Authenticate();
                if (principal != null)
                {
                    var identity = (GraywulfIdentity)principal.Identity;
                    identity.LoadUser(RegistryContext.Domain);
                    if (identity.IsAuthenticated)
                    {
                        RedirectAuthenticatedUser(identity.User);
                    }
                    else
                    {
                        // User doesn't exist. It can be either associated with
                        // an existing one, or registration is offered. In the
                        // latter case, save user data received from the authentication
                        // authority

                        // TODO: we may trust certain identity services, so users coming
                        // from them could automatically registered without beging
                        // sent to the user form.

                        Session[Constants.SessionTempPrincipal] = principal;
                        Response.Redirect(Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl));
                    }
                }
            }
        }

        private void RedirectAuthenticatedUser(Registry.User user)
        {
            if (user.IsActivated)
            {
                Response.Redirect(Activate.GetUrl(ReturnUrl));
            }
            else
            {
                FormsAuthentication.RedirectFromLoginPage(user.GetFullyQualifiedName(), Remember.Checked);
            }
        }
    }
}