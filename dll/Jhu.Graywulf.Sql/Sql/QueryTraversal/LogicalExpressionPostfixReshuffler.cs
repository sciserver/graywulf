using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    /// <summary>
    /// Implements a specialized version of the shuntling yard algorithm to output
    /// expressions in reverse polish notation.
    /// </summary>
    class LogicalExpressionPostfixReshuffler : ExpressionReshuffler
    {
        protected Stack<Token> operatorStack;

        public override TraversalDirection Direction
        {
            get { return TraversalDirection.Forward; }
        }

        public LogicalExpressionPostfixReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
            : base(visitor, sink)
        {
            this.operatorStack = new Stack<Token>();
        }

        public override void Output(Token node)
        {
            base.Output(node);

            // Inline all predicates because they're all special

            switch (node)
            {
                case Predicate p:
                    visitor.TraverseLogicalExpressionInlineNode(node);
                    break;
                default:
                    break;
            }
        }

        public override void Route(Token node)
        {
            switch (node)
            {
                // Brackets
                case BracketOpen n:
                    Push(n);
                    break;
                case BracketClose n:
                    Pop(n);
                    break;

                // Predicates are inlined and sent directly to the output
                case Predicate n:
                    Output(n);
                    break;

                // Operators
                case Operator n:
                    Push(n);
                    break;

                // Default behavior: skip token
                default:
                    break;
            }
        }

        private void Push(Operator n)
        {
            FlushTop(n);
            operatorStack.Push(n);
        }

        private void Push(Comma n)
        {
            // Pop operator stack to have the function call on top
            while (!(operatorStack.Peek() is BracketOpen))
            {
                Output(operatorStack.Pop());
            }
            Output(n);
        }

        private void Push(BracketOpen n)
        {
            operatorStack.Push(n);
        }

        private void Pop(BracketClose n)
        {
            // Empty down stack to first opening bracket
            // No need to test bracket pairing, that's done by parser
            while (operatorStack.Count > 0)
            {
                var op = operatorStack.Pop();
                if (op is BracketOpen)
                {
                    break;
                }
                else
                {
                    Output(op);
                }
            }
        }

        private void FlushTop(Node n)
        {
            while (operatorStack.Count > 0)
            {
                var p = operatorStack.Peek();

                var op1 = p as Operator;
                var op2 = n as Operator;

                if (op1 != null && op2 != null &&
                    (op1.Precedence < op2.Precedence ||
                    op1.IsLeftAssociative && op1.Precedence == op2.Precedence))
                {
                    Output(operatorStack.Pop());
                    continue;
                }

                break;
            }
        }

        public override void Flush()
        {
            while (operatorStack.Count > 0)
            {
                Output(operatorStack.Pop());
            }
        }
    }
}
