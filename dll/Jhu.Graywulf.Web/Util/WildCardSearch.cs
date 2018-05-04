using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Util
{
    public static class WildCardSearch
    {
        // From: https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard

        public static Regex GetRegex(string pattern)
        {
            return new Regex(
                "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
