using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class Cloner
    {
        public static T Clone<T>(T obj)
            where T : ICloneable, new()
        {
            if (obj == null)
            {
                return default(T);
            }
            else
            {
                return (T)obj.Clone();
            }
        }
    }
}
