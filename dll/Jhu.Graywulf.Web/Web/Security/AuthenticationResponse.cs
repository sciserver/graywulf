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
        private Uri uri;
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

        public AuthenticationResponse()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.success = false;
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
    }
}
