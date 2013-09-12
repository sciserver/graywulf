using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableValuedFunctionCall : ITableReference
    {
        private TableReference tableReference;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public UdfIdentifier UdfIdentifier
        {
            get
            {
                var fi = FindDescendant<FunctionIdentifier>();
                var udfi = fi.FindDescendant<UdfIdentifier>();

                return udfi;
            }
        }

        public bool IsUdf
        {
            get { return UdfIdentifier != null; }
        }

        public TableValuedFunctionCall()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.tableReference = null;
        }

        public override Node Interpret()
        {
            if (IsUdf)
            {
                this.tableReference = new TableReference(this);
            }

            return base.Interpret();
        }

        public override bool AcceptCodeGenerator(CodeGenerator cg)
        {
            return ((SqlCodeGen.SqlCodeGeneratorBase)cg).WriteTableValuedFunctionCall(this);
        }
    }
}
