using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// Authenticates request based on a FormsAuthenticationTicket
    /// </summary>
    /// <remarks>
    /// This is used by WCF services
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
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);

                // TODO: validate ticket here

                return GraywulfPrincipal.Create(ticket);
            }
            else
            {
                return null;
            }
        }
    }
}
