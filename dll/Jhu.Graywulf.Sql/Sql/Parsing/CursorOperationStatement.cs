using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CursorOperationStatement : ICursorReference
    {
        private CursorReference cursorReference;

        public CursorName CursorName
        {
            get { return FindDescendant<CursorName>(); }
        }

        public UserVariable UserVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public CursorDefinition CursorDefinition
        {
            get { return FindDescendant<CursorDefinition>(); }
        }

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
            var old = (CursorOperationStatement)other;
            this.cursorReference = old.cursorReference;
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.cursorReference = CursorReference.Interpret(this);
        }
    }
}
