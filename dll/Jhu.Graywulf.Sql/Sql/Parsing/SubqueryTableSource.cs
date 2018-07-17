using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SubqueryTableSource
    {
        private string uniqueKey;

        public override string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
        }

        public TableAlias Alias
        {
            get { return FindDescendant<TableAlias>(); }
        }

        public Subquery Subquery
        {
            get { return FindDescendant<Subquery>(); }
        }

        public QueryExpression QueryExpression
        {
            get { return Subquery.QueryExpression; }
        }
        
        public override TableReference TableReference
        {
            get { return QueryExpression.ResultsTableReference; }
            set { QueryExpression.ResultsTableReference = value; }
        }

        public override bool IsSubquery
        {
            get { return true; }
        }

        public override bool IsMultiTable
        {
            get { return false; }
        }

        public static SubqueryTableSource Create(SelectStatement ss, string tableAlias)
        {
            var sts = new SubqueryTableSource();

            sts.Stack.AddLast(Subquery.Create(ss));
            sts.Stack.AddLast(Whitespace.Create());
            sts.Stack.AddLast(Keyword.Create("AS"));
            sts.Stack.AddLast(Whitespace.Create());
            sts.Stack.AddLast(TableAlias.Create(tableAlias));

            return sts;
        }

        public override void Interpret()
        {
            base.Interpret();

            TableReference = TableReference.Interpret(this);
        }
    }
}
