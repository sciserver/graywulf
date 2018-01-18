using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.LogicalExpressions
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
            conditions = new List<LogicalExpressions.Expression>();
        }
        
        public void CollectConditions(Parsing.QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendants<Parsing.QuerySpecification>())
            {
                CollectConditions(qs);
            }
        }

        public void CollectConditions(Parsing.QuerySpecification qs)
        {
            // TODO: review processing of subqueries here
            // TODO: what to do with conditions that involve other tables (subqueries)
            //       probably those are filtered out in GenerateWhereClauseSpecificToTable?

            /* TODO: delete
            foreach (var sq in qs.EnumerateSubqueries())
            {
                CollectConditions(sq.QueryExpression);
            }*/

            // Process join conditions
            var from = qs.FindDescendant<Parsing.FromClause>();

            if (from != null)
            {
                var tablesource = from.FindDescendant<Parsing.TableSourceExpression>();
                foreach (Parsing.JoinedTable jt in tablesource.EnumerateDescendantsRecursive<Parsing.JoinedTable>(typeof(Parsing.Subquery)))
                {
                    // CROSS JOIN queries have no search condition
                    var sc = jt.FindDescendant<Parsing.BooleanExpression>();
                    if (sc != null)
                    {
                        conditions.Add(GetConjunctiveNormalForm(sc));
                    }
                }
            }

            // Process where clause
            var where = qs.FindDescendant<Parsing.WhereClause>();

            if (where != null)
            {
                var sc = where.FindDescendant<Parsing.BooleanExpression>();
                conditions.Add(GetConjunctiveNormalForm(sc));
            }
        }

        public Parsing.WhereClause GenerateWhereClauseSpecificToTable(TableReference table)
        {
            Parsing.BooleanExpression sc = null;

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
                        nsc.Stack.AddLast(Parsing.Whitespace.Create());
                        nsc.Stack.AddLast(Parsing.LogicalOperator.CreateAnd());
                        nsc.Stack.AddLast(Parsing.Whitespace.Create());
                        nsc.Stack.AddLast(sc);
                    }

                    sc = nsc;
                }
            }

            // Prefix with the WHERE keyword
            if (sc != null)
            {
                return Parsing.WhereClause.Create(sc);
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
        private static Parsing.Predicate GetCnfLiteralPredicate(LogicalExpressions.Expression term)
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

        private LogicalExpressions.Expression GetDisjunctiveNormalForm(Parsing.BooleanExpression sc)
        {
            var exp = sc.GetExpressionTree();
            var dnf = new LogicalExpressions.DnfConverter();
            return dnf.Visit(exp);
        }

        private LogicalExpressions.Expression GetConjunctiveNormalForm(Parsing.BooleanExpression sc)
        {
            var exp = sc.GetExpressionTree();
            var cnf = new LogicalExpressions.CnfConverter();
            return cnf.Visit(exp);
        }
    }
}
