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
    public sealed class Keyword : Literal
    {
        public Keyword()
            : base()
        {
            InitializeMembers();
        }

        public Keyword(string keyword)
            :base(keyword)
        {
            InitializeMembers();
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

            res.LiteralText = res.Value = keyword;

            return res;
        }
    }
}
