using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class TypeNameFormatter
    {
        public static string ToUnversionedAssemblyQualifiedName(Type type)
        {
            var typeName = type.FullName;
            var assemblyName = type.Assembly.FullName;

            // Remove version info from assembly name
            var i = assemblyName.IndexOf(',');
            if (i > 0)
            {
                assemblyName = assemblyName.Substring(0, i);
            }

            return String.Format("{0}, {1}", typeName, assemblyName);
        }
    }
}
