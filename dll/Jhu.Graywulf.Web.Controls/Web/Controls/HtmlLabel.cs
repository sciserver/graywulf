using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class HtmlLabel : Literal
    {
        private static readonly Regex AppRelativeUrlRegex = new Regex(
            @"""(~/[^""]*)""", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        protected override void Render(HtmlTextWriter writer)
        {
            if (Text != null)
            {
                writer.Write(AppRelativeUrlRegex.Replace(Text, ReplaceUrl));
            }
        }

        private string ReplaceUrl(Match match)
        {
            return Page.ResolveClientUrl(match.Groups[0].Value);
        }
    }
}
