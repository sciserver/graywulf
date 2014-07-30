using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class MediaTypeComparer
    {
        public static bool Compare(string a, string b)
        {
            // TODO: add logic to handle wild-cards
            return StringComparer.InvariantCultureIgnoreCase.Compare(a, b) == 0;
        }
    }
}
