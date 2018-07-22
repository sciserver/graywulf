using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    class ExpressionPrefixReshuffler : ExpressionPostfixReshuffler
    {
        private Stack<Token> outputStack;

        public ExpressionPrefixReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
            : base(visitor, sink)
        {
            this.outputStack = new Stack<Token>();
        }

        protected override void Output(Token n)
        {
            outputStack.Push(n);
        }

        public override void Route(Token node)
        {
            switch (node)
            {
                // Brackets work the opposite because reading from the end
                case BracketClose n:
                    Push(n);
                    break;
                case BracketOpen n:
                    Pop(n);
                    break;
                default:
                    base.Route(node);
                    break;

                case Comma n:
                    Push(n);
                    break;
            }
        }

        private void Push(BracketClose n)
        {
            operatorStack.Push(n);
        }

        private void Pop(BracketOpen n)
        {
            // Empty down stack to first _closing_ bracket
            // No need to test bracket pairing, that's done by parser
            while (operatorStack.Count > 0)
            {
                var op = operatorStack.Pop();
                if (op is BracketClose)
                {
                    break;
                }
                else
                {
                    Output(op);
                }
            }
        }

        private void Push(Comma n)
        {
            // Pop operator stack to have the function call on top
            while (!(operatorStack.Peek() is BracketClose))
            {
                Output(operatorStack.Pop());
            }
            Output(n);
        }

        public override void Flush()
        {
            base.Flush();

            while (outputStack.Count > 0)
            {
                base.Output(outputStack.Pop());
            }
        }
    }
}
