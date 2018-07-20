using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class MethodCall : IMethodReference
    {
        public abstract MethodReference MethodReference { get; set; }

        public FunctionArguments FunctionArguments
        {
            get { return FindDescendant<FunctionArguments>(); }
        }
    }
}
