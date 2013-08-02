using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.CommandLineParser
{
    internal class PartialStringComparer : StringComparer
    {
        private StringComparer internalComparer;

        public PartialStringComparer(StringComparer internalComparer)
        {
            this.internalComparer = internalComparer;
        }

        public override int Compare(string x, string y)
        {
            if (x.Length == y.Length)
            {
                return internalComparer.Compare(x, y);
            }
            else if (x.Length < y.Length)
            {
                string yy = y.Substring(0, x.Length);
                return internalComparer.Compare(x, yy);
            }
            else
            {
                string xx = x.Substring(0, y.Length);
                return internalComparer.Compare(xx, y);
            }
        }

        public override bool Equals(string x, string y)
        {
            return Compare(x, y) == 0;
        }

        public override int GetHashCode(string obj)
        {
            // The only possible hash is the value of the first character
            if (obj.Length > 0)
            {
                return 1 + (int)char.ToLowerInvariant(obj[0]);
            }
            else
            {
                return 0;
            }
        }
    }
}
