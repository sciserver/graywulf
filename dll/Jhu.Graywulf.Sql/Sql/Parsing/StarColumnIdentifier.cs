using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StarColumnIdentifier :  IColumnReference
    {
        private ColumnReference columnReference;

        public ColumnReference ColumnReference
        {
            get { return columnReference; }
            set { columnReference = value; }
        }
        
        public TableOrViewIdentifier TableOrViewIdentifier
        {
            get { return FindDescendant<TableOrViewIdentifier>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.columnReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (StarColumnIdentifier)other;

            this.columnReference = old.columnReference;
        }

        public static StarColumnIdentifier Create()
        {
            var ci = new StarColumnIdentifier();

            ci.columnReference = ColumnReference.CreateStar();
            ci.Stack.AddLast(Mul.Create());

            return ci;
        }

        public static StarColumnIdentifier Create(TableReference tableReference)
        {
            var ci = new StarColumnIdentifier();
            ci.columnReference = ColumnReference.CreateStar(tableReference);

            var ti = TableOrViewIdentifier.Create(tableReference);
            ci.Stack.AddLast(ti);
            ci.Stack.AddLast(Dot.Create());
            ci.Stack.AddLast(Mul.Create());
            
            return ci;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.columnReference = ColumnReference.Interpret(this);
        }
    }
}
