using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Jhu.Graywulf.Web.Security
{
    public class AuthenticationRequest
    {
        private Uri uri;
        private NameValueCollection queryString;
        private NameValueCollection headers;
        private HttpCookieCollection cookies;

        public NameValueCollection QueryString
        {
            get { return queryString; }
        }

        public NameValueCollection Headers
        {
            get { return headers; }
        }

        public HttpCookieCollection Cookies
        {
            get { return cookies; }
        }

        #region Constructors and initializers

        public AuthenticationRequest(System.Web.HttpRequest request)
        {
            InitializeMembers();

            this.uri = request.Url;
            this.queryString = request.QueryString;
            this.headers = request.Headers;
            this.cookies = request.Cookies;
        }

        public AuthenticationRequest(System.ServiceModel.Web.IncomingWebRequestContext request)
        {
            InitializeMembers();

            this.uri = System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To;
            this.queryString = HttpUtility.ParseQueryString(uri.Query);
            this.headers = request.Headers;

            // Web services don't parse cookies, so we do it now
            ParseCookies();
        }

        private void InitializeMembers()
        {
            this.queryString = null;
            this.headers = null;
            this.cookies = null;
        }

        #endregion

        private void ParseCookies()
        {
            this.cookies = new HttpCookieCollection();

            var cookieHeader = headers[Constants.HttpHeaderCookie];

            int l = (cookieHeader != null) ? cookieHeader.Length : 0;
            int i = 0;
            int j;
            char ch;

            HttpCookie lastCookie = null;

            while (i < l)
            {
                // Find next ';' (don't look to ',' as per 91884)
                j = i;

                while (j < l)
                {
                    ch = cookieHeader[j];
                    if (ch == ';')
                    {
                        break;
                    }

                    j++;
                }

                // Create cookie form string
                String cookieString = cookieHeader.Substring(i, j - i).Trim();

                // Next cookie start
                i = j + 1; 

                if (cookieString.Length == 0)
                {
                    continue;
                }

                HttpCookie cookie = CreateCookieFromString(cookieString);

                // some cookies starting with '$' are really attributes of the last cookie
                if (lastCookie != null)
                {
                    String name = cookie.Name;

                    // Add known attribute to the last cookie (if any)
                    if (name != null && name.Length > 0 && name[0] == '$')
                    {
                        if (StringComparer.InvariantCultureIgnoreCase.Compare(name, "$Path") == 0)
                        {
                            lastCookie.Path = cookie.Value;
                        }
                        else if (StringComparer.InvariantCultureIgnoreCase.Compare(name, "$Domain") == 0)
                        {
                            lastCookie.Domain = cookie.Value;
                        }

                        continue;
                    }
                }

                // Regular cookie
                cookies.Add(cookie);
                lastCookie = cookie;

                // Goto next cookie
            }
        }

        private static HttpCookie CreateCookieFromString(String s)
        {
            int l = (s != null) ? s.Length : 0;
            int i = 0;
            int ai, ei;
            bool firstValue = true;
            int numValues = 1;

            HttpCookie c = null;
            string name;

            // Format: cookiename[=key1=val2&key2=val2&...]

            while (i < l)
            {
                //  Find next &
                ai = s.IndexOf('&', i);
                
                if (ai < 0)
                {
                    ai = l;
                }

                // First value might contain cookie name before =
                if (firstValue)
                {
                    ei = s.IndexOf('=', i);

                    if (ei >= 0 && ei < ai)
                    {
                        name = s.Substring(i, ei - i);
                        i = ei + 1;
                    }
                    else if (ai == l)
                    {
                        // The whole cookie is just a name
                        name = s;
                        break;
                    }
                    else
                    {
                        name = String.Empty;
                    }

                    c = new HttpCookie(name);
                    firstValue = false;
                }

                // Find '='
                ei = s.IndexOf('=', i);

                if (ei < 0 && ai == l && numValues == 0)
                {
                    // Simple cookie with simple value
                    c.Value = s.Substring(i, l - i);
                }
                else if (ei >= 0 && ei < ai)
                {
                    // key=value
                    c.Values.Add(s.Substring(i, ei - i), s.Substring(ei + 1, ai - ei - 1));
                    numValues++;
                }
                else
                {
                    // Value without key
                    c.Values.Add(null, s.Substring(i, ai - i));
                    numValues++;
                }

                i = ai + 1;
            }

            return c;
        }
    }
}
