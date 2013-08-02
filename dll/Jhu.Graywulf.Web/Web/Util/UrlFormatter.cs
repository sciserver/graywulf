using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Jhu.Graywulf.Web.Util
{
    public static class UrlFormatter
    {
        public static string GetClientRedirect(string url)
        {
            return String.Format("javascript:location='{0}';", MakeRelativePath(url));
        }

        public static string GetClientPopUp(string url)
        {
            return String.Format("javascript:window.open('{0}');", MakeRelativePath(url));
        }

        private static string MakeRelativePath(string url)
        {
            if (HttpContext.Current != null && VirtualPathUtility.IsAppRelative(url))
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
