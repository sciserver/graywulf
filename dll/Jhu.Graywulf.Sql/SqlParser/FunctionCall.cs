using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FunctionCall : IFunctionReference
    {
        public FunctionIdentifier FunctionIdentifier
        {
            get { return FindDescendant<FunctionIdentifier>(); }
        }

        public FunctionReference FunctionReference 
        {
            get { return FunctionIdentifier.FunctionReference; }
            set { FunctionIdentifier.FunctionReference = value; }
        }
    }
}
