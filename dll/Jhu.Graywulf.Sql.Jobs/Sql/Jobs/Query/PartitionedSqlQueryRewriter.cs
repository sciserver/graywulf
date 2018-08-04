using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Extensions.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.Extensions.QueryRewriting;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    /// <summary>
    /// Implements query rewrite logic to transform query into an executable
    /// version.
    /// </summary>
    public class PartitionedSqlQueryRewriter : Jhu.Graywulf.Sql.Extensions.QueryRewriting.GraywulfSqlQueryRewriter
    {
        #region Private member variables

        protected QueryObject queryObject;

        #endregion
        #region Properties

        private SqlQueryPartition Partition
        {
            get { return queryObject as SqlQueryPartition; }
        }

        public new PartitionedSqlQueryRewriterOptions Options
        {
            get { return (PartitionedSqlQueryRewriterOptions)base.Options; }
            set { base.Options = value; }
        }

        #endregion
        #region Constructors and initializers

        public PartitionedSqlQueryRewriter(QueryObject queryObject)
        {
            InitializeMembers();

            this.queryObject = queryObject;
        }

        private void InitializeMembers()
        {
            this.queryObject = null;
        }

        protected override GraywulfSqlQueryRewriterOptions CreateOptions()
        {
            return new PartitionedSqlQueryRewriterOptions();
        }

        #endregion
        #region Visitor entry points

        protected virtual void Accept(PartitionedQuerySpecification qs)
        {

            if (Visitor.Pass == 1)
            {
                if (Options.AppendPartitioning && Visitor.QuerySpecificationDepth == 0)
                {
                    // Check if it is a partitioned query and append partitioning conditions, if necessary
                    var ts = qs.FirstTableSource as PartitionedTableSource;

                    if (ts != null)
                    {
                        if (Visitor.Index > 0)
                        {
                            throw new InvalidOperationException();
                        }

                        AppendPartitioningConditions(qs, ts);
                    }
                }

                if (Options.RemovePartitioning)
                {
                    var ts = qs.FirstTableSource as PartitionedTableSource;

                    if (ts != null)
                    {
                        // Strip off PARTITION BY clause
                        var pc = ts.FindDescendant<TablePartitionClause>();

                        if (pc != null)
                        {
                            pc.Parent.Stack.Remove(pc);
                        }
                    }
                }
            }
        }



        #endregion
        #region Query partitioning

        protected virtual void AppendPartitioningConditions(QuerySpecification qs, PartitionedTableSource ts)
        {
            var sc = GetPartitioningConditions(ts.PartitioningKeyExpression);
            if (sc != null)
            {
                qs.AppendSearchCondition(sc, "AND");
            }
        }

        protected LogicalExpression GetPartitioningConditions(Expression partitioningKeyExpression)
        {
            if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMin) &&
                !Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                var from = GetPartitioningKeyMinCondition(partitioningKeyExpression);
                var to = GetPartitioningKeyMaxCondition(partitioningKeyExpression);
                return LogicalExpression.Create(from, to, LogicalOperator.CreateAnd());
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
        private LogicalExpression GetPartitioningKeyMinCondition(Expression partitioningKeyExpression)
        {
            var a = Expression.Create(UserVariable.Create(Constants.PartitionKeyMinParameterName));
            var p = Predicate.CreateLessThanOrEqual(a, partitioningKeyExpression);
            return LogicalExpression.Create(false, p);
        }

        /// <summary>
        /// Generates a parsing tree segment for the the partitioning key upper limit
        /// </summary>
        /// <param name="partitioningKey"></param>
        /// <returns></returns>
        private LogicalExpression GetPartitioningKeyMaxCondition(Expression partitioningKeyExpression)
        {
            var b = Expression.Create(UserVariable.Create(Constants.PartitionKeyMaxParameterName));
            var p = Predicate.CreateLessThan(partitioningKeyExpression, b);
            return LogicalExpression.Create(false, p);
        }

        #endregion
    }
}
