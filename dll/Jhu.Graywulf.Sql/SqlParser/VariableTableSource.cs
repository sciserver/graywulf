using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser
{
    public partial class VariableTableSource : ITableSource
    {
        private TableReference tableReference;

        public TableReference TableReference
        {
            get { return tableReference; }
            set { tableReference = value; }
        }

        public override Node Interpret()
        {
            this.tableReference = new TableReference(this);

            return base.Interpret();
        }
    }
}
