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
        public static Uri FromFilePath(string path)
        {
            path = path.Replace('\\', '/');

            if (path.StartsWith(@"//"))
            {
                // This is a UNC path, replace \ with / and remove one of the starting /-s
                // The format is file://server/test.txt
                path = path.Substring(2);
                return new Uri(String.Format("{0}{1}{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, path));
            }
            else if (path.StartsWith("/"))
            {
                // This is an absolute path without drive letter
                // The format is file:///test.txt
                return new Uri(String.Format("{0}{1}{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, path));
            }
            else if (Path.IsPathRooted(path))
            {
                // This is a local path starting with a volume letter
                // The format is file:///C:/test.txt
                return new Uri(String.Format("{0}{1}/{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, path));
            }
            else
            {
                // This is a relative URI, just replace \ with /
                return new Uri(path, UriKind.Relative);
            }
        }

        /// <summary>
        /// Converts a relative or a file:// uri to a local or UNC path.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string ToFilePath(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                return uri.ToString().Replace("/", "\\");
            }
            else if (!uri.IsFile)
            {
                throw new ArgumentException("Uri is not a local or UNC file.", "uri");
            }
            else
            {
                return uri.LocalPath.Replace("/", "\\");
            }
        }

        /// <summary>
        /// Returns the path from any URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetPath(Uri uri)
        {
            string path;

            if (!uri.IsAbsoluteUri)
            {
                path = uri.ToString();

                int idx1 = path.IndexOf('?');
                int idx2 = path.IndexOf('#');

                if (idx1 > 0)
                {
                    path = path.Substring(0, idx1);
                }
                else if (idx2 > 0)
                {
                    path = path.Substring(0, idx2);
                }
                else if (idx1 == 0 || idx2 == 0)
                {
                    path = String.Empty;
                }
            }
            else
            {
                path = uri.LocalPath;
            }

            return path;
        }

        /// <summary>
        /// Gets the file name part from any URI, ie, the very last part of the
        /// path (can be empty string if it is a directory) but before the ? or #
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetFilename(Uri uri)
        {
            var path = GetPath(uri);
            return Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(Uri uri)
        {
            var path = GetPath(uri);
            return Path.GetFileNameWithoutExtension(path);
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

        /// <summary>
        /// Combines two URIs into a single one assuming the first part ends
        /// in a path without query and fragments.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
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
