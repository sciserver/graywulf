using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Util
{
    public static class UriConverter
    {
        /// <summary>
        /// Converts a path to a file:// uri
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Uri FromFilePath(string path)
        {
            path = path.Replace('\\', '/');

            if (path.StartsWith(@"//"))
            {
                // This is a UNC path, replace \ with / and remove one of the starting /-s
                path = path.Substring(2);
                return new Uri(String.Format("{0}{1}{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, path));
            }
            if (Path.IsPathRooted(path))
            {
                // This is a local patch starting with a volume letter
                return new Uri(String.Format("{0}{1}/{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, path));
            }
            else
            {
                // This is a relative URI, just replace \ with /
                return new Uri(path, UriKind.Relative);
            }
        }

        public static string ToFilePath(Uri uri)
        {
            // TODO: replace / with \ ?
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

        // TODO: merge this with ToFilePath... Or maybe just use localpath?
        public static string ToPath(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return uri.ToString();
            }
            else
            {
                return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            }
        }

        public static string ToFileName(Uri uri)
        {
            var path = ToPath(uri);
            return Path.GetFileName(path);
        }

        public static string ToFileNameWithoutExtension(Uri uri)
        {
            var path = ToPath(uri);
            return Path.GetFileNameWithoutExtension(path);
        }

        public static string ToExtension(Uri uri)
        {
            var path = ToPath(uri);
            return Path.GetExtension(path);
        }

        public static Uri Combine(Uri baseUri, string relativeUri)
        {
            var reluri = new Uri(relativeUri, UriKind.RelativeOrAbsolute);

            if (reluri.IsAbsoluteUri)
            {
                return reluri;
            }
            else
            {
                return Combine(baseUri, reluri);
            }
        }

        public static Uri Combine(Uri baseUri, Uri relativeUri)
        {
            if (baseUri == null || relativeUri.IsAbsoluteUri)
            {
                return relativeUri;
            }
            else if (relativeUri == null)
            {
                return baseUri;
            }
            else
            {
                var uristr = baseUri.ToString().TrimEnd('/', '\\') + "/" +
                    relativeUri.ToString().TrimStart('/', '\\');

                var uri = new Uri(uristr, UriKind.RelativeOrAbsolute);
                return uri;
            }
        }

        public static string ToCookieDomain(Uri uri)
        {
            return uri.Host;
        }
    }
}
