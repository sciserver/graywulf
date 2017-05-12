using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    /// <summary>
    /// Summary description for Includer.
    /// </summary>
    public class HtmlIncluder : Control
    {
        private static readonly Regex BodyRegex = new Regex(
            @"<body[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex EndBodyRegex = new Regex(
            @"</body>", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex UrlRegex = new Regex(
            @"(src|href)\s*=\s*""([^""]*)""", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private string path;

        public string Src
        {
            get { return (string)ViewState["Src"] ?? String.Empty; }
            set { ViewState["Src"] = value; }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Src.StartsWith("~"))
            {
                path = MapPathSecure(Src);
            }
            else
            {
                path = Src;
            }

            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);

                // Get body
                var m1 = BodyRegex.Match(content);

                if (!m1.Success)
                {
                    throw new Exception("Body tag not found"); // *** TODO
                }

                var m2 = EndBodyRegex.Match(content);

                if (!m2.Success)
                {
                    throw new Exception("Body end tag not found");  // **** TODO
                }

                int start = m1.Index + m1.Length;
                int end = m2.Index;

                content = content.Substring(start, end - start);
                content = UrlRegex.Replace(content, ReplaceUrl);

                writer.Write(content);
            }
        }

        private string ReplaceUrl(Match match)
        {
            var attribute = match.Groups[1].Value;
            var url = match.Groups[2].Value;

            if (Src.StartsWith("~"))
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                if (!uri.IsAbsoluteUri)
                {
                    url = VirtualPathUtility.Combine(Src, url);
                }
            }

            if (url.StartsWith("~"))
            {
                url = ResolveUrl(url);
            }

            return attribute + "=\"" + url + "\"";
        }
    }
}