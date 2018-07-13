using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Expression
    {
        /// <summary>
        /// Returns true if the expression is a single column name
        /// </summary>
        /// <returns></returns>
        public bool IsSingleColumn
        {
            get
            {
                // A single column expression can only have the following nodes:
                // - CommentOrWhitespace
                // - ExpressionBrackets
                // - Expression
                // - ColumnIdentifier

                foreach (var t in this.EnumerateDescendantsRecursive<Node>(typeof(ColumnIdentifier)))
                {
                    if (!(t is CommentOrWhitespace ||
                          t is ColumnIdentifier ||
                          t is ExpressionBrackets ||
                          t is Expression))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsConstantNumber
        {
            get
            {
                if (Stack.Count == 1)
                {
                    var nc = FindDescendant<Constant>()?.FindDescendant<NumericConstant>();
                    if (nc != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static Expression Create(ColumnIdentifier ci)
        {
            var nex = new Expression();
            nex.Stack.AddLast(ci);

            return nex;
        }

        public static Expression Create(ColumnReference cr)
        {
            var ci = ColumnIdentifier.Create(cr);
            return Create(ci);
        }

        public static Expression Create(SystemVariable var)
        {
            var nex = new Expression();
            nex.Stack.AddLast(var);
            return nex;
        }

        public static Expression Create(UserVariable var)
        {
            var nex = new Expression();
            nex.Stack.AddLast(var);
            return nex;
        }

        public static Expression Create(ScalarFunctionCall fun)
        {
            var nex = new Expression();
            nex.Stack.AddLast(fun);
            return nex;
        }

        public static Expression Create(FunctionReference fr, Expression[] args)
        {
            var exp = new Expression();
            exp.Stack.AddLast(ScalarFunctionCall.Create(fr, args));
            return exp;
        }

        public static Expression Create(VariableReference vr, MethodReference mr, Expression[] args)
        {
            var exp = new Expression();
            var m = UdtMethodCall.Create(mr, args);

            exp.Stack.AddLast(UserVariable.Create(vr));
            exp.Stack.AddLast(UdtMemberList.Create(m));

            return exp;
        }
        
        public static Expression CreateNumber(string number)
        {
            var nex = new Expression();
            nex.Stack.AddLast(Constant.CreateNumeric(number));
            return nex;
        }

        public static Expression CreateString(string s)
        {
            var nex = new Expression();
            nex.Stack.AddLast(Constant.CreateString(s));
            return nex;
        }
    }
}
