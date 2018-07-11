using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SetCursorStatement : ICursorReference
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
            var old = (SetCursorStatement)other;
            this.cursorReference = old.cursorReference;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.cursorReference = CursorReference.Interpret(this);
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
