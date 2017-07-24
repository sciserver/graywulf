using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Apps.Auth
{
    public partial class SignIn : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/Apps/Auth/SignIn.aspx?ReturnUrl={0}", returnUrl);
        }

        private AuthenticationResponse authResponse;

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            // The are various ways a user can arrive to this page:
            //
            // 1. When a user owns no authentication ticket or cookie and wants to access
            //    a restricted resource they are redirected here.
            // 2. The same happens when the user owns a ticket but the web site requires
            //    it in a different form. For instance, instead of a cookie, in the form of
            //    a query string parameter. It is a common practice when cross-domain
            //    authentication should happen which isn't supported by the domain-specific
            //    cookies.
            // 3. When a user is trying to sing in using third-party identity providers, for
            //    example OpenID. In this case the user is redirected back to this page by
            //    the third party after successful authentication. In this case the session holds
            //    a reference to the authenticator object used to send the user to the third party.

            // First try to authenticate by headers and cookies in the request only.
            // If that works, the user will be redirected back to the URL they were
            // coming from and execution will not get to the update form stage.

            if (User.Identity is GraywulfIdentity && User.Identity.IsAuthenticated)
            {
                // The user is already authenticated
                RedirectAuthenticatedUser();
            }
            else if (AuthenticateByRequest())
            {
                // The user is authenticated by a third party and returning now
                // with a valid ticket
                RedirectAuthenticatedUser();
            }
            else
            {
                // If the authentication is unsuccessful based on request data,
                // we need to display the form
                UpdateForm();
            }
        }

        /// <summary>
        /// Fires when the user clicks on the Sign in button after providing a
        /// username and a password.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void PasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Attempt to log in with supplied username and password
            args.IsValid = AuthenticateByForm();
        }

        /// <summary>
        /// Fires when the user clicks on the Sign in button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                RedirectAuthenticatedUser();
            }
        }

        /// <summary>
        /// Fires when the user click on the Register button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Register_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Auth.User.GetUrl(ReturnUrl), false);
        }

        /// <summary>
        /// Fires when the users clicks on any of the identity service links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AuthenticatorButton_Click(object sender, EventArgs e)
        {
            var key = ((IButtonControl)sender).CommandArgument;
            var a = CreateAuthenticator(key);

            Session[Web.UI.Constants.SessionAuthenticator] = key;

            a.RedirectToLoginPage();
        }

        /// <summary>
        /// Fires when the user cancels the authentication using a
        /// third-party identity service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelIdentifier_Click(object sender, EventArgs e)
        {
            TemporaryPrincipal = null;
            UpdateForm();
        }

        #endregion
        #region Form update

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

                var af = AuthenticationFactory.Create(RegistryContext.Domain);
                var auth = af.GetAuthentication(identity.Protocol, identity.AuthorityUri);

                AuthorityName.Text = auth.DisplayName;
                Identifier.Text = identity.Identifier;

                RegisterLink2.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.User.GetUrl(ReturnUrl);
            }

            SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle]);

            RegisterLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.User.GetUrl(ReturnUrl);
            ActivateLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.Activate.GetUrl(ReturnUrl);
            ResetLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.RequestReset.GetUrl(ReturnUrl);
        }

        #endregion
        #region Third party authenticators

        private Authentication CreateAuthenticator(string key)
        {
            var parts = key.Split('|');
            var af = AuthenticationFactory.Create(RegistryContext.Domain);
            var a = af.GetAuthentication(parts[0], parts[1]);

            return a;
        }

        private void CreateAuthenticatorButtons()
        {
            var af = AuthenticationFactory.Create(RegistryContext.Domain);

            Authenticators.Controls.Add(new LiteralControl("<ul>"));

            foreach (var au in af.GetAuthentications(AuthenticatorProtocolType.WebInteractive))
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

        #endregion
        #region Authentication logic

        /// <summary>
        /// Authenticates a post-back request with certain authenticators
        /// (OpenID etc which requires a post-back from a remote server).
        /// </summary>
        private bool AuthenticateByRequest()
        {
            var key = (string)Session[Web.UI.Constants.SessionAuthenticator];

            if (key != null)
            {
                // The postback needs to be handled by a specific authenticator
                var authenticator = CreateAuthenticator(key);

                // Now that the postback has happened, remove the session data
                Session[Web.UI.Constants.SessionAuthenticator] = null;

                // Authenticate the response based on the value received from the browser
                var request = new AuthenticationRequest(HttpContext.Current);
                var response = new AuthenticationResponse(request);
                authenticator.Authenticate(request, response);

                // If the resposen contains a valid principal it means the authentication
                // was successful. We need to load the user from the registry now.
                if (response.Principal != null)
                {
                    var gwip = new GraywulfIdentityProvider(RegistryContext.Domain);

                    // This call will load the user from the registry. If the authenticator
                    // is marked as master, it also created the user if necessary
                    gwip.LoadOrCreateUser(response.Principal);

                    if (response.Principal.Identity.IsAuthenticated)
                    {
                        return true;
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

            return false;
        }

        /// <summary>
        /// Authenticates a user coming in with a username and a password.
        /// </summary>
        private bool AuthenticateByForm()
        {
            // Try to authenticate the user.
            // It might happen that the user is awaiting activation.
            try
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);

                var request = new AuthenticationRequest(HttpContext.Current)
                {
                    Username = Username.Text,
                    Password = Password.Text,
                };
                authResponse = ip.VerifyPassword(request);

                // Get user from the response
                var user = authResponse.Principal.Identity.User;

                if (user.IsActivated)
                {
                    RegistryContext.UserReference.Value = user;

                    // If there's any temporary identifier set, associate with the user
                    if (TemporaryPrincipal != null)
                    {
                        var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                        ip.AddUserIdentity(user, identity);

                        TemporaryPrincipal = null;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Load failed logon attempt
                LogError(ex);

#if BREAKDEBUG
                System.Diagnostics.Debugger.Break();
#endif

                // TODO: there five different exceptions thrown by different parts of
                // the code when access is denied. Sort these out for once.

                // This exception means we cannot authenticate the user
                return false;
            }
        }

        private void RedirectAuthenticatedUser()
        {
            // If the user is authenticated automatically authResponce needs to
            // be taken from the responsible http module
            if (authResponse == null)
            {
                var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
                authResponse = wam.AuthenticationResponse;
            }

            var user = authResponse.Principal.Identity.User;

            if (user.IsActivated)
            {
                if (FormsAuthentication.IsEnabled)
                {
                    authResponse.AddFormsAuthenticationTicket(Remember.Checked);
                }
                
                authResponse.SetResponseHeaders(Response);

                var url = authResponse.GetReturnUrl();
                Response.Redirect(url, false);
            }
            else
            {
                Response.Redirect(Activate.GetUrl(ReturnUrl), false);
            }
        }

        #endregion
    }
}