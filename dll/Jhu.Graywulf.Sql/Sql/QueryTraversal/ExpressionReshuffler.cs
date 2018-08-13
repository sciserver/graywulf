using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    public class ExpressionReshuffler
    {
        private SqlQueryVisitor visitor;
        private SqlQueryVisitorSink sink;
        private ExpressionReshufflerRules rules;
        private ExpressionTraversalMethod method;

        private Stack<Token> operatorStack;
        private Stack<Token> outputStack;
        private HashSet<Token> inlineSet;

        public TraversalDirection Direction
        {
            get
            {
                switch (method)
                {
                    case ExpressionTraversalMethod.Infix:
                    case ExpressionTraversalMethod.Postfix:
                        return TraversalDirection.Forward;
                    case ExpressionTraversalMethod.Prefix:
                        return TraversalDirection.Backward;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public ExpressionReshuffler(SqlQueryVisitor visitor, SqlQueryVisitorSink sink, ExpressionTraversalMethod method, ExpressionReshufflerRules rules)
        {
            this.visitor = visitor;
            this.sink = sink;
            this.method = method;
            this.rules = rules;

            this.operatorStack = new Stack<Token>();

            if (method == ExpressionTraversalMethod.Prefix)
            {
                this.outputStack = new Stack<Token>();
            }

            inlineSet = new HashSet<Token>();

            this.rules.Init(this);
        }

        public void Route(Token node)
        {
            switch (node)
            {
                // Brackets
                case BracketOpen n:
                    switch (method)
                    {
                        case ExpressionTraversalMethod.Postfix:
                            Push(n);
                            break;
                        case ExpressionTraversalMethod.Prefix:
                            // Empty down stack to first _closing_ bracket
                            // No need to test bracket pairing, that's done by parser
                            EmptyBelow<BracketClose>();
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case BracketClose n:
                    switch (method)
                    {
                        case ExpressionTraversalMethod.Postfix:
                            // Empty down stack to first opening bracket
                            // No need to test bracket pairing, that's done by parser
                            EmptyBelow<BracketOpen>();
                            break;
                        case ExpressionTraversalMethod.Prefix:
                            Push(n);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;

                // Function calls and operands are routed by rules because their behavior depends
                // on whether they're functions or methods

                // All operators
                case Operator n:
                    EmptyWithPrecedence(n);
                    Push(n);
                    break;

                // If it is a new argument, pop operator stack to have the bracket on top
                case Comma nc:
                case Literal nl:        // AS, USING in special functions
                    switch (method)
                    {
                        case ExpressionTraversalMethod.Postfix:
                            EmptyUntil<BracketOpen>();
                            Output(node);
                            break;
                        case ExpressionTraversalMethod.Prefix:
                            EmptyUntil<BracketClose>();
                            Output(node);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;

                default:
                    rules.Route(node);
                    break;
            }
        }

        public void Push(Token node)
        {
            operatorStack.Push(node);
        }

        public void EmptyAndPush(Node node)
        {
            EmptyWithPrecedence(node);
            operatorStack.Push(node);
        }

        public void EmptyBelow<T>()
            where T : Token
        {
            // Empty down stack below the first token of T
            // No need to test bracket pairing, that's done by parser
            while (operatorStack.Count > 0)
            {
                var t = operatorStack.Pop();
                if (t is T)
                {
                    break;
                }
                else
                {
                    Output(t);
                }
            }
        }

        public void EmptyUntil<T>()
            where T : Token
        {
            // Empty stack down until the first token of T
            // Pop operator stack to have the function call on top
            while (operatorStack.Count > 0)
            {
                var t = operatorStack.Peek();
                if (t is T)
                {
                    break;
                }
                else
                {
                    Output(operatorStack.Pop());
                }
            }
        }

        public void EmptyWithPrecedence(Node n)
        {
            while (operatorStack.Count > 0)
            {
                var p = operatorStack.Peek();

                if (p is FunctionCall || p is MemberAccess || p is PropertyAccess)
                {
                    // TODO: Is it ever called?
                    Output(operatorStack.Pop());
                    continue;
                }
                else
                {
                    var op1 = p as Operator;
                    var op2 = n as Operator;

                    if (op1 != null && op2 != null &&
                       (op1.Precedence < op2.Precedence ||
                        op1.IsLeftAssociative && op1.Precedence == op2.Precedence))
                    {
                        Output(operatorStack.Pop());
                        continue;
                    }
                }

                break;
            }
        }

        public void Flush()
        {
            while (operatorStack.Count > 0)
            {
                Output(operatorStack.Pop());
            }

            if (outputStack != null)
            {
                while (outputStack.Count > 0)
                {
                    var node = outputStack.Pop();
                    Visit(node);
                }
            }
        }

        public void Inline(Token node)
        {
            inlineSet.Add(node);
        }

        public virtual void Output(Token node)
        {
            if (outputStack != null)
            {
                outputStack.Push(node);
            }
            else
            {
                Visit(node);
            }
        }

        private void Visit(Token node)
        {
            if (inlineSet.Contains(node))
            {
                visitor.TraverseInlineNode(node);
            }
            else
            {
                sink.AcceptVisitor(visitor, node);
            }
        }
    }
}
