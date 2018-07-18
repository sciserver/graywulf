using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionCall : IFunctionReference
    {
        public FunctionIdentifier FunctionIdentifier
        {
            get { return FindDescendant<FunctionIdentifier>(); }
        }

        public MethodName MethodName
        {
            get { return FindDescendant<MethodName>(); }
        }

        public FunctionArguments FunctionArguments
        {
            get { return FindDescendant<FunctionArguments>(); }
        }

        public FunctionReference FunctionReference
        {
            get { return FunctionIdentifier?.FunctionReference; }
            set { if (FunctionIdentifier != null) FunctionIdentifier.FunctionReference = value; }
        }
    }
}
