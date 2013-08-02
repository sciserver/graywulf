using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token that matches a symbol
    /// </summary>
    public abstract class Symbol : Token
    {
        protected abstract string Pattern
        {
            get;
        }

        public Symbol()
            : base()
        {
            InitializeMembers();
        }

        public Symbol(string symbol)
        {
            InitializeMembers();
            Value = symbol;
        }

        public Symbol(Symbol old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(Symbol old)
        {
        }

        public sealed override bool Match(Parser parser)
        {
            if (parser.Code.Length >= parser.Pos + Pattern.Length &&
                parser.Comparer.Compare(parser.Code.Substring(parser.Pos, Pattern.Length), Pattern) == 0)
            {
                this.Value = Pattern;
                parser.GetLineCol(out pos, out line, out col);
                parser.Advance(Pattern.Length);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
