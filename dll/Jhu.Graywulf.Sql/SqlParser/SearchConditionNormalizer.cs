using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    public class SearchConditionNormalizer
    {
        private List<LogicalExpressions.Expression> conditions;

        public SearchConditionNormalizer()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public void CollectConditions(SelectStatement selectStatement)
        {
            CollectConditions(selectStatement.QueryExpression);
        }

        public void CollectConditions(QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendants<QuerySpecification>())
            {
                CollectConditions(qs);
            }
        }

        public void CollectConditions(QuerySpecification qs)
        {
            foreach (var sq in qs.EnumerateSubqueries())
            {
                CollectConditions(sq.SelectStatement);
            }

            // Process join conditions
            conditions = new List<LogicalExpressions.Expression>();
            var from = qs.FindDescendant<FromClause>();
            if (from != null)
            {
                var tablesource = from.FindDescendant<TableSourceExpression>();
                foreach (JoinedTable jt in tablesource.EnumerateDescendantsRecursive<JoinedTable>(typeof(Subquery)))
                {
                    // CROSS JOIN queries have no search condition
                    BooleanExpression sc = jt.FindDescendant<BooleanExpression>();
                    if (sc != null)
                    {
                        conditions.Add(GetConjunctiveNormalForm(sc));
                    }
                }
            }

            // Process where clause
            WhereClause where = qs.FindDescendant<WhereClause>();
            if (where != null)
            {
                var sc = where.FindDescendant<BooleanExpression>();
                conditions.Add(GetConjunctiveNormalForm(sc));
            }
        }

        public WhereClause GenerateWhereClauseSpecificToTable(TableReference table)
        {
            BooleanExpression sc = null;

            // Loop over all conditions (JOIN ONs and WHERE conditions)
            // Result will be a combinations of the table specific conditions terms
            // of all these

            // Chain up search conditions with AND operator
            foreach (var condition in conditions)
            {
                foreach (var ex in EnumerateCnfTermsSpecificToTable(condition, table))
                {
                    var nsc = ex.GetParsingTree();

                    if (sc != null)
                    {
                        nsc.Stack.AddLast(Whitespace.Create());
                        nsc.Stack.AddLast(LogicalOperator.CreateAnd());
                        nsc.Stack.AddLast(Whitespace.Create());
                        nsc.Stack.AddLast(sc);
                    }

                    sc = nsc;
                }
            }

            // Prefix with the WHERE keyword
            if (sc != null)
            {
                return WhereClause.Create(sc);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Enumerates terms of an expression tree of a search condition.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>The expression tree must be in CNF</remarks>
        private static IEnumerable<LogicalExpressions.Expression> EnumerateCnfTerms(LogicalExpressions.Expression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node is LogicalExpressions.OperatorAnd)
            {
                // The CNF has multiple terms

                foreach (var exp in ((LogicalExpressions.OperatorAnd)node).EnumerateTerms())
                {
                    yield return exp;
                }
            }
            else if (node is LogicalExpressions.OperatorOr ||
                node is LogicalExpressions.OperatorNot ||
                node is LogicalExpressions.Predicate)
            {
                // The CNF has only one term
                yield return node;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Enumerates the terms of an expression tree that are specific to a table.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <remarks>The expression must be in CNF</remarks>
        private static IEnumerable<LogicalExpressions.Expression> EnumerateCnfTermsSpecificToTable(LogicalExpressions.Expression node, TableReference table)
        {
            foreach (var term in EnumerateCnfTerms(node))
            {
                if (term is LogicalExpressions.OperatorOr)
                {
                    // A term is only specific to a table if it contains predicates
                    // only specific to the particular table
                    var specifictotable = true;
                    foreach (var exp in EnumerateCnfTermPredicates(term))
                    {
                        if (!GetCnfLiteralPredicate(exp).IsSpecificToTable(table))
                        {
                            specifictotable = false;
                            break;
                        }
                    }

                    if (specifictotable)
                    {
                        yield return term;
                    }
                }
                else
                {
                    if (GetCnfLiteralPredicate(term).IsSpecificToTable(table))
                    {
                        yield return term;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a search condition predicate associated with a CNF literal
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private static Predicate GetCnfLiteralPredicate(LogicalExpressions.Expression term)
        {
            if (term is LogicalExpressions.OperatorNot)
            {
                return ((LogicalExpressions.Predicate)((LogicalExpressions.OperatorNot)term).Operand).Value;
            }
            else if (term is LogicalExpressions.Predicate)
            {
                return ((LogicalExpressions.Predicate)term).Value;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static IEnumerable<LogicalExpressions.Expression> EnumerateCnfTermPredicates(LogicalExpressions.Expression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node is LogicalExpressions.OperatorOr)
            {
                foreach (var exp in ((LogicalExpressions.OperatorOr)node).EnumerateTerms())
                {
                    yield return exp;
                }
            }
            else if (node is LogicalExpressions.OperatorNot ||
                node is LogicalExpressions.Predicate)
            {
                yield return node;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private LogicalExpressions.Expression GetDisjunctiveNormalForm(BooleanExpression sc)
        {
            var exp = sc.GetExpressionTree();
            var dnf = new LogicalExpressions.DnfConverter();
            return dnf.Visit(exp);
        }

        private LogicalExpressions.Expression GetConjunctiveNormalForm(BooleanExpression sc)
        {
            var exp = sc.GetExpressionTree();
            var cnf = new LogicalExpressions.CnfConverter();
            return cnf.Visit(exp);
        }
    }
}
