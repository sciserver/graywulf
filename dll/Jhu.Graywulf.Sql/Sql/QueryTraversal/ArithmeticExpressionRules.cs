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
    class ArithmeticExpressionRules : ExpressionReshufflerRules
    {
        public override void Route(Token node)
        {
            switch (node)
            {
                // Methods act on the previous result so must empty the top of
                // the operator stack first. Functions are simply pushed to the top
                // and arguments are counted.
                case MemberAccess n:
                    EmptyAndPush(n);
                    break;
                case MemberCall n:
                    EmptyAndPush(n);
                    break;
                case PropertyAccess n:
                    EmptyAndPush(n);
                    break;
                case MethodCall n:
                    EmptyAndPush(n);
                    break;

                // Function calls and property/member access
                case SystemFunctionCall n:
                    Push(n);
                    break;
                case WindowedFunctionCall n:
                    Push(n);
                    break;
                case ScalarFunctionCall n:
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

                // Windowed functions are rather special, for postfix, push to stack to
                // put it before function just like its arguments
                case OverClause n:
                    Inline(n);
                    break;

                // Case expression are operands, just push to output
                case CaseExpression n:
                    Inline(n);
                    break;

                // Default behavior: skip token
                default:
                    break;
            }
        }
    }
}
