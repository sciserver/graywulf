using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jhu.Graywulf.Util
{
    public static class UrlFormatter
    {
        public static string ToBaseUrl(String url, string applicationPath)
        {
            return ToBaseUrl(new Uri(url), applicationPath);
        }

        public static string ToBaseUrl(Uri uri, string applicationPath)
        {
            return String.Format("{0}://{1}{2}/", uri.Scheme, uri.Authority, applicationPath.TrimEnd('/'));
        }

        public static string GetClientRedirect(string url)
        {
            return String.Format("javascript:location='{0}';", ToRelativeUrl(url));
        }

        public static string GetClientPopUp(string url)
        {
            return String.Format("javascript:window.open('{0}');", ToRelativeUrl(url));
        }

        public static string ToRelativeUrl(string url)
        {
            if (HttpContext.Current != null && !String.IsNullOrEmpty(url) && VirtualPathUtility.IsAppRelative(url))
            {
                url = VirtualPathUtility.MakeRelative(
                    HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath,
                    url);
            }

            return url;
        }

        public static string ArrayToUrlList(string[] array)
        {
            string res = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    res += ",";
                }

                res += array[i];
            }

            return res;
        }

        public static string ArrayToUrlList(Guid[] array)
        {
            string res = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    res += ",";
                }

                res += array[i];
            }

            return res;
        }
    }
}
