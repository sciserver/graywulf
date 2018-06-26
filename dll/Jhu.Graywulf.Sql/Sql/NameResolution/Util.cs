using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.NameResolution
{
    static class Util
    {
        public static string RemoveIdentifierQuotes(string id)
        {
            if (id == null)
            {
                return null;
            }
            else if (id[0] == '[' && id[id.Length - 1] == ']')
            {
                return id.Substring(1, id.Length - 2);
            }
            else
            {
                return id;
            }
        }
    }
}
