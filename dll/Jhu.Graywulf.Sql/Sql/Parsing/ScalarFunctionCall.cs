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

        public FunctionIdentifier UdfIdentifier
        {
            get { return FindDescendant<FunctionIdentifier>(); }
        }

        public FunctionReference FunctionReference
        {
            get { return UdfIdentifier.FunctionReference; }
            set { UdfIdentifier.FunctionReference = value; }
        }

        public IEnumerable<Argument> EnumerateArguments()
        {
            var args = FindDescendant<FunctionArguments>();
            var list = args?.FindDescendant<ArgumentList>();

            if (list != null)
            {
                foreach (var arg in list.EnumerateArguments())
                {
                    yield return arg;
                }
            }
            else
            {
                yield break;
            }
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

            /*
            var f = new ScalarFunctionCall();
            var fun = FunctionIdentifier.Create(functionReference);
            var args = FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            return f;
            */
        }

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
    }
}
