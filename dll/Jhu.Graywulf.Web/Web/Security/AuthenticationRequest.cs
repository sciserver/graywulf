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

        public AuthenticationRequest(System.Web.HttpContext context)
        {
            InitializeMembers();

            var request = context.Request;

            this.uri = request.Url;
            this.queryString.Add(request.QueryString);
            this.headers.Add(request.Headers);

            foreach (HttpCookie httpCookie in request.Cookies)
            {
                // TODO: this might not handle multi-values cookies, test

                var cookie = Util.CookieConverter.ToCookie(httpCookie);
                this.cookies.Add(cookie);
            }
            
            this.principal = context.User;
        }

        public AuthenticationRequest(System.ServiceModel.Web.IncomingWebRequestContext request)
        {
            InitializeMembers();

            this.uri = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To;
            this.queryString = HttpUtility.ParseQueryString(uri.Query);
            this.headers = request.Headers;

            // Web services don't parse cookies, so we do it now
            this.cookies.SetCookies(uri, headers[Web.Constants.HttpHeaderCookie]);
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.queryString = new NameValueCollection();
            this.headers = new NameValueCollection();
            this.cookies = new CookieContainer();
            this.principal = null;
        }

        #endregion

    }
}
