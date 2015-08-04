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
    public class Literal : Token
    {
        private Regex pattern = new Regex(@"\G[a-zA-Z][a-zA-Z0-9_]*");

        protected Regex Pattern
        {
            get { return pattern; }
        }

        private string literalText;

        protected string LiteralText
        {
            get { return literalText; }
            set { literalText = value; }
        }

        public Literal()
            : base()
        {
            InitializeMembers();
        }

        public Literal(string literalText)
        {
            InitializeMembers();
            Value = this.literalText = literalText;
        }

        public Literal(Literal old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(Literal old)
        {
        }

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
