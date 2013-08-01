using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token
    /// </summary>
    public abstract class Token
    {
        protected int pos;
        protected int line;
        protected int col;
        private string value;

        private Node parent;

        public int Pos
        {
            get { return pos; }
        }

        public int Line
        {
            get { return line; }
        }

        public int Col
        {
            get { return col; }
        }

        public virtual string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public Token()
        {
            InitializeMembers();
        }

        public Token(Token old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.pos = -1;
            this.line = -1;
            this.col = -1;
            this.value = null;

            this.parent = null;
        }

        private void CopyMembers(Token old)
        {
            this.pos = old.pos;
            this.line = old.line;
            this.col = old.col;
            this.value = old.value;

            this.parent = old.parent;
        }

        public abstract bool Match(Parser parser);

        public virtual bool AcceptCodeGenerator(CodeGenerator cg)
        {
            return cg.WriteToken(this);
        }

        public override string ToString()
        {
            return Value.Replace("\r\n", " ");
        }
    }
}
