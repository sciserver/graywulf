using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Util
{
    public static class DeepCloner
    {
        public static T CloneObject<T>(T source)
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

        /// <summary>
        /// Creates a deep copy of an array of primitive types or structs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] CopyArray<T>(T[] source)
            where T : struct
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                var res = new T[source.Length];
                Array.Copy(source, res, source.Length);
                return res;
            }
        }


        /// <summary>
        /// Creates a deep copy of an array of IClonable objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] CloneArray<T>(T[] source)
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

        public static IEnumerable<T> CloneCollection<T>(IEnumerable<T> source)
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
