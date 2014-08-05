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
        private NameValueCollection headers;
        private HttpCookieCollection cookies;
        private GraywulfPrincipal principal;

        #endregion
        #region Properties

        public bool Success
        {
            get { return success; }
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
            foreach (string header in headers.Keys)
            {
                response.Headers.Add(header, headers[header]);
            }

            foreach (HttpCookie cookie in cookies)
            {
                response.Cookies.Add(cookie);
            }
        }

        public void SetResponseHeaders(System.ServiceModel.Web.OutgoingWebResponseContext response)
        {
            foreach (string header in headers.Keys)
            {
                response.Headers.Add(header, headers[header]);
            }

            foreach (HttpCookie cookie in cookies)
            {
                response.Headers.Add(Web.Security.Constants.HttpHeaderSetCookie, AuthenticationResponse.GetSetCookieHeader(cookie));
            }
        }

        public static string GetSetCookieHeader(HttpCookie cookie)
        {
            StringBuilder s = new StringBuilder();

            // cookiename=
            if (!String.IsNullOrEmpty(cookie.Name))
            {
                s.Append(cookie.Name);
                s.Append('=');
            }

            // key=value&...
            if (cookie.Values != null)
            {
                s.Append(EncodeCookieMultiValue(cookie.Values, false, null));
            }
            else if (cookie.Value != null)
            {
                s.Append(cookie.Value);
            }

            // domain
            if (!String.IsNullOrEmpty(cookie.Domain))
            {
                s.Append("; domain=");
                s.Append(cookie.Domain);
            }

            // expiration
            if (cookie.Expires != DateTime.MinValue)
            {
                s.Append("; expires=");
                s.Append(FormatHttpCookieDateTime(cookie.Expires));
            }

            // path
            if (!String.IsNullOrEmpty(cookie.Path))
            {
                s.Append("; path=");
                s.Append(cookie.Path);
            }

            // secure
            if (cookie.Secure)
            {
                s.Append("; secure");
            }

            // httponly, Note: IE5 on the Mac doesn't support this
            if (cookie.HttpOnly /*&& SupportsHttpOnly(context)*/)
            {
                s.Append("; HttpOnly");
            }

            return s.ToString();
        }

        private static string EncodeCookieMultiValue(NameValueCollection multiValue, bool urlencoded, IDictionary excludeKeys)
        {
            int n = multiValue.Count;

            if (n == 0)
            {
                return String.Empty;
            }

            StringBuilder s = new StringBuilder();
            String key, keyPrefix, item;

            for (int i = 0; i < n; i++)
            {
                key = multiValue.Keys[i];

                if (excludeKeys != null && key != null && excludeKeys[key] != null)
                {
                    continue;
                }

                if (urlencoded)
                {
                    key = HttpUtility.UrlEncodeUnicode(key);
                }

                keyPrefix = (key != null) ? (key + "=") : String.Empty;

                string[] values = multiValue.GetValues(i);

                if (s.Length > 0)
                {
                    s.Append('&');
                }

                if (values == null || values.Length == 0)
                {
                    s.Append(keyPrefix);
                }
                else if (values.Length == 1)
                {
                    s.Append(keyPrefix);
                    item = values[0];

                    if (urlencoded)
                    {
                        item = HttpUtility.UrlEncodeUnicode(item);
                    }

                    s.Append(item);
                }
                else
                {
                    for (int j = 0; j < values.Length; j++)
                    {
                        if (j > 0)
                        {
                            s.Append('&');
                        }

                        s.Append(keyPrefix);
                        item = values[j];

                        if (urlencoded)
                        {
                            item = HttpUtility.UrlEncodeUnicode(item);
                        }

                        s.Append(item);
                    }
                }
            }

            return s.ToString();
        }

        private static string FormatHttpCookieDateTime(DateTime dt)
        {
            if (dt < DateTime.MaxValue.AddDays(-1) &&
                dt > DateTime.MinValue.AddDays(1))
            {
                dt = dt.ToUniversalTime();
            }

            return dt.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
