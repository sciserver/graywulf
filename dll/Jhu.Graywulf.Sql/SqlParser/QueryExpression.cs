using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class QueryExpression : ITableReference
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

            var old = (QueryExpression)other;

            this.tableReference = old.tableReference;
        }

        public override Node Interpret()
        {
            // Subqueries already initialize this
            if (this.tableReference == null)
            {
                this.tableReference = new TableReference(this);
            }

            return base.Interpret();
        }

        public virtual IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            foreach (var qs in EnumerateDescendants<QuerySpecification>())
            {
                foreach (var ts in qs.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }
        }
    }
}
