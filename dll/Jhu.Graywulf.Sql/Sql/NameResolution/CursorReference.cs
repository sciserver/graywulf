using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class CursorReference : ReferenceBase
    {
        #region Property storage variables

        private string cursorName;
        private string variableName;

        private VariableReference variableReference;

        #endregion
        #region Properties

        public string CursorName
        {
            get { return cursorName; }
            set { cursorName = value; }
        }

        public string VariableName
        {
            get { return variableName; }
            set { variableName = value; }
        }

        public override string UniqueName
        {
            get { return cursorName; }
            set { throw new InvalidOperationException(); }
        }

        public VariableReference VariableReference
        {
            get { return variableReference; }
            set { variableReference = value; }
        }

        #endregion
        #region Constructor and initializers

        public CursorReference()
        {
            InitializeMembers();
        }

        public CursorReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }

        public CursorReference(CursorReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.cursorName = null;
            this.variableName = null;
            this.variableReference = null;
        }

        private void CopyMembers(CursorReference old)
        {
            this.cursorName = old.cursorName;
            this.variableName = old.variableName;
            this.variableReference = old.variableReference;
        }

        public override object Clone()
        {
            return new CursorReference(this);
        }

        #endregion

        public static CursorReference Interpret(CursorName cn)
        {
            var cr = new CursorReference(cn)
            {
                cursorName = RemoveIdentifierQuotes(cn.Value)
            };
            return cr;
        }

        public static CursorReference Interpret(DeclareCursorStatement dc)
        {
            return InterpretCursorOperation(dc);
        }

        public static CursorReference Interpret(SetCursorStatement dc)
        {
            return InterpretCursorOperation(dc);
        }

        public static CursorReference Interpret(CursorOperationStatement dc)
        {
            return InterpretCursorOperation(dc);
        }

        public static CursorReference Interpret(FetchStatement fs)
        {
            return InterpretCursorOperation(fs.FromClause);
        }

        private static CursorReference InterpretCursorOperation(Node node)
        {
            CursorReference cr;
            var cn = node.FindDescendant<CursorName>();
            var uv = node.FindDescendant<UserVariable>();

            if (cn != null)
            {
                cr = cn.CursorReference;
            }
            else if (uv != null)
            {
                cr = new CursorReference(uv);
                cr.VariableName = uv.VariableName;
                cr.VariableReference = uv.VariableReference;
            }
            else
            {
                throw new NotImplementedException();
            }

            return cr;
        }
    }
}
