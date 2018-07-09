using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class QuerySpecification : ISourceTableCollection
    {
        #region Private member variables

        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference resultsTableReference;

        #endregion
        #region Properties

        public Dictionary<string, TableReference> ResolvedSourceTableReferences
        {
            get { return sourceTableReferences; }
        }

        /// <summary>
        /// Gets the reference to the table that represents
        /// the resultset of the query
        /// </summary>
        public TableReference ResultsTableReference
        {
            get { return resultsTableReference; }
            set { resultsTableReference = value; }
        }

        public SelectList SelectList
        {
            get { return FindDescendant<SelectList>(); }
        }

        public IntoClause IntoClause
        {
            get { return FindDescendant<IntoClause>(); }
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
        /// Enumerates source tables referenced in the current, and optionally in
        /// all subqueries.
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITableSource> EnumerateSourceTables(bool recursive)
        {
            // Start from the FROM clause, if specified, otherwise no
            // table sources in the query
            var from = FromClause;
            var where = WhereClause;

            if (from != null)
            {
                foreach (var ts in from.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }

            if (where != null)
            {
                foreach (var ts in where.EnumerateSourceTables(recursive))
                {
                    yield return ts;
                }
            }
        }

        /// <summary>
        /// Enumerates through all table sources and returns every TableReference
        /// associated with the table source
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public IEnumerable<TableReference> EnumerateSourceTableReferences(bool recursive)
        {
            return EnumerateSourceTables(recursive).Select(ts => ts.TableReference);
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
