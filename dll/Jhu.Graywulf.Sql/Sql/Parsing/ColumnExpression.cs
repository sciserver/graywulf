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
            get { return columnReference.ParentTableReference; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets or sets the table reference associated with this column expression
        /// </summary>
        /// <remarks></remarks>
        public TableReference TableReference
        {
            get { return columnReference.ParentTableReference; }
            set { columnReference.ParentTableReference = value; }
        }

        public UserVariable AssignedVariable
        {
            get { return FindDescendant<UserVariable>(); }
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
            var ci = ColumnIdentifier.CreateStar();
            var exp = Expression.Create(ci);
            var ce = Create(exp);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression CreateStar(TableReference tableReference)
        {
            var ci = ColumnIdentifier.CreateStar(tableReference);
            var exp = Expression.Create(ci);
            var ce = Create(exp);

            ce.columnReference = ci.ColumnReference;

            return ce;
        }

        public static ColumnExpression Create(Expression exp)
        {
            var ce = new ColumnExpression();

            ce.Stack.AddLast(exp);

            return ce;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
