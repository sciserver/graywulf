using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class BeginEndStatement
    {
        public StatementBlock StatementBlock
        {
            get { return FindDescendant<StatementBlock>(); }
        }

        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            return StatementBlock.EnumerateSubStatements();
        }

        public static BeginEndStatement Create(StatementBlock statementBlock)
        {
            var ns = new BeginEndStatement();
            ns.Stack.AddLast(Keyword.Create("BEGIN"));

            if (!(statementBlock.Stack.First.Value is CommentOrWhitespace))
            {
                ns.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.CreateNewLine()));
            }

            ns.Stack.AddLast(statementBlock);

            if (!(statementBlock.Stack.Last.Value is CommentOrWhitespace))
            {
                ns.Stack.AddLast(CommentOrWhitespace.Create(Whitespace.CreateNewLine()));
            }

            ns.Stack.AddLast(Keyword.Create("END"));

            return ns;
        }
    }
}
