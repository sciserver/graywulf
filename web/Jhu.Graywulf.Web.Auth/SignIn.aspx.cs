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

            Session[Web.UI.Constants.SessionAuthenticator] = key;

            a.RedirectToLoginPage();
        }

        private Authenticator CreateAuthenticator(string key)
        {
            var parts = key.Split('|');
            var af = AuthenticatorFactory.Create(RegistryContext.Domain);
            var a = af.GetInteractiveAuthenticator(parts[0], parts[1]);

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
                var auth = af.GetInteractiveAuthenticator(identity.Protocol, identity.AuthorityUri);

                AuthorityName.Text = auth.DisplayName;
                Identifier.Text = identity.Identifier;

                RegisterLink2.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            }

            SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle]);

            RegisterLink.NavigateUrl = Jhu.Graywulf.Web.Auth.User.GetUrl(ReturnUrl);
            ActivateLink.NavigateUrl = Jhu.Graywulf.Web.Auth.Activate.GetUrl(ReturnUrl);
            ResetLink.NavigateUrl = Jhu.Graywulf.Web.Auth.RequestReset.GetUrl(ReturnUrl);
        }

        private void CreateAuthenticatorButtons()
        {
            var af = AuthenticatorFactory.Create(RegistryContext.Domain);

            Authenticators.Controls.Add(new LiteralControl("<ul>"));

            foreach (var au in af.GetInteractiveAuthenticators())
            {
                var b = new LinkButton()
                {
                    CausesValidation = false,
                    Text = au.DisplayName,
                    ToolTip = String.Format("Log on using {0}.", au.DisplayName),
                    CommandArgument = String.Format("{0}|{1}", au.ProtocolName, au.AuthorityUri)
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

        /// <summary>
        /// Authenticates a post-back request with certain authenticators
        /// (OpenID etc which requires a post-back from a remote server)
        /// </summary>
        private void Authenticate()
        {
            var key = (string)Session[Web.UI.Constants.SessionAuthenticator];

            if (key != null)
            {
                // The postback needs to be handled by a specific authenticator
                var authenticator = CreateAuthenticator(key);

                // Now that the postback has happened, remove the session data
                Session[Web.UI.Constants.SessionAuthenticator] = null;

                // Authenticate the response based on the value received from
                // the borswer
                var response = authenticator.Authenticate(new AuthenticationRequest(HttpContext.Current));

                // If the resposen contains a valid principal it means the authentication
                // was successful. We need to load the user from the registry now.
                if (response.Principal != null)
                {
                    var gwip = new GraywulfIdentityProvider(RegistryContext.Domain);
                    var identity = response.Principal.Identity;

                    // This call will load the user from the registry. If the authenticator
                    // is marked as master, it also created the user if necessary
                    gwip.LoadOrCreateUser(identity);

                    if (identity.IsAuthenticated)
                    {
                        // TODO: pass the response here, because we will need
                        // to set headers etc.
                        RedirectAuthenticatedUser(identity.User);
                    }
                    else
                    {
                        // User doesn't exist. It can be either associated with
                        // an existing one, or registration is offered. In the
                        // latter case, save user data received from the authentication
                        // authority
                        TemporaryPrincipal = response.Principal;
                    }
                }
            }
        }

        /// <summary>
        /// Authenticates a user coming in with a username and a password.
        /// </summary>
        private void LoginUser()
        {
            AuthenticationResponse response = null;
            var ip = IdentityProvider.Create(RegistryContext.Domain);

            // Try to authenticate the user.
            // It might happen that the user is awaiting activation.
            response = ip.VerifyPassword(Username.Text, Password.Text);

            // Get user from the response
            user = response.Principal.Identity.User;

            if (user.IsActivated)
            {
                RegistryContext.UserGuid = user.Guid;
                RegistryContext.UserName = user.Name;

                // If there's any temporary identifier set, associate with the user
                if (TemporaryPrincipal != null)
                {
                    var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                    ip.AddUserIdentity(user, identity);

                    TemporaryPrincipal = null;
                }
            }
        }

        private void RedirectAuthenticatedUser(Registry.User user)
        {
            // TODO return cookies and headers here
            // TODO: solve disabled keystone user problem here

            if (user.IsActivated)
            {
                FormsAuthentication.RedirectFromLoginPage(user.GetFullyQualifiedName(), Remember.Checked);
            }
            else
            {
                Response.Redirect(Activate.GetUrl(ReturnUrl));
            }
        }
    }
}