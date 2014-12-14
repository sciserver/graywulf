using System;
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
    /// Conveys authentication information from the HTTP requests
    /// to the authenticator classes. Provides a unified view of
    /// WCF web services and ASP.NET applications.
    /// </summary>
    public class AuthenticationRequest
    {
        #region Private member variables

        private Uri uri;
        private string username;
        private string password;
        private NameValueCollection queryString;
        private NameValueCollection headers;
        private CookieContainer cookies;

        /// <summary>
        /// Holds the principal established so far during the processing of
        /// the request.
        /// </summary>
        private IPrincipal principal;

        #endregion
        #region Properties

        public Uri Uri
        {
            get { return uri; }
        }

        /// <summary>
        /// Gets or sets the username associated with the request.
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Gets or sets the password associated with the request.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public NameValueCollection QueryString
        {
            get { return queryString; }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public CookieContainer Cookies
        {
            get { return cookies; }
        }

        public IPrincipal Principal
        {
            get { return principal; }
        }

        #endregion
        #region Constructors and initializers

        public AuthenticationRequest(string username, string password)
        {
            InitializeMembers();

            this.username = username;
            this.password = password;
        }

        public AuthenticationRequest(System.Web.HttpContext context)
        {
            InitializeMembers();

            var request = context.Request;

            this.uri = request.Url;
            this.queryString.Add(request.QueryString);
            this.headers.Add(request.Headers);

            for (int i = 0; i < request.Cookies.Count; i ++)
            {
                // TODO: this might not handle multi-values cookies, test

                var cookie = Util.CookieConverter.ToCookie(request.Cookies[i]);
                this.cookies.Add(uri, cookie);
            }
            
            this.principal = context.User;
        }

        public AuthenticationRequest(System.ServiceModel.Web.IncomingWebRequestContext context)
        {
            InitializeMembers();

            this.uri = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To;
            this.queryString = HttpUtility.ParseQueryString(uri.Query);
            this.headers = context.Headers;

            // Web services don't parse cookies, so we do it now
            var cookies = headers[Services.Constants.HttpHeaderCookie];
            if (cookies != null)
            {
                this.cookies.SetCookies(uri, Util.CookieConverter.ToCommaSeparatedCookieList(cookies));
            }
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.username = null;
            this.password = null;
            this.queryString = new NameValueCollection();
            this.headers = new NameValueCollection();
            this.cookies = new CookieContainer();
            this.principal = null;
        }

        #endregion

    }
}
