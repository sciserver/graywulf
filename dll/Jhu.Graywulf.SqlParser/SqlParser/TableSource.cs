using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableSource : ITableReference
    {
        private ITableSource specificTableSource;

        public ITableSource SpecificTableSource
        {
            get { return specificTableSource; }
            protected set { specificTableSource = value; }
        }

        public TableReference TableReference
        {
            get { return specificTableSource.TableReference; }
            set { specificTableSource.TableReference = value; }
        }

        public QuerySpecification QuerySpecification
        {
            get { return FindAscendant<QuerySpecification>(); }
        }

        public override Node Interpret()
        {
            var ts = FindDescendant<SimpleTableSource>();
            if (ts != null)
            {
                this.specificTableSource = ts;
            }

            var fts = FindDescendant<FunctionTableSource>();
            if (fts != null)
            {
                this.specificTableSource = fts;
            }

            var vts = FindDescendant<VariableTableSource>();
            if (vts != null)
            {
                this.specificTableSource = vts;
            }

            var sts = FindDescendant<SubqueryTableSource>();
            if (sts != null)
            {
                this.specificTableSource = sts;
            }

            if (this.specificTableSource == null)
            {
                throw new NotImplementedException();
            }

            return base.Interpret();
        }

        public static TableSource Create(ComputedTableSource ts)
        {
            var res = new TableSource();
            res.Stack.AddLast(ts);
            return res;
        }
    }
}
