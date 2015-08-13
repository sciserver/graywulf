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

        public bool IsSubquery
        {
            get { return false; }
        }

        public bool IsMultiTable
        {
            get { return false; }
        }

        protected override void InitializeMembers()
        {
            base.InitializeMembers();

            this.tableReference = null;
        }

        protected override void CopyMembers(object other)
        {
            base.CopyMembers(other);

            var old = (VariableTableSource)other;

            this.tableReference = old.tableReference;
        }

        public override Node Interpret()
        {
            this.tableReference = new TableReference(this);

            return base.Interpret();
        }

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
