using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ScalarFunctionCall : IFunctionReference
    {
        public static ScalarFunctionCall CreateVariableMethodCall(string variableName, string functionName)
        {
            throw new NotImplementedException();

            /*
            var udtf = new UdtFunctionCall();
            var fun = UdtFunctionIdentifier.Create(variableName, functionName);
            var args = FunctionArguments.Create(arguments);

            udtf.Stack.AddLast(fun);
            udtf.Stack.AddLast(args);

            return udtf;
            */
        }

        public FunctionIdentifier FunctionIdentifier
        {
            get { return FindDescendant<FunctionIdentifier>(); }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return FunctionIdentifier.FunctionReference; }
        }

        public FunctionReference FunctionReference
        {
            get { return FunctionIdentifier.FunctionReference; }
            set { FunctionIdentifier.FunctionReference = value; }
        }

        public static ScalarFunctionCall Create(string functionName, params Expression[] arguments)
        {
            throw new NotImplementedException();

            /*
            var f = new ScalarFunctionCall();
            var fun = FunctionIdentifier.Create(functionName);
            var args = FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            return f;
            */
        }

        public static ScalarFunctionCall Create(FunctionReference functionReference, params Expression[] arguments)
        {
            throw new NotImplementedException();

            var f = new ScalarFunctionCall();
            var fun = FunctionIdentifier.Create(functionReference);
            var args = FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            return f;
        }
    }
}
