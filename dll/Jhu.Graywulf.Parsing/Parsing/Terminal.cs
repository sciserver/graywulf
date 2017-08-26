using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Parsing
{
    /// <summary>
    /// Represents a token that matches a terminal
    /// defined by a regular expression
    /// </summary>
    public abstract class Terminal : Token, ICloneable
    {
        #region Properties

        protected abstract Regex Pattern
        {
            get;
        }

        #endregion
        #region Constructors and initializers

        protected Terminal()
            :base()
        {
        }

        protected Terminal(Terminal old)
            :base(old)
        {
        }

        #endregion

        public sealed override bool Match(Parser parser)
        {
            Match m = this.Pattern.Match(parser.Code, parser.Pos);

            // Exclude keywords from matches
            if (m.Success && !parser.Keywords.Contains(m.Value))
            {
                Value = m.Value;
                parser.GetLineCol(parser.Pos, out line, out col);
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
