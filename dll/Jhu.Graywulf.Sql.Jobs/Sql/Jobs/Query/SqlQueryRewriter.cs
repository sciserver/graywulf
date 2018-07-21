using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRewriting;
using Jhu.Graywulf.Sql.QueryRewriting.SqlServer;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    /// <summary>
    /// Implements query rewrite logic to transform query into an executable
    /// version.
    /// </summary>
    public class SqlQueryRewriter : SqlServerQueryRewriter
    {
        #region Private member variables

        protected QueryObject queryObject;
        private SqlQueryRewriterOptions options;

        #endregion
        #region Properties

        private SqlQueryPartition Partition
        {
            get { return queryObject as SqlQueryPartition; }
        }

        public SqlQueryRewriterOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryRewriter(QueryObject queryObject)
        {
            InitializeMembers();

            this.queryObject = queryObject;
        }

        private void InitializeMembers()
        {
            this.queryObject = null;
            this.options = new SqlQueryRewriterOptions();
        }

        #endregion

        protected override void RewriteQuerySpecification(QuerySpecification qs, int depth, int index, QueryContext queryContext)
        {
            base.RewriteQuerySpecification(qs, depth, index, queryContext);

            // TODO: get rid of this logic, partitioning will be solved differently

            if (options.AppendPartitioning && depth == 0)
            {
                // Check if it is a partitioned query and append partitioning conditions, if necessary
                var ts = qs.EnumerateSourceTables(false).FirstOrDefault();

                if (ts != null && ts is SimpleTableSource && ((SimpleTableSource)ts).IsPartitioned)
                {
                    if (index > 0)
                    {
                        throw new InvalidOperationException();
                    }

                    AppendPartitioningConditions(qs, (SimpleTableSource)ts);
                }
            }

            if (options.RemovePartitioning)
            {
                foreach (var ts in qs.EnumerateSourceTables(false))
                {
                    // Strip off PARTITION BY clause
                    var pc = (ts as Node)?.FindDescendant<TablePartitionClause>();

                    if (pc != null)
                    {
                        pc.Parent.Stack.Remove(pc);
                    }
                }
            }
        }

        #region Table source rewrite



        #endregion
        #region Select list rewrite

        protected override void RewriteSelectList(QuerySpecification qs, SelectList selectList, int depth, int index, QueryContext queryContext)
        {
            base.RewriteSelectList(qs, selectList, depth, index, queryContext);

            if (options.SubstituteStars)
            {
                var nsl = SubstituteStars(qs, selectList, depth);
                selectList.ReplaceWith(nsl);
                selectList = nsl;
            }

            if (options.AssignColumnAliases)
            {
                AssignDefaultColumnAliases(qs, selectList, depth, depth != 0, (queryContext & QueryContext.SemiJoin) != 0);
            }
        }

        public SelectList SubstituteStars(QuerySpecification qs, SelectList selectList, int depth)
        {
            var ce = selectList.FindDescendant<ColumnExpression>();
            var subsl = selectList.FindDescendant<SelectList>();

            SelectList sl = null;

            if (ce.ColumnReference.IsStar)
            {
                // Build select list from the column list of
                // the referenced table, then replace current node
                if (ce.TableReference.IsUndefined)
                {
                    sl = SelectList.Create(qs);
                }
                else
                {
                    sl = SelectList.Create(ce.TableReference);
                }

                if (subsl != null)
                {
                    sl.Append(SubstituteStars(qs, subsl, depth));
                }

                return sl;
            }
            else
            {
                if (subsl != null)
                {
                    selectList.Stack.Replace<SelectList>(SubstituteStars(qs, subsl, depth));
                    // TODO: delete selectList.Replace(SubstituteStars(qs, subsl, depth));
                }

                return selectList;
            }
        }
        
        /// <summary>
        /// Adds default aliases to columns with no aliases specified in the query
        /// </summary>
        /// <param name="qs"></param>
        private void AssignDefaultColumnAliases(QuerySpecification qs, SelectList selectList, int depth, bool subquery, bool singleColumnSubquery)
        {
            var aliases = new HashSet<string>(SchemaManager.Comparer);
            var cnt = selectList.EnumerateColumnExpressions().Count();

            int q = 0;
            foreach (var ce in selectList.EnumerateColumnExpressions())
            {
                var cr = ce.ColumnReference;
                string alias;

                if (singleColumnSubquery && q > 0)
                {
                    throw NameResolutionError.SingleColumnSubqueryRequired(ce);
                }

                if (cr.ColumnAlias == null)
                {
                    if (cr.ColumnName == null)
                    {
                        if (subquery && !singleColumnSubquery && cnt > 1)
                        {
                            throw NameResolutionError.MissingColumnAlias(ce);
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("Col_{0}", cr.SelectListIndex));
                        }
                    }
                    else if (!subquery)
                    {
                        if (cr.TableReference != null && cr.TableReference.Alias != null)
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("{0}_{1}", cr.TableReference.Alias, cr.ColumnName));
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, cr.ColumnName);
                        }
                    }
                    else
                    {
                        alias = null;
                    }
                }
                else
                {
                    // Alias is set explicitly, so do not make it unique forcibly
                    alias = cr.ColumnAlias;
                }

                if (alias != null)
                {
                    if (aliases.Contains(alias))
                    {
                        throw NameResolutionError.DuplicateColumnAlias(alias, ce);
                    }

                    aliases.Add(alias);
                    cr.ColumnAlias = alias;
                }

                q++;
            }
        }

        private string GetUniqueColumnAlias(HashSet<string> aliases, string alias)
        {
            int q = 0;
            var alias2 = alias;
            while (aliases.Contains(alias2))
            {
                alias2 = String.Format("{0}_{1}", alias, q);
                q++;
            }

            return alias2;
        }

        #endregion

        protected override void RewriteIntoClause(SelectStatement selectStatement, IntoClause into)
        {
            base.RewriteIntoClause(selectStatement, into);

            // Create a magic statement and insert before the SELECT
            var parent = selectStatement.FindAscendant<StatementBlock>();
            var magic = new ServerMessageMagicToken()
            {
                DestinationTable = into.TargetTable.TableOrViewIdentifier.TableReference
            };

            var sb = StatementBlock.Create(magic, selectStatement);
            var be = BeginEndStatement.Create(sb);
            selectStatement.ReplaceWith(be);

            into.Remove();
        }


        protected override void RewriteOrderByClause(SelectStatement selectStatement, OrderByClause orderby)
        {
            base.RewriteOrderByClause(selectStatement, orderby);

            // TODO: Right now keep order by intact, it might be necessary for the TOP expression
            // to work correctly. Later the order by clause could be rewritten to match primary key
            // ordering.

            selectStatement.Stack.Remove(orderby);
        }

        #region Query partitioning

        protected virtual void AppendPartitioningConditions(QuerySpecification qs, SimpleTableSource ts)
        {
            var sc = GetPartitioningConditions(ts.PartitioningKeyExpression);
            if (sc != null)
            {
                qs.AppendSearchCondition(sc, "AND");
            }
        }

        protected BooleanExpression GetPartitioningConditions(Expression partitioningKeyExpression)
        {
            if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMin) &&
                !Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                var from = GetPartitioningKeyMinCondition(partitioningKeyExpression);
                var to = GetPartitioningKeyMaxCondition(partitioningKeyExpression);
                return BooleanExpression.Create(from, to, LogicalOperator.CreateAnd());
            }
            else if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMin))
            {
                return GetPartitioningKeyMinCondition(partitioningKeyExpression);
            }
            else if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                return GetPartitioningKeyMaxCondition(partitioningKeyExpression);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a parsing tree segment for the the partitioning key lower limit
        /// </summary>
        /// <param name="partitioningKey"></param>
        /// <returns></returns>
        private BooleanExpression GetPartitioningKeyMinCondition(Expression partitioningKeyExpression)
        {
            var a = Expression.Create(UserVariable.Create(Constants.PartitionKeyMinParameterName));
            var p = Predicate.CreateLessThanOrEqual(a, partitioningKeyExpression);
            return BooleanExpression.Create(false, p);
        }

        /// <summary>
        /// Generates a parsing tree segment for the the partitioning key upper limit
        /// </summary>
        /// <param name="partitioningKey"></param>
        /// <returns></returns>
        private BooleanExpression GetPartitioningKeyMaxCondition(Expression partitioningKeyExpression)
        {
            var b = Expression.Create(UserVariable.Create(Constants.PartitionKeyMaxParameterName));
            var p = Predicate.CreateLessThan(partitioningKeyExpression, b);
            return BooleanExpression.Create(false, p);
        }

        #endregion        
    }
}
