using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token that matches a symbol
    /// </summary>
    public abstract class Symbol : Token, ICloneable
    {
        #region Properties

        protected abstract string Pattern
        {
            get;
        }

        #endregion
        #region Constructors and initializers

        protected Symbol()
            :base()
        {
        }

        protected Symbol(Symbol old)
            : base(old)
        {
        }

        protected Symbol(string symbol)
            :this()
        {
            Value = symbol;
        }

        #endregion

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
