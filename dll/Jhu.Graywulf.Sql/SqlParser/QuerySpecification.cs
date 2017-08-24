using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;


namespace Jhu.Graywulf.SqlParser
{
    public partial class QuerySpecification
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference resultsTableReference;

        #endregion
        #region Properties

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
            get { return FindDescendant<SelectList>(); }
        }

        public FromClause FromClause
        {
            get { return FindDescendant<FromClause>(); }
        }

        public WhereClause WhereClause
        {
            get { return FindDescendant<WhereClause>(); }
        }

        #endregion
        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
            this.resultsTableReference = new TableReference(this);
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (QuerySpecification)other;

            this.sourceTableReferences = new Dictionary<string, TableReference>(old.sourceTableReferences);
            this.resultsTableReference = new TableReference(old.resultsTableReference);
        }

        #endregion

        public static QuerySpecification Create(SelectList selectList, FromClause from)
        {
            var qs = new QuerySpecification();

            qs.Stack.AddLast(Keyword.Create("SELECT"));
            qs.Stack.AddLast(Whitespace.Create());
            qs.Stack.AddLast(selectList);

            if (from != null)
            {
                qs.Stack.AddLast(Whitespace.CreateNewLine());
                qs.Stack.AddLast(from);
            }

            return qs;
        }

        /// <summary>
        /// Enumerates all subqueries of the query specification recursively.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Subquery> EnumerateSubqueries()
        {
            return this.EnumerateDescendantsRecursive<Subquery>(typeof(Subquery));
        }

        /// <summary>
        /// Enumerates source tables referenced in the current, and optionally in
        /// all subqueries.
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

                    // Enumerate recursively, if necessary
                    if (recursive && ts.SpecificTableSource.IsSubquery)
                    {
                        foreach (var tts in ts.SpecificTableSource.EnumerateSubqueryTableSources(recursive))
                        {
                            yield return tts;
                        }
                    }

                    // TODO: extend here to return all tables from
                    // the XMATCH table source
                    if (ts.SpecificTableSource.IsMultiTable)
                    {
                        foreach (var mts in ts.SpecificTableSource.EnumerateMultiTableSources())
                        {
                            yield return mts;

                            // Enumerate recursively, if necessary
                            if (recursive && mts.IsSubquery)
                            {
                                foreach (var tts in mts.EnumerateSubqueryTableSources(recursive))
                                {
                                    yield return tts;
                                }
                            }
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
                    foreach (var ts in sq.EnumerateSourceTables(recursive))
                    {
                        yield return ts;
                    }
                }
            }

            // TODO: add functionality to handle semi-join constructs
            // verify this, because might be covered by the where clause above
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

            // Currently used by the Parser UI only, need to remove.

            WhereClause wh = FindDescendant<WhereClause>();

            if (wh == null)
            {
                yield break;
            }
            else
            {
                BooleanExpression sc = wh.FindDescendant<BooleanExpression>();
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
                        else if (n is BooleanExpressionBrackets)
                        {
                            wc = new SearchConditionReference((BooleanExpressionBrackets)n);
                            yield return wc;
                        }
                    }
                }
            }
        }

        #region Query construction functions

        public void AppendWhereClause(WhereClause where)
        {
            if (where == null)
            {
                throw new ArgumentNullException();
            }

            if (WhereClause != null)
            {
                throw new InvalidOperationException();
            }

            // The where clause follows the from clause but a
            // whitespace needs to be added first
            var ws = Whitespace.CreateNewLine();
            var wsn = Stack.AddAfter(Stack.Find(FromClause), ws);
            Stack.AddAfter(wsn, where);
        }

        public void AppendSearchCondition(BooleanExpression condition, string op)
        {
            if (condition == null)
            {
                throw new ArgumentNullException();
            }

            WhereClause where = WhereClause;

            if (where == null)
            {
                where = WhereClause.Create(condition);
                AppendWhereClause(where);
            }
            else
            {
                where.AppendCondition(condition, op);
            }
        }

        #endregion
    }
}
