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
            specificTableSource = FindSpecificTableSource();

            if (specificTableSource == null)
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

        public static TableSource Create(ComputedTableSource ts)
        {
            var res = new TableSource();
            res.Stack.AddLast(ts);
            return res;
        }
    }
}
