using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class TableSource : ITableReference
    {

        public ITableSource SpecificTableSource
        {
            get { return FindSpecificTableSource(); }
        }

        public TableReference TableReference
        {
            get { return SpecificTableSource.TableReference; }
            set { SpecificTableSource.TableReference = value; }
        }

        public QuerySpecification QuerySpecification
        {
            get { return FindAscendant<QuerySpecification>(); }
        }

        public static TableSource Create(ComputedTableSource ts)
        {
            var res = new TableSource();
            res.Stack.AddLast(ts);
            return res;
        }

        public static TableSource Create(TableReference tr)
        {
            var ts = new TableSource();
            var sts = SimpleTableSource.Create(tr);
            ts.Stack.AddLast(sts);
            return ts;
        }

        public override Node Interpret()
        {
            if (FindSpecificTableSource() == null)
            {
                throw new NotImplementedException();
            }

            return base.Interpret();
        }

        protected virtual ITableSource FindSpecificTableSource()
        {
            var ts = FindDescendant<SimpleTableSource>();
            if (ts != null)
            {
                return ts;
            }

            var fts = FindDescendant<FunctionTableSource>();
            if (fts != null)
            {
                return fts;
            }

            var vts = FindDescendant<VariableTableSource>();
            if (vts != null)
            {
                return vts;
            }

            var sts = FindDescendant<SubqueryTableSource>();
            if (sts != null)
            {
                return sts;
            }

            return null;
        }
    }
}
