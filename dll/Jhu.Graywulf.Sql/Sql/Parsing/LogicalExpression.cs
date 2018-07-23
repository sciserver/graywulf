using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class LogicalExpression
    {
        public Predicate Predicate
        {
            get { return FindAscendant<Predicate>(); }
        }

        public LogicalExpressionBrackets BooleanExpressionBrackets
        {
            get { return FindAscendant<LogicalExpressionBrackets>(); }
        }

        #region Instance creation
        
        private static LogicalExpression CreateInternal(bool negated, Node n)
        {
            var sc = new LogicalExpression();
            if (negated)
            {
                sc.Stack.AddLast(LogicalNotOperator.Create());
                sc.Stack.AddLast(Whitespace.Create());
            }
            sc.Stack.AddLast(n);
            return sc;
        }

        public static LogicalExpression Create(bool negated, Predicate predicate)
        {
            return CreateInternal(negated, predicate);
        }

        public static LogicalExpression Create(bool negated, LogicalExpressionBrackets brackets)
        {
            return CreateInternal(negated, brackets);
        }

        public static LogicalExpression Create(LogicalExpression a, LogicalExpression b, LogicalOperator op)
        {
            var nsc = new LogicalExpression();

            nsc.Stack.AddLast(a);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(op);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(b);

            return nsc;
        }

        public static LogicalExpression Create(LogicalExpressionBrackets br, LogicalExpression sc, LogicalOperator op)
        {
            var nsc = new LogicalExpression();

            nsc.Stack.AddLast(br);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(op);
            nsc.Stack.AddLast(Whitespace.Create());
            nsc.Stack.AddLast(sc);

            return nsc;
        }

        #endregion
    }
}
