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

        public FunctionIdentifier()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.functionReference = null;
        }

        public override ParserLib.Node Interpret()
        {
            this.functionReference = new FunctionReference(this);

            return base.Interpret();
        }

        public override bool AcceptCodeGenerator(ParserLib.CodeGenerator cg)
        {
            return ((SqlCodeGen.SqlCodeGeneratorBase)cg).WriteFunctionIdentifier(this);
        }
    }
}
