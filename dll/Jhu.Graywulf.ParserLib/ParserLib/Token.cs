using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token
    /// </summary>
    public abstract class Token : ICloneable
    {
        #region Private member variables

        protected int pos;
        protected int line;
        protected int col;
        private string value;

        private Node parent;

        #endregion
        #region Properties

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

        #endregion
        #region Constructors and initializers

        public Token()
        {
            InitializeMembers();
        }

        public Token(Token old)
        {
            CopyMembers(old);
        }

        protected virtual void InitializeMembers()
        {
            this.pos = -1;
            this.line = -1;
            this.col = -1;
            this.value = null;

            this.parent = null;
        }

        protected virtual void CopyMembers(object other)
        {
            var old = (Token)other;

            this.pos = old.pos;
            this.line = old.line;
            this.col = old.col;
            this.value = old.value;

            this.parent = old.parent;
        }

        public abstract object Clone();

        #endregion

        public abstract bool Match(Parser parser);

        public override string ToString()
        {
            return Value.Replace("\r\n", " ");
        }
    }
}
