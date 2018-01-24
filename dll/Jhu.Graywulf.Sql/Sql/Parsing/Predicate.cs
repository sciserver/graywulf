using System;
using System.Collections.Generic;
using System.Linq;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class Predicate
    {
        public bool IsSpecificToTable(TableReference table)
        {
            foreach (var tr in NodeExtensions.EnumerateTableReferences(this))
            {
                if (tr != null && tr != table)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSpecificToTable(DatabaseObject table)
        {
            foreach (var tr in NodeExtensions.EnumerateTableReferences(this))
            {
                if (tr != null && (tr.DatabaseObject == null || tr.DatabaseObject != table))
                {
                    return false;
                }
            }

            return true;
        }

        public LogicalExpressions.Expression GetExpressionTree()
        {
            return new LogicalExpressions.Predicate(this);
        }

        public static Predicate CreateEquals(Expression a, Expression b)
        {
            return Create(a, b, new Equals1());
        }

        public static Predicate CreateLessThan(Expression a, Expression b)
        {
            return Create(a, b, new LessThan());
        }

        public static Predicate CreateLessThanOrEqual(Expression a, Expression b)
        {
            return Create(a, b, new LessThanOrEqual());
        }

        public static Predicate CreateGreaterThan(Expression a, Expression b)
        {
            return Create(a, b, new GreaterThan());
        }

        public static Predicate CreateGreaterThanOrEqual(Expression a, Expression b)
        {
            return Create(a, b, new GreaterThanOrEqual());
        }

        private static Predicate Create(Expression a, Expression b, Symbol op)
        {
            var predicate = new Predicate();
            var comparison = new ComparisonOperator();

            comparison.Stack.AddLast(op);

            predicate.Stack.AddLast(a);
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(comparison);
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(b);

            return predicate;
        }

        public static Predicate CreateBetween(Expression x, Expression a, Expression b)
        {
            var predicate = new Predicate();

            predicate.Stack.AddLast(x);
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(Keyword.Create("BETWEEN"));
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(a);
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(Keyword.Create("AND"));
            predicate.Stack.AddLast(Whitespace.Create());
            predicate.Stack.AddLast(b);

            return predicate;
        }
    }
}
