using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Principal;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Conveys authentication results from the authenticators
    /// to the HTTP response. Provides a unified view of
    /// WCF web services and ASP.NET applications.
    /// </summary>
    public class AuthenticationResponse
    {
        #region Private member variables

        private bool success;
        private AuthenticationRequest request;
        private NameValueCollection queryParameters;
        private NameValueCollection headers;
        private HttpCookieCollection cookies;
        private GraywulfPrincipal principal;

        #endregion
        #region Properties

        public bool Success
        {
            get { return success; }
        }

        public NameValueCollection QueryParameters
        {
            get { return queryParameters; }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public HttpCookieCollection Cookies
        {
            get { return cookies; }
        }

        public GraywulfPrincipal Principal
        {
            get { return principal; }
        }

        #endregion
        #region Constructors and initializers

        public AuthenticationResponse(AuthenticationRequest request)
        {
            InitializeMembers();

            this.request = request;
        }

        private void InitializeMembers()
        {
            this.success = false;
            this.request = null;
            this.queryParameters = new NameValueCollection();
            this.headers = new NameValueCollection();
            this.cookies = new HttpCookieCollection();
            this.principal = null;
        }

        #endregion

        public void SetPrincipal(GraywulfPrincipal principal)
        {
            this.success = (principal != null);
            this.principal = principal;
        }

        public void AddFormsAuthenticationTicket(bool createPersistentCookie)
        {
            var cookie = System.Web.Security.FormsAuthentication.GetAuthCookie(
                this.principal.Identity.User.GetFullyQualifiedName(),
                createPersistentCookie,
                System.Web.Security.FormsAuthentication.FormsCookiePath);

            cookies.Add(cookie);
        }

        public void SetResponseHeaders(HttpResponse response)
        {
            foreach (string key in headers.Keys)
            {
                response.Headers.Add(key, headers[key]);
            }

            foreach (string key in cookies)
            {
                response.Cookies.Add(cookies[key]);
            }
        }

        public void DeleteResponseHeaders(HttpResponse response)
        {
            foreach (string key in headers.Keys)
            {
                response.Headers.Remove(key);
            }

            foreach (string key in cookies)
            {
                response.Cookies.Remove(key);

                // Reset cookie and add back to collection
                var cookie = new HttpCookie(key, "");
                cookie.Expires = DateTime.Now.AddDays(-1);
                response.Cookies.Add(cookie);
            }
        }

        public void SetResponseHeaders(System.ServiceModel.Web.OutgoingWebResponseContext response)
        {
            foreach (string key in headers.Keys)
            {
                response.Headers.Add(key, headers[key]);
            }

            // Create 'Set-Cookie' headers
            foreach (string key in cookies.Keys)
            {
                response.Headers.Add(
                    Services.Constants.HttpHeaderSetCookie,
                    Util.CookieConverter.ToSetCookieHeader(cookies[key]));
            }
        }

        /// <summary>
        /// Returns the URL where the user is
        /// </summary>
        /// <returns></returns>
        public string GetReturnUrl()
        {
            // We will redirect the user to an url passed as a parameter
            var url = new StringBuilder(HttpUtility.UrlDecode(request.QueryString["ReturnUrl"]));

            // Take all parameters from the return url and substitute parameter values
            foreach (string key in queryParameters.Keys)
            {
                url.Replace("$" + key, HttpUtility.UrlEncode(queryParameters[key]));
            }

            return url.ToString();
        }
    }
}
