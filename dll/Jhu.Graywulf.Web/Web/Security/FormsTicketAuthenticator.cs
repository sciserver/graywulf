using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Authenticates requests based on a FormsAuthenticationTicket
    /// </summary>
    /// <remarks>
    /// This is used by WCF services to accept and process the same
    /// tickets that FormsAuthentication uses.
    /// </remarks>
    public class FormsTicketAuthenticator : Authenticator
    {
        #region Properties

        public override string ProtocolName
        {
            get { return Constants.ProtocolNameForms; }
        }

        public override AuthenticatorProtocolType ProtocolType
        {
            get { return AuthenticatorProtocolType.RestRequest; }
        }

        #endregion
        #region Constructors and initializers

        public FormsTicketAuthenticator()
        {
            InitializeMembers();
            FormsAuthentication.Initialize();
        }

        private void InitializeMembers()
        {
            AuthorityName = Constants.AuthorityNameGraywulf;
        }

        #endregion

        public override AuthenticationResponse Authenticate(AuthenticationRequest request)
        {
            // The logic implemented here is based on the .Net reference source
            // original class: FormAuthenticationModule

            // Since the tickets are encrypted, we can trust the information
            // inside, and take the name of the user from the ticket
            // directly. This saves us from looking into the database at every
            // single request.

            var response = new AuthenticationResponse();

            // Get the forms authentication cookie from the request
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                // Decrypt the token and check whether it's already expired
                var oldToken = FormsAuthentication.Decrypt(cookie.Value);
                if (oldToken != null && !oldToken.Expired)
                {
                    // The token hasn't expired yet
                    // Create a GraywulfPrincipal based on the ticket.
                    response.SetPrincipal(CreatePrincipal(oldToken));

#if false // TODO: this is ticket renewal logic. It is removed because no way to send cookies back now.
                    // Read the ticket from the token and renew it if sliding expiration is turned on
                    var ticket = oldToken;
                    if (FormsAuthentication.SlidingExpiration)
                    {
                        ticket = FormsAuthentication.RenewTicketIfOld(oldToken);
                    }                  

                    // Set special cookie path, if necessary
                    if (!ticket.CookiePath.Equals("/"))
                    {
                        cookie = request.Cookies[FormsAuthentication.FormsCookieName];
                        if (cookie != null)
                        {
                            cookie.Path = ticket.CookiePath;
                        }
                    }

                    // If the ticket has been renewed, it has to be encryted and sent to the client
                    if (ticket != oldToken)
                    {
                        // Encryp ticket into a new cookie
                        string cookieValue = FormsAuthentication.Encrypt(ticket);

                        if (cookie != null)
                        {
                            cookie = request.Cookies[FormsAuthentication.FormsCookieName];
                        }

                        if (cookie == null)
                        {
                            cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
                            {
                                Path = ticket.CookiePath
                            };
                        }

                        // If the ticket is persistent (survives browser sessions) an expiration
                        // date is set. Otherwise the browser will not save the cookie across
                        // sessions
                        if (ticket.IsPersistent)
                        {
                            cookie.Expires = ticket.Expiration;
                        }

                        // Set additional cookie options
                        cookie.Value = cookieValue;
                        cookie.Secure = FormsAuthentication.RequireSSL;
                        cookie.HttpOnly = true;

                        if (FormsAuthentication.CookieDomain != null)
                        {
                            cookie.Domain = FormsAuthentication.CookieDomain;
                        }

                        // Remove old cookie and set new one
                        // TODO: this doesn't work with web services, so it is removed temporarily
                        /*
                        httpContext.Response.Cookies.Remove(cookie.Name);
                        httpContext.Response.Cookies.Add(cookie);
                         * */
                    }
#endif
                }
            }

            return response;
        }

        /// <summary>
        /// Creates a Graywulf principal based on the user name stored in the
        /// forms authentication ticket.
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        /// <remarks>
        /// A FormsAuthenticationTicket is always automatically accepted as from
        /// a master authority.
        /// </remarks>
        private GraywulfPrincipal CreatePrincipal(FormsAuthenticationTicket ticket)
        {
            var principal = base.CreatePrincipal();
            var identity = principal.Identity;

            identity.Identifier = ticket.Name;
            identity.IsAuthenticated = true;
            identity.IsMasterAuthority = true;

            identity.UserReference.Name = ticket.Name;
            
            return principal;
        }
    }
}
