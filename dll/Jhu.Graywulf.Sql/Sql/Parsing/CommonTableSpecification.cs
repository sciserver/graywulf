using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CommonTableSpecification : ITableSource
    {
        private string uniqueKey;

        public string UniqueKey
        {
            get { return uniqueKey; }
            set { uniqueKey = value; }
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

        public override void Interpret()
        {
            base.Interpret();

            TableReference = new TableReference(this);
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
