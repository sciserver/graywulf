﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public FunctionArguments FunctionArguments
        {
            get { return FindDescendant<FunctionArguments>(); }
        }

        public static ScalarFunctionCall Create(FunctionReference functionReference, params Expression[] arguments)
        {
            var f = new ScalarFunctionCall();
            var fun = FunctionIdentifier.Create(functionReference);
            var args = Parsing.FunctionArguments.Create(arguments);

            f.Stack.AddLast(fun);
            f.Stack.AddLast(args);

            return f;
        }
    }
}
