using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableValuedFunctionCall : IFunctionReference
    {
        public FunctionReference FunctionReference
        {
            get { return UdfIdentifier.FunctionReference; }
            set { UdfIdentifier.FunctionReference = value; }
        }

        public UdfIdentifier UdfIdentifier
        {
            get { return FindDescendant<UdfIdentifier>(); }
        }

        public static TableValuedFunctionCall Create(FunctionReference functionReference, params Expression[] arguments)
        {
            throw new NotImplementedException();

            /*
            var f = new TableValuedFunctionCall();
            var fun = FunctionIdentifier.Create(functionReference);
            var args = FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            f.functionReference = TableReference.Interpret(f);

            return f;
            */
        }
    }
}
