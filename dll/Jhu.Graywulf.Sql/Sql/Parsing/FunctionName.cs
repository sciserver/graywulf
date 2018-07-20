using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionName : IFunctionReference
    {
        private FunctionReference functionReference;

        public FunctionReference FunctionReference
        {
            get { return functionReference; }
            set { functionReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
            this.functionReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (FunctionName)other;
            this.functionReference = old.functionReference;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.functionReference = FunctionReference.Interpret(this);
        }

        public static FunctionName Create(string functionName)
        {
            var nfn = new FunctionName();
            nfn.Stack.AddLast(Identifier.Create(functionName));

            return nfn;
        }

        public static FunctionName Create(FunctionReference functionReference)
        {
            var nfn = Create(functionReference.FunctionName);
            nfn.functionReference = functionReference;
            return nfn;
        }
    }
}
