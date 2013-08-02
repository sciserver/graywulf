using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableOrViewName : ITableReference
    {
        private TableReference tableReference;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public TableOrViewName()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.tableReference = null;
        }

        public static TableOrViewName Create(TableReference tr)
        {
            var res = new TableOrViewName();
            res.tableReference = tr;
            return res;
        }

        public override Node Interpret()
        {
            this.tableReference = new TableReference(this);

            return base.Interpret();
        }

        public override bool AcceptCodeGenerator(CodeGenerator cg)
        {
            return ((SqlCodeGen.SqlCodeGeneratorBase)cg).WriteTableOrViewName(this);
        }
    }
}
