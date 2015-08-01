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
    public sealed class Keyword : Token
    {
        private Regex pattern = new Regex(@"\G[a-zA-Z][a-zA-Z0-9_]*");

        private Regex Pattern
        {
            get { return pattern; }
        }

        private string keyword;

        public Keyword()
            : base()
        {
            InitializeMembers();
        }

        public Keyword(string keyword)
        {
            InitializeMembers();
            Value = this.keyword = keyword;
        }

        public Keyword(Keyword old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(Keyword old)
        {
        }

        public static Keyword Create(string keyword)
        {
            var res = new Keyword();

            res.keyword = res.Value = keyword;

            return res;
        }

        public override bool Match(Parser parser)
        {
            Match m = this.Pattern.Match(parser.Code, parser.Pos);

            if (m.Success && parser.Comparer.Compare(m.Value, keyword) == 0)
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
