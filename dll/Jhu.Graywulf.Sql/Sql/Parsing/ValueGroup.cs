using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ValueGroup
    {
        public IEnumerable<Node> EnumerateValues()
        {
            foreach (var v in EnumerateDescendantsRecursive<ValueList>())
            {
                yield return (Node)v.Stack.First;
            }
        }
    }
}
