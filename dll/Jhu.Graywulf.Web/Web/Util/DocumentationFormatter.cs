using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Web.Util
{
    public static class DocumentationFormatter
    {
        private static Regex HtmlTagPairRegex = new Regex(@"<([a-z][a-z0-9]*)\b[^>]*>(.*?)</\1>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public static string FormatExample(string source)
        {
            // TODO: replace this with syntaxhighlighter and move to UI
            var mm = HtmlTagPairRegex.Matches(source);

            if (mm.Count > 0)
            {
                var s = new StringBuilder();
                int i = 0;

                foreach (Match m in mm)
                {
                    if (m.Index - i > 0)
                    {
                        s.Append(source.Substring(i, m.Index - i));
                    }

                    switch (m.Groups[1].Value.ToLower())
                    {
                        case "query":
                            s.Append("<pre>");
                            s.Append(FormatExample(m.Groups[2].Value));
                            s.Append("</pre>");
                            break;
                        default:
                            s.Append(m.Value);
                            break;
                    }

                    i = m.Index + m.Length;
                }

                if (i > 0 && i < source.Length)
                {
                    s.Append(source.Substring(i));
                }

                return s.ToString();
            }
            else
            {
                return source;
            }
        }
    }
}
