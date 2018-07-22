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
    class ExpressionPostfixReshuffler : ExpressionReshuffler
    {
        protected Stack<Token> operatorStack;

        public override TraversalDirection Direction
        {
            get { return TraversalDirection.Forward; }
        }

        public ExpressionPostfixReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink)
            : base(visitor, sink)
        {
            this.operatorStack = new Stack<Token>();
        }

        public override void Output(Token node)
        {
            base.Output(node);

            // Inline constructs such as OVER and CASE needs to be passed back to the
            // query visitor for further traversal

            switch (node)
            {
                case OverClause oc:
                    visitor.TraverseInline(node);
                    break;
                default:
                    break;
            }
        }

        public override void Route(Token node)
        {
            switch (node)
            {
                case SimpleCaseExpression n:
                    throw new NotImplementedException();
                case SearchedCaseExpression n:
                    throw new NotImplementedException();

                // Brackets
                case BracketOpen n:
                    Push(n);
                    break;
                case BracketClose n:
                    Pop(n);
                    break;

                // Function calls and property/member access
                case MemberAccess n:
                    Push(n);
                    break;
                case MemberCall n:
                    Push(n);
                    break;
                case PropertyAccess n:
                    Push(n);
                    break;
                case MethodCall n:
                    Push(n);
                    break;

                case SystemFunctionCall n:
                    Push(n);
                    break;
                case WindowedFunctionCall n:
                    Push(n);
                    break;
                case ScalarFunctionCall n:
                    Push(n);
                    break;

                // Windowed functions are rather special, for postfix, push to stack to
                // put it before function just like its arguments
                case OverClause n:
                    Push(n);
                    break;

                // Operators
                case Operator n:
                    Push(n);
                    break;

                case Comma n:
                    Push(n);
                    break;

                // Operands and other important tokens that go directly
                // to the output
                case Constant c:
                case CountStar cs:
                case SystemVariable sv:
                case UserVariable uv:
                case ExpressionSubquery sq:
                case ObjectName on:
                case DataTypeIdentifier di:
                    Output(node);
                    break;

                // Default behavior: skip token
                default:
                    break;
            }
        }

        #region Method and function calls

        // Methods act on the previous result so must empty the top of
        // the operator stack first. Functions are simply pushed to the top
        // and arguments are counted.

        private void Push(MethodCall n)
        {
            FlushTop(n);
            operatorStack.Push(n);
        }

        private void Push(MemberCall n)
        {
            FlushTop(n);
            operatorStack.Push(n);
        }

        void Push(FunctionCall n)
        {
            operatorStack.Push(n);
        }

        void Push(OverClause n)
        {
            operatorStack.Push(n);
        }

        #endregion

        void Push(PropertyAccess n)
        {
            FlushTop(n);
            operatorStack.Push(n);
        }

        private void Push(MemberAccess n)
        {
            FlushTop(n);
            operatorStack.Push(n);
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

                if (p is FunctionCall || p is MethodCall || p is MemberCall ||
                    p is MemberAccess || p is PropertyAccess)
                {
                    Output(operatorStack.Pop());
                    continue;
                }
                else
                {
                    var op1 = p as Operator;
                    var op2 = n as Operator;

                    if (op1 != null && op2 != null &&
                       (op1.Precedence < op2.Precedence ||
                        op1.LeftAssociative && op1.Precedence == op2.Precedence))
                    {
                        Output(operatorStack.Pop());
                        continue;
                    }
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
