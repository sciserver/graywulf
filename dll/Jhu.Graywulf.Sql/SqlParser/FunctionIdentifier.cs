using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class FunctionIdentifier : IFunctionReference
    {
        private FunctionReference functionReference;

        public FunctionReference FunctionReference
        {
            get { return functionReference; }
            set { functionReference = value; }
        }

        public UdfIdentifier UdfIdentifier
        {
            get { return FindDescendant<UdfIdentifier>(); }
        }

        public FunctionName FunctionName
        {
            get { return FindDescendant<FunctionName>(); }
        }

        protected override void InitializeMembers()
        {
            base.InitializeMembers();

            this.functionReference = null;
        }

        protected override void CopyMembers(object other)
        {
            base.CopyMembers(other);

            var old = (FunctionIdentifier)other;

            this.functionReference = old.functionReference;
        }

        public override ParserLib.Node Interpret()
        {
            this.functionReference = new FunctionReference(this);

            return base.Interpret();
        }
    }
}
