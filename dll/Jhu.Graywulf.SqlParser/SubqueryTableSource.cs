using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.SqlParser
{
    public partial class SubqueryTableSource : ITableSource
    {
        public SelectStatement SelectStatement
        {
            get
            {
                return FindDescendant<Subquery>().SelectStatement;
            }
        }

        public QueryExpression QueryExpression
        {
            get
            {
                return SelectStatement.QueryExpression;
            }
        }

        public TableReference TableReference
        {
            get { return QueryExpression.TableReference; }
            set { QueryExpression.TableReference = value; }
        }

        public override Node Interpret()
        {
            TableReference = new TableReference(this);

            return base.Interpret();
        }
    }
}
