using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnExpression : ITableReference, IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return columnReference.TableReference; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets or sets the table reference associated with this column expression
        /// </summary>
        /// <remarks></remarks>
        public TableReference TableReference
        {
            get { return columnReference.TableReference; }
            set { columnReference.TableReference = value; }
        }

        public UserVariable AssignedVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public ColumnAlias ColumnAlias
        {
            get { return FindDescendant<ColumnAlias>(); }
        }

        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }

        public StarColumnIdentifier StarColumnIdentifier
        {
            get { return FindDescendant<StarColumnIdentifier>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (ColumnExpression)other;

            this.columnReference = old.columnReference;
        }

        public static ColumnExpression CreateStar()
        {
            var ci = StarColumnIdentifier.Create();
            var ce = Create(ci);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression CreateStar(TableReference tableReference)
        {
            var ci = StarColumnIdentifier.Create(tableReference);
            var ce = Create(ci);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression Create(Expression exp, string alias)
        {
            var ce = new ColumnExpression();
            ce.Stack.AddLast(exp);

            if (!String.IsNullOrWhiteSpace(alias))
            {
                ce.Stack.AddLast(Whitespace.Create());
                ce.Stack.AddLast(Keyword.Create("AS"));
                ce.Stack.AddLast(Whitespace.Create());
                ce.Stack.AddLast(ColumnAlias.Create(alias));
            }

            return ce;
        }

        public static ColumnExpression Create(StarColumnIdentifier ci)
        {
            var ce = new ColumnExpression();
            ce.Stack.AddLast(ci);

            return ce;
        }
    }
}
