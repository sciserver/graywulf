using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class QuerySpecification : ISourceTableProvider
    {
        #region Private member variables

        private QuerySpecification parentQuerySpecification;
        private Dictionary<string, TableReference> sourceTableReferences;
        private TableReference resultsTableReference;

        #endregion
        #region Properties

        public QuerySpecification ParentQuerySpecification
        {
            get { return parentQuerySpecification; }
            set { parentQuerySpecification = value; }
        }

        public Dictionary<string, TableReference> SourceTableReferences
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

        public TableSource FirstTableSource
        {
            get { return FromClause?.FirstTableSource; }
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

        public GroupByClause GroupByClause
        {
            get { return FindDescendant<GroupByClause>(); }
        }

        public HavingClause HavingClause
        {
            get { return FindDescendant<HavingClause>(); }
        }

        #endregion
        #region Constructors and initializers

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.parentQuerySpecification = null;
            this.sourceTableReferences = new Dictionary<string, TableReference>(Schema.SchemaManager.Comparer);
            this.resultsTableReference = new TableReference(this);
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (QuerySpecification)other;

            this.parentQuerySpecification = old.parentQuerySpecification;
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
            var wsn = Stack.Insert(Stack.IndexOf(FromClause), ws);
            Stack.Insert(wsn, where);
        }

        public void AppendSearchCondition(LogicalExpression condition, string op)
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
