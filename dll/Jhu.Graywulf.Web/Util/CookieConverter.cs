using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Jhu.Graywulf.Util
{
    public static class CookieConverter
    {
        public static HttpCookie ToHttpCookie(Cookie cookie)
        {
            if (cookie == null)
            {
                return null;
            }
            else
            {
                return new HttpCookie(cookie.Name, cookie.Value)
                {
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Expires = cookie.Expires,
                    HttpOnly = cookie.HttpOnly,
                    Secure = cookie.Secure,
                };
            }
        }

        public static Cookie ToCookie(HttpCookie httpCookie)
        {
            var cookie = new Cookie()
            {
                Name = httpCookie.Name,
                Value = httpCookie.Value,
                Expires = httpCookie.Expires,
                HttpOnly = httpCookie.HttpOnly,
                Secure = httpCookie.Secure,
            };

            // Due to a minor bug, these needs to be set one by one:
            if (!String.IsNullOrEmpty(httpCookie.Domain))
            {
                cookie.Domain = httpCookie.Domain;
            }

            if (!String.IsNullOrEmpty(httpCookie.Path))
            {
                cookie.Path = httpCookie.Path;
            }

            return cookie;
        }

        public static string ToSetCookieHeader(HttpCookie httpCookie)
        {
            return ToSetCookieHeader(ToCookie(httpCookie));
        }

        public static string ToSetCookieHeader(Cookie cookie)
        {
            StringBuilder s = new StringBuilder();

            // cookiename=
            if (!String.IsNullOrEmpty(cookie.Name))
            {
                s.Append(cookie.Name);
                s.Append('=');
            }

            // key=value&...
            if (cookie.Value != null)
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
