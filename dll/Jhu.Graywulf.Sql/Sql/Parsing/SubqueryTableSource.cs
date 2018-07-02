using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SubqueryTableSource : ITableSource
    {
        private string uniqueKey;

        public string UniqueKey
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

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return QueryExpression.ResultsTableReference; }
        }

        public TableReference TableReference
        {
            get { return QueryExpression.ResultsTableReference; }
            set { QueryExpression.ResultsTableReference = value; }
        }

        public bool IsSubquery
        {
            get { return true; }
        }

        public bool IsMultiTable
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

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            foreach (var tts in Subquery.EnumerateSourceTables(recursive))
            {
                yield return tts;
            }
        }

        public IEnumerable<ITableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
