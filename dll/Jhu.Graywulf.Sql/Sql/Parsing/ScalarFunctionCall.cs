using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ScalarFunctionCall : IFunctionReference
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
        
        public static ScalarFunctionCall Create(FunctionReference functionReference, params Expression[] arguments)
        {
            var f = new ScalarFunctionCall();
            var fun = FunctionIdentifier.Create(functionReference);

            f.Stack.AddLast(fun);
            f.AppendArguments(arguments);

            return f;
        }

        public override bool Match(Parser parser)
        {
            throw new NotImplementedException();
        }
    }
}
