using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Represents a token that matches keywords
    /// </summary>
    public class Literal : Token, ICloneable
    {
        #region Private members variables

        private static readonly Regex pattern = new Regex(@"\G[a-zA-Z][a-zA-Z0-9_]*", RegexOptions.Compiled);
        private string literalText;

        #endregion
        #region Properties

        protected Regex Pattern
        {
            get { return pattern; }
        }

        protected string LiteralText
        {
            get { return literalText; }
            set { literalText = value; }
        }

        #endregion
        #region Constructors and initializers

        public Literal()
            : base()
        {
        }

        public Literal(Literal old)
            :base(old)
        {
        }

        public Literal(string literalText)
            : this()
        {
            Value = this.literalText = literalText;
        }

        protected override void InitializeMembers()
        {
            base.InitializeMembers();

            this.literalText = null;
        }

        protected override void CopyMembers(object other)
        {
            base.CopyMembers(other);

            var old = (Literal)other;

            this.literalText = old.literalText;
        }

        public override object Clone()
        {
            return new Literal(this);
        }

        #endregion

        public static Literal Create(string literalText)
        {
            var res = new Literal();

            res.literalText = res.Value = literalText;

            return res;
        }

        public override bool Match(Parser parser)
        {
            Match m = this.Pattern.Match(parser.Code, parser.Pos);

            if (m.Success && parser.Comparer.Compare(m.Value, literalText) == 0)
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
