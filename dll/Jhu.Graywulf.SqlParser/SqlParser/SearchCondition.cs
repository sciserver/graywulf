using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public partial class SearchCondition
    {
        public SearchCondition()
            : base()
        {
        }

        #region Instance creation

        public static SearchCondition Create()
        {
            return new SearchCondition();
        }

        private static SearchCondition CreateInternal(bool negated, Node n)
        {
            var sc = new SearchCondition();
            if (negated)
            {
                sc.Stack.AddLast(LogicalNot.Create());
            }
            sc.Stack.AddLast(n);
            return sc;
        }

        public static SearchCondition Create(bool negated, Predicate predicate)
        {
            return CreateInternal(negated, predicate);
        }

        public static SearchCondition Create(bool negated, SearchConditionBrackets brackets)
        {
            return CreateInternal(negated, brackets);
        }

        public static SearchCondition Create(SearchConditionBrackets br, LogicalOperator op, SearchCondition sc)
        {
            var nsc = new SearchCondition();

            // TODO: add whitespaces here

            nsc.Stack.AddLast(br);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(op);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(sc);

            return nsc;
        }

        #endregion
        #region Expression tree functions

        /// <summary>
        /// Returns the search conditions in the form of an expression tree.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The resulting expression tree differs from the parsing tree which
        /// lists terms of complex logical expression as written and not by precedence,
        /// where as the expression tree observers operator precedence.
        /// </remarks>
        public LogicalExpressions.Expression GetExpressionTree()
        {
            // now build the tree from the stack
            return GetExpressionTreeInternal(ExecuteShuntingYard(EnumerateRawExpressions()));
        }

        /// <summary>
        /// Implements the shunting-yard algorithm to observe precedence of operators.
        /// </summary>
        /// <param name="rawExpressions"></param>
        private Stack<LogicalExpressions.Expression> ExecuteShuntingYard(IEnumerable<LogicalExpressions.Expression> rawExpressions)
        {
            // operator stack used by the shunting-yard algorithm
            var ops = new Stack<LogicalExpressions.Expression>();

            // output stack containing the 
            var sta = new Stack<LogicalExpressions.Expression>();

            foreach (var exp in rawExpressions)
            {
                if (exp.Precedence > 0)
                {
                    while (ops.Count > 0 && ops.Peek().Precedence > exp.Precedence)
                    {
                        sta.Push(ops.Pop());
                    }
                    ops.Push(exp);
                }
                else
                {
                    sta.Push(exp);
                }
            }

            while (ops.Count > 0)
            {
                sta.Push(ops.Pop());
            }

            return sta;
        }

        /// <summary>
        /// Traverses the parsing tree along search condition nodes and returns every important token
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function returns LogicalExpression.Brackets for the
        /// SearchConditionBrackets token which is not part of the final
        /// expression tree (eliminated by the shunting-yard algorimth
        /// in the function GetExpressionTree
        /// </remarks>
        private IEnumerable<LogicalExpressions.Expression> EnumerateRawExpressions()
        {
            SearchCondition sc = this;

            while (sc != null)
            {
                var not = sc.FindDescendant<LogicalNot>();

                if (not != null)
                {
                    yield return new LogicalExpressions.OperatorNot();
                }

                var pr = sc.FindDescendant<Predicate>();
                var br = sc.FindDescendant<SearchConditionBrackets>();

                if (pr != null)
                {
                    yield return pr.GetExpressionTree();
                }
                else if (br != null)
                {
                    yield return br.GetExpressionTree();
                }
                else
                {
                    throw new InvalidOperationException();
                }

                // see if there's an operator on the list
                var op = sc.FindDescendant<LogicalOperator>();

                if (op != null)
                {
                    // return the operator
                    if (op.IsOr)
                    {
                        yield return new LogicalExpressions.OperatorOr();
                    }
                    else if (op.IsAnd)
                    {
                        yield return new LogicalExpressions.OperatorAnd();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    // proceed to next iteration
                    sc = sc.FindDescendant<SearchCondition>();
                }
                else
                {
                    sc = null;
                }
            }
        }

        /// <summary>
        /// Builds the expression tree from the output stack created by the shunting-yard algorithm
        /// </summary>
        /// <param name="sta"></param>
        /// <returns></returns>
        private LogicalExpressions.Expression GetExpressionTreeInternal(Stack<LogicalExpressions.Expression> sta)
        {
            LogicalExpressions.Expression exp = sta.Pop();
            {
                if (exp is LogicalExpressions.BinaryExpression)
                {
                    var bexp = exp as LogicalExpressions.BinaryExpression;
                    bexp.Right = GetExpressionTreeInternal(sta);
                    bexp.Left = GetExpressionTreeInternal(sta);
                }
                else if (exp is LogicalExpressions.UnaryExpression)
                {
                    var uexp = exp as LogicalExpressions.UnaryExpression;
                    uexp.Operand = GetExpressionTreeInternal(sta);
                }
                else if (exp is LogicalExpressions.Brackets)
                {
                    // Brackets are not used in the expression tree, so eliminate them
                    var br = exp as LogicalExpressions.Brackets;
                    exp = br.Expression;
                }
            }

            return exp;
        }

        #endregion
    }
}
