using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.Web.Security;
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
            // Try all authentication methods.
            Authenticate();
            UpdateForm();
        }

        protected void PasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Attempt to log in with supplied credentials
            try
            {
                LoginUser();
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

        protected void CancelIdentifier_Click(object sender, EventArgs e)
        {
            TemporaryPrincipal = null;
            UpdateForm();
        }

        void AuthenticatorButton_Click(object sender, EventArgs e)
        {
            var key = ((IButtonControl)sender).CommandArgument;
            var a = CreateAuthenticator(key);

            Session[Constants.SessionAuthenticator] = key;

            a.RedirectToLoginPage();
        }

        private IInteractiveAuthenticator CreateAuthenticator(string key)
        {
            var parts = key.Split('|');
            var af = AuthenticatorFactory.Create(RegistryContext.Domain);
            var a = af.CreateInteractiveAuthenticator(parts[0], parts[1]);

            return a;
        }


        private void UpdateForm()
        {
            var unknownId = TemporaryPrincipal != null;

            SignInIntroPanel.Visible = !unknownId;
            UnknownIdentityIntroPanel.Visible = unknownId;
            SignInDetailsPanel.Visible = !unknownId;
            AuthenticatorPanel.Visible = !unknownId;

            if (!unknownId)
            {
                CreateAuthenticatorButtons();
            }
            else
            {
                var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;

                var af = AuthenticatorFactory.Create(RegistryContext.Domain);
                var auth = af.CreateInteractiveAuthenticator(identity.Protocol, identity.AuthorityUri);

                AuthorityName.Text = auth.DisplayName;
                Identifier.Text = identity.Identifier;
                
                RegisterLink2.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            }

            SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle]);

            RegisterLink.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            ActivateLink.NavigateUrl = Jhu.Graywulf.Web.Auth.Activate.GetUrl(ReturnUrl);
            ResetLink.NavigateUrl = Jhu.Graywulf.Web.Auth.RequestReset.GetUrl(ReturnUrl);
        }

        private void CreateAuthenticatorButtons()
        {
            var af = AuthenticatorFactory.Create(RegistryContext.Domain);
            var aus = af.CreateInteractiveAuthenticators();

            Authenticators.Controls.Add(new LiteralControl("<ul>"));
            for (int i = 0; i < aus.Length; i++)
            {
                var b = new LinkButton()
                {
                    CausesValidation = false,
                    Text = aus[i].DisplayName,
                    ToolTip = String.Format("Log on using {0}.", aus[i].DisplayName),
                    CommandArgument = String.Format("{0}|{1}", aus[i].Protocol, aus[i].AuthorityUri)
                };

                b.Click += new EventHandler(AuthenticatorButton_Click);

                Authenticators.Controls.Add(new LiteralControl("<li>"));
                Authenticators.Controls.Add(b);
                Authenticators.Controls.Add(new LiteralControl("</li>"));
            }
            Authenticators.Controls.Add(new LiteralControl("</ul>"));

            // Focus on the 'sign in' button
            Ok.Focus();
        }

        private void Authenticate()
        {
            var key = (string)Session[Constants.SessionAuthenticator];

            if (key != null)
            {
                var a = CreateAuthenticator(key);

                Session[Constants.SessionAuthenticator] = null;

                var principal = a.Authenticate();
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

                        TemporaryPrincipal = principal;
                    }

                }
            }
        }

        private void LoginUser()
        {
            user = IdentityProvider.VerifyPassword(Username.Text, Password.Text);

            RegistryContext.UserGuid = user.Guid;
            RegistryContext.UserName = user.Name;

            // If there's any temporary identifier set, associate
            // with the user
            if (TemporaryPrincipal != null)
            {
                var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                var ui = identity.CreateUserIdentity(user);
                ui.Save();

                TemporaryPrincipal = null;
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