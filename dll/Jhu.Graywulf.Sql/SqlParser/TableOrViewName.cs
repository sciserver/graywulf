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

        protected override void InitializeMembers()
        {
            base.InitializeMembers();

            this.tableReference = null;
        }

        protected override void CopyMembers(object other)
        {
            base.CopyMembers(other);

            var old = (TableOrViewName)other;

            this.tableReference = old.tableReference;
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
    }
}
