using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    class ExpressionPostfixReshuffler : ExpressionReshuffler
    {

        private Stack<Token> stack;

        public ExpressionPostfixReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
            :base(visitor, sink)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.stack = new Stack<Token>();
        }

        public override void Route(Token node)
        {
            switch (node)
            {
                case SimpleCaseExpression n:
                    throw new NotImplementedException();
                case SearchedCaseExpression n:
                    throw new NotImplementedException();

                // Function calls and property/member access
                case UdtStaticPropertyAccess n:
                    Output(node);
                    break;
                case UdtStaticMethodCall n:
                    Push(n);
                    break;
                case WindowedFunctionCall n:
                    Push(n);
                    break;
                case ScalarFunctionCall n:
                    Push(n);
                    break;
                case UdtMethodCall n:
                    Push(n);
                    break;
                case UdtPropertyAccess n:
                    Push(n);
                    break;

                // Windowed functions are rather special
                case OverClause n:
                    Push(n);
                    break;

                // Operators
                case UnaryOperator n:
                    Push(n);
                    break;
                case BinaryOperator n:
                    Push(n);
                    break;

                // Brackets
                case BracketOpen n:
                    Push(n);
                    break;
                case BracketClose n:
                    Pop(n);
                    break;

                // Default behavior
                default:
                    Output(node);
                    break;
            }
        }

        void Push(FunctionCall n)
        {
            stack.Push(n);
        }

        void Push(UdtPropertyAccess n)
        {
            stack.Push(n);
        }

        void Push(OverClause n)
        {
            stack.Push(n);
        }
        
        void Push(Operator n)
        {
            while (stack.Count > 0)
            {
                var p = stack.Peek();

                // TODO: unary operator ?

                if (p is FunctionCall || p is UdtPropertyAccess)
                {
                    Output(stack.Pop());
                }
                else if (p is Operator op &&
                        (op.Precedence < n.Precedence ||
                         op.LeftAssociative && op.Precedence == n.Precedence))
                {
                    Output(stack.Pop());
                }
                else
                {
                    break;
                }
            }

            stack.Push(n);
        }

        void Push(BracketOpen n)
        {
            stack.Push(n);
        }

        void Pop(BracketClose n)
        {
            // Empty down stack to first opening bracket
            // No need to test bracket pairing, that's done by parser
            while (stack.Count > 0)
            {
                var op = stack.Pop();
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

        public override void Flush()
        {
            while (stack.Count > 0)
            {
                Output(stack.Pop());
            }
        }
    }
}
