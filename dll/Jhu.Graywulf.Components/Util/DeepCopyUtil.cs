using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class DeepCopy
    {
        public static T CopyObject<T>(T source)
            where T : ICloneable
        {
            if (source == null)
            {
                return default(T);
            }
            else
            {
                return (T)source.Clone();
            }
        }

        public static T[] CopyArray<T>(T[] source)
            where T : ICloneable
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                var res = new T[source.Length];
                for (int i = 0; i < source.Length; i++)
                {
                    res[i] = (T)source[i].Clone();
                }

                return res;
            }
        }

        public static IEnumerable<T> CopyCollection<T>(IEnumerable<T> source)
            where T : ICloneable
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                return source.Select(i => (T)i.Clone());
            }
        }
    }
}
