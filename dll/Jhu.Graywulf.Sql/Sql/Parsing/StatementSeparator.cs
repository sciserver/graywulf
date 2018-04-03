using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class StatementSeparator
    {
        public static StatementSeparator Create()
        {
            var ss = new StatementSeparator();
            ss.Stack.AddLast(Semicolon.Create());
            ss.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.CreateNewLine()));
            return ss;
        }
    }
}
