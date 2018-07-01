using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MethodName
    {
        public static MethodName Create(string methodName)
        {
            var id = Identifier.Create(methodName);
            var mn = new MethodName();
            mn.Stack.AddLast(id);
            return mn;
        }
    }
}
