using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class SearchConditionNormalizer
    {
        private List<LogicalExpressions.ExpressionTreeNode> conditions;
        
        public SearchConditionNormalizer()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            conditions = new List<LogicalExpressions.ExpressionTreeNode>();
        }

        public void CollectConditions(Parsing.StatementBlock script)
        {
            foreach (var qs in script.EnumerateDescendantsRecursive<Parsing.QuerySpecification>())
            {
                CollectConditions(qs);
            }
        }

        public void CollectConditions(Parsing.QueryExpression qe)
        {
            foreach (var qs in qe.EnumerateDescendantsRecursive<Parsing.QuerySpecification>())
            {
                CollectConditions(qs);
            }
        }

        public void CollectConditions(Parsing.QuerySpecification qs)
        {
            // TODO: review processing of subqueries here
            // TODO: what to do with conditions that involve other tables (subqueries)
            //       probably those are filtered out in GenerateWhereClauseSpecificToTable?
            //       subqueries may reference tables outside the subquery!

            // Process join conditions
            var from = qs.FindDescendant<Parsing.FromClause>();

            if (from != null)
            {
                var tablesource = from.FindDescendant<Parsing.TableSourceExpression>();
                foreach (Parsing.JoinedTable jt in tablesource.EnumerateDescendantsRecursive<Parsing.JoinedTable>(typeof(Parsing.Subquery)))
                {
                    // CROSS JOIN queries have no search condition
                    var sc = jt.FindDescendant<Parsing.LogicalExpression>();
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
                var sc = where.FindDescendant<Parsing.LogicalExpression>();
                conditions.Add(GetConjunctiveNormalForm(sc));
            }
        }

        public Parsing.LogicalExpression GenerateWherePredicatesSpecificToTable(IList<TableReference> trs)
        {
            Parsing.LogicalExpression sc = null;

            // Chain up search conditions with the AND operator
            foreach (var tr in trs)
            {
                var nsc = GenerateWherePredicatesSpecificToTable(tr);

                // No predicate which means we need the entire table
                if (nsc == null)
                {
                    return null;
                }

                if (sc != null)
                {
                    //

                    nsc.Stack.AddLast(Parsing.Whitespace.Create());
                    nsc.Stack.AddLast(Parsing.LogicalOperator.CreateOr());
                    nsc.Stack.AddLast(Parsing.Whitespace.Create());
                    nsc.Stack.AddLast(sc);
                }

                sc = nsc;
            }

            return sc;
        }

        public Parsing.LogicalExpression GenerateWherePredicatesSpecificToTable(TableReference tr)
        {
            Parsing.LogicalExpression sc = null;

            // Loop over all conditions (JOIN ONs and WHERE conditions)
            // Result will be a combinations of the table specific conditions terms
            // of all these

            // Chain up search conditions with AND operator
            foreach (var condition in conditions)
            {
                foreach (var ex in EnumerateCnfTermsSpecificToTable(condition, tr, null))
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

            if (sc != null)
            {
                sc = Parsing.LogicalExpression.Create(false, Parsing.LogicalExpressionBrackets.Create(sc));
            }

            return sc;
        }

        /// <summary>
        /// Enumerates terms of an expression tree of a search condition.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>The expression tree must be in CNF</remarks>
        private static IEnumerable<LogicalExpressions.ExpressionTreeNode> EnumerateCnfTerms(LogicalExpressions.ExpressionTreeNode node)
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
        private static IEnumerable<ExpressionTreeNode> EnumerateCnfTermsSpecificToTable(LogicalExpressions.ExpressionTreeNode node, TableReference tr, DatabaseObject dbobj)
        {
            var predicateVisitor = new PredicateVisitor();
            bool specifictotable;

            foreach (var term in EnumerateCnfTerms(node))
            {
                if (term is OperatorOr)
                {
                    // A term is only specific to a table if it contains predicates
                    // only specific to the particular table
                    specifictotable = true;

                    foreach (var exp in EnumerateCnfTermPredicates(term))
                    {
                        var predicate = GetCnfLiteralPredicate(exp);

                        if (tr != null)
                        {
                            specifictotable &= predicateVisitor.IsSpecificToTable(predicate, tr);
                        }
                        else
                        {
                            specifictotable &= predicateVisitor.IsSpecificToTable(predicate, dbobj);
                        }

                        if (!specifictotable)
                        {
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
                    var predicate = GetCnfLiteralPredicate(term);

                    if (tr != null)
                    {
                        specifictotable = predicateVisitor.IsSpecificToTable(predicate,tr);
                    }
                    else
                    {
                        specifictotable = predicateVisitor.IsSpecificToTable(predicate, dbobj);
                    }

                    if (specifictotable)
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
        private static Parsing.Predicate GetCnfLiteralPredicate(LogicalExpressions.ExpressionTreeNode term)
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

        private static IEnumerable<LogicalExpressions.ExpressionTreeNode> EnumerateCnfTermPredicates(LogicalExpressions.ExpressionTreeNode node)
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

        private LogicalExpressions.ExpressionTreeNode GetDisjunctiveNormalForm(Parsing.LogicalExpression sc)
        {
            var exp = new ExpressionTreeBuilder().Execute(sc);
            var dnf = new DnfConverter();
            return dnf.Visit(exp);
        }

        private LogicalExpressions.ExpressionTreeNode GetConjunctiveNormalForm(Parsing.LogicalExpression sc)
        {
            var exp = new ExpressionTreeBuilder().Execute(sc);
            var cnf = new CnfConverter();
            return cnf.Visit(exp);
        }
    }
}
