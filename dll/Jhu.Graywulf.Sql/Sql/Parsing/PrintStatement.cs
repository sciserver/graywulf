using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class PrintStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Command; }
        }

        public static PrintStatement Create(string message)
        {
            var p = new PrintStatement();
            p.Stack.AddLast(Keyword.Create("PRINT"));
            p.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.Create()));
            p.Stack.AddLast(Expression.CreateString(message));

            return p;
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
