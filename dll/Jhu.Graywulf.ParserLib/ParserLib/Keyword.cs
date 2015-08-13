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
    public sealed class Keyword : Literal, ICloneable
    {
        #region Constructors and initializers

        public Keyword()
            : base()
        {
        }

        public Keyword(Keyword old)
            :base(old)
        {
        }

        public Keyword(string keyword)
            :base(keyword)
        {
        }

        public override object Clone()
        {
            return new Keyword(this);
        }

        #endregion

        public static new Keyword Create(string keyword)
        {
            var res = new Keyword();

            res.LiteralText = res.Value = keyword;

            return res;
        }
    }
}
