using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Util
{
    public static class UriConverter
    {
        public static Uri FromFilePath(string path)
        {
            if (path.StartsWith(@"\\") || path.StartsWith(@"//"))
            {
                return new Uri(String.Format("file:{0}", path.Replace('\\', '/')));
            }
            if (Path.IsPathRooted(path))
            {
                return new Uri(String.Format("file:///{0}", path.Replace('\\', '/')));
            }
            else
            {
                return new Uri(path.Replace('\\', '/'), UriKind.Relative);
            }
        }

        public static string ToFilePath(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return uri.ToString();
            }
            else if (!uri.IsFile)
            {
                throw new ArgumentException("Uri is not a local or UNC file.", "uri");
            }
            else if (uri.IsUnc)
            {
                return String.Format("//{0}/{1}", uri.Host, uri.GetComponents(UriComponents.Path, UriFormat.Unescaped));
            }
            else
            {
                return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            }
        }

        public static string ToFileName(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return uri.ToString();
            }
            else
            {
                return uri.AbsolutePath;
            }
        }

        public static Uri Combine(Uri baseUri, Uri relativeUri)
        {
            if (relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }
            else if (baseUri.IsAbsoluteUri)
            {
                return new Uri(baseUri, relativeUri);
            }
            else
            {
                var basestr = baseUri.ToString().TrimEnd('/', '\\');
                var relstr = relativeUri.ToString().TrimStart('/', '\\');
                return new Uri(basestr + "/" + relstr, UriKind.Relative);
            }
        }
    }
}
