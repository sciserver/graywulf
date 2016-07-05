using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class MediaTypeComparer
    {
        public static bool Compare(string pattern, string mime)
        {
            if (pattern == null || mime == null)
            {
                return false;
            }

            // TODO: add logic to handle wild-cards
            var partsa = pattern.Split('/');
            var partsm = mime.Split('/');

            if (partsa.Length < 2 || partsm.Length < 2)
            {
                return false;
            }

            return
                (StringComparer.InvariantCultureIgnoreCase.Compare(partsa[0], partsm[0]) == 0 ||
                 partsa[0] == "*" || partsm[0] == "*") &&
                 (StringComparer.InvariantCultureIgnoreCase.Compare(partsa[1], partsm[1]) == 0 ||
                 partsa[1] == "*" || partsm[1] == "*");
        }
    }
}
