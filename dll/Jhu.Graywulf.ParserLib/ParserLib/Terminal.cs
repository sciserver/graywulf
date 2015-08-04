using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token that matches a terminal
    /// defined by a regular expression
    /// </summary>
    public abstract class Terminal : Token
    {
        protected abstract Regex Pattern
        {
            get;
        }

        
        public Terminal()
            :base()
        {
            InitializeMembers();
        }

        public Terminal(Terminal old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(Terminal old)
        {
        }

        public sealed override bool Match(Parser parser)
        {
            Match m = this.Pattern.Match(parser.Code, parser.Pos);

            // Exclude keywords from matches
            if (m.Success && !parser.Keywords.Contains(m.Value))
            {
                Value = m.Value;
                parser.GetLineCol(out pos, out line, out col);
                parser.Advance(m.Length);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
