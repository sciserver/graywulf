using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SpecialFunctionCall
    {
        public FunctionName FunctionName
        {
            get { return ((Node)Stack.First).FindDescendant<FunctionName>(); }
        }

        public override void Interpret()
        {
            base.Interpret();

            // To avoid using reserved keywords for special function names, we use
            // a literal that has to be excanged with a FunctionName token

            var first = (Node)Stack.First;
            var name = first.FindDescendant<Literal>();
            var fname = FunctionName.Create(name.Value);
            first.Stack.Replace(name, fname);
        }
    }
}
