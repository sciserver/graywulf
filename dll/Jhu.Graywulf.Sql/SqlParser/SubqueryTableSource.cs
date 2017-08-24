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
        public QueryExpression QueryExpression
        {
            get { return FindDescendant<Subquery>().QueryExpression; }
        }

        public TableReference TableReference
        {
            get { return QueryExpression.TableReference; }
            set { QueryExpression.TableReference = value; }
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

            TableReference = new TableReference(this);
        }

        public IEnumerable<ITableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            foreach (var tts in FindDescendant<Subquery>().EnumerateSourceTables(recursive))
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
