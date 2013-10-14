using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    static class Util
    {
        public static string RemoveIdentifierQuotes(string id)
        {
            if (id[0] == '[' && id[id.Length - 1] == ']')
            {
                return id.Substring(1, id.Length - 2);
            }
            else
            {
                return id;
            }
        }

        public static string EscapeIdentifierName(string name)
        {
            var res = name.Replace(".", "_");

            return res;
        }
    }
}
