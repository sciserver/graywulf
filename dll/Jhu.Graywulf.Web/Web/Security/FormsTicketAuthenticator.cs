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
    public class FormsTicketAuthenticator : IRequestAuthenticator
    {
        public string Protocol
        {
            get { return "Forms Authentication"; }
        }

        public string AuthorityName
        {
            get { return "Graywulf"; }
        }

        public string AuthorityUri
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string DisplayName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public FormsTicketAuthenticator()
        {
            FormsAuthentication.Initialize();
        }

        public GraywulfPrincipal Authenticate()
        {
            GraywulfPrincipal user = null;

            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie != null)
            {
                var tOld = FormsAuthentication.Decrypt(cookie.Value);

                if ((tOld != null) && !tOld.Expired)
                {
                    var ticket = tOld;
                    if (FormsAuthentication.SlidingExpiration)
                    {
                        ticket = FormsAuthentication.RenewTicketIfOld(tOld);
                    }

                    user = GraywulfPrincipal.Create(ticket);

                    if (!ticket.CookiePath.Equals("/"))
                    {
                        cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                        if (cookie != null)
                        {
                            cookie.Path = ticket.CookiePath;
                        }
                    }

                    if (ticket != tOld)
                    {
                        string cookieValue = FormsAuthentication.Encrypt(ticket);

                        if (cookie != null)
                        {
                            cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                        }
                        if (cookie == null)
                        {
                            cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
                            {
                                Path = ticket.CookiePath
                            };
                        }
                        if (ticket.IsPersistent)
                        {
                            cookie.Expires = ticket.Expiration;
                        }
                        cookie.Value = cookieValue;
                        cookie.Secure = FormsAuthentication.RequireSSL;
                        cookie.HttpOnly = true;
                        if (FormsAuthentication.CookieDomain != null)
                        {
                            cookie.Domain = FormsAuthentication.CookieDomain;
                        }
                        HttpContext.Current.Response.Cookies.Remove(cookie.Name);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                }
            }

            return user;
        }
    }
}
