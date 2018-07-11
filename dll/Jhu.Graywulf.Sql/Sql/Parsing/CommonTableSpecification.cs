using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CommonTableSpecification
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

        public override void Interpret()
        {
            base.Interpret();

            TableReference = TableReference.Interpret(this);
        }

        public override IEnumerable<TableSource> EnumerateSubqueryTableSources(bool recursive)
        {
            foreach (var tts in Subquery.EnumerateSourceTables(recursive))
            {
                yield return tts;
            }
        }

        public override IEnumerable<TableSource> EnumerateMultiTableSources()
        {
            throw new NotImplementedException();
        }
    }
}
