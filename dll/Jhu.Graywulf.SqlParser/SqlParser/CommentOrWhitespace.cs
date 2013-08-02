using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class CommentOrWhitespace
    {
        public static CommentOrWhitespace Create(Token t)
        {
            var ncw = new CommentOrWhitespace();

            if (t is Whitespace || t is SingleLineComment || t is MultiLineComment)
            {
                ncw.Stack.AddLast(t);
            }
            else
            {
                throw new NotImplementedException();
            }

            return ncw;
        }
    }
}
