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
            get { return FindDescendant<FunctionName>(); }
        }

        public override void Interpret()
        {
            base.Interpret();
            var oldname = (Literal)Stack.First;
            var newname = FunctionName.Create(oldname.Value);
            newname.Interpret();
            oldname.ReplaceWith(newname);
        }
    }
}
