using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    class Util
    {
        /// <summary>
        /// Return the AssemblyName from a fully qualified .net Type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetAssemblyNameFromTypeName(string typeName)
        {
            // Find the first non-escaped comma in the typename that separates the
            // assembly name from the type name

            int insidebrac = 0;
            int foundat = -1;

            int i = 0;
            while (foundat == -1 && i < typeName.Length)
            {
                // skip escaped characters
                switch (typeName[i])
                {
                    case '\\':
                        i++;
                        break;
                    case '[':
                        insidebrac++;
                        break;
                    case ']':
                        insidebrac--;
                        break;
                    case ',':
                        if (insidebrac == 0)
                        {
                            foundat = i;
                        }
                        break;
                }

                i++;
            }

            if (foundat != -1)
            {
                return typeName.Substring(foundat + 1).Trim();
            }
            else
            {
                throw new TypeLoadException(String.Format(ExceptionMessages.ErrorParsingTypeName, typeName));
            }
        }
    }
}
