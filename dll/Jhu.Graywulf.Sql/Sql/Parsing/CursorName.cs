using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CursorName : ICursorReference
    {
        private CursorReference cursorReference;

        public CursorReference CursorReference
        {
            get { return cursorReference; }
            set { cursorReference = value; }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.cursorReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);
            var old = (CursorName)other;
            this.cursorReference = old.cursorReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.cursorReference = CursorReference.Interpret(this);
        }
    }
}
