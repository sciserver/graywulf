using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Jhu.Graywulf.Components
{
    class AssemblyNameComparer : IComparer<AssemblyName>
    {
        public static AssemblyNameComparer Instance
        {
            get { return new AssemblyNameComparer(); }
        }

        public int Compare(AssemblyName x, AssemblyName y)
        {
            var c = StringComparer.InvariantCultureIgnoreCase.Compare(x.Name, y.Name);
            if (c != 0)
            {
                return c;
            }

            if (x.Version == null || y.Version == null)
            {
                return 0;
            }
            else
            {
                return x.Version.CompareTo(y.Version);
            }
        }
    }
}
