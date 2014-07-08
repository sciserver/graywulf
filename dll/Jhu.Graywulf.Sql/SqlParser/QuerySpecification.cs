using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;


namespace Jhu.Graywulf.SqlParser
{
    public partial class QuerySpecification
    {
        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference resultsTableReference;

        public Dictionary<string, TableReference> SourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        public TableReference ResultsTableReference
        {
            get { return resultsTableReference; }
        }

        public SelectList SelectList
        {
            get
            {
                return FindDescendant<SelectList>();
            }
        }

        public QuerySpecification()
            : base()
        {
            InitializeMembers();
        }

        public QuerySpecification(QuerySpecification old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
            this.resultsTableReference = new TableReference(this);
        }

        private void CopyMembers(QuerySpecification old)
        {
            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
            this.resultsTableReference = new TableReference(old.resultsTableReference);
        }

        public IEnumerable<Subquery> EnumerateSubqueries()
        {
            return this.EnumerateDescendantsRecursive<Subquery>(typeof(Subquery));
        }

        /// <summary>
        /// Enumerates source tables referenced in the current, and optionally in
        /// all sub-, queries.
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            // Start from the FROM clause, if specified, otherwise no
            // table sources in the query
            var from = FindDescendant<FromClause>();
            if (from != null)
            {
                var node = (Node)from.FindDescendant<TableSourceExpression>();

                while (node != null)
                {
                    var ts = node.FindDescendant<TableSource>();
                    yield return ts.SpecificTableSource;

                    if (recursive && ts.SpecificTableSource is SubqueryTableSource)
                    {
                        var sts = (SubqueryTableSource)ts.SpecificTableSource;
                        foreach (var tts in sts.FindDescendant<Subquery>().SelectStatement.EnumerateSourceTables(recursive))
                        {
                            yield return tts;
                        }
                    }

                    node = node.FindDescendant<JoinedTable>();
                }
            }

            var where = FindDescendant<WhereClause>();
            if (where != null)
            {
                foreach (var sq in where.EnumerateDescendantsRecursive<Subquery>(typeof(Subquery)))
                {
                    foreach (var ts in sq.SelectStatement.EnumerateSourceTables(recursive))
                    {
                        yield return ts;
                    }
                }
            }

            // TODO: add functionality to handle semi-join constructs
        }

        /// <summary>
        /// Enumerates through all table sources and returns every TableReference
        /// associated with the table source
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive)
        {
            return EnumerateSourceTables(recursive).Select(ts => ts.TableReference);
        }

        public virtual IEnumerable<ColumnExpression> EnumerateSelectListColumnExpressions()
        {
            return SelectList.EnumerateDescendantsRecursive<ColumnExpression>();
        }

        public virtual IEnumerable<ColumnIdentifier> EnumerateSelectListColumnIdentifiers()
        {
            return SelectList.EnumerateDescendantsRecursive<ColumnIdentifier>();
        }

        public IEnumerable<SearchConditionReference> EnumerateConditions()
        {
            //*** This goes to the SkyQueryJob and the Condition normalizer
            // TODO: this has to be updated to handle non conjunctive normal form predicates too
            WhereClause wh = FindDescendant<WhereClause>();

            if (wh == null)
            {
                yield break;
            }
            else
            {
                SearchCondition sc = wh.FindDescendant<SearchCondition>();
                if (sc == null)
                {
                    yield break;
                }
                else
                {
                    foreach (object n in sc.Nodes)
                    {
                        SearchConditionReference wc;
                        if (n is Predicate)
                        {
                            wc = new SearchConditionReference((Predicate)n);
                            yield return wc;
                        }
                        else if (n is SearchConditionBrackets)
                        {
                            wc = new SearchConditionReference((SearchConditionBrackets)n);
                            yield return wc;
                        }
                    }
                }
            }
        }
    }
}
