using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryCodeGenerator : SqlServerCodeGenerator
    {
        protected const string keyFromParameterName = "@keyFrom";
        protected const string keyToParameterName = "@keyTo";

        public void RewriteQueryForExecute(SelectStatement selectStatement, object partitioningKeyFrom, object partitioningKeyTo)
        {
            var qs = selectStatement.EnumerateQuerySpecifications().First<QuerySpecification>();

            // Check if it is a partitioned query and append partitioning conditions, if necessary
            var ts = qs.EnumerateSourceTables(false).FirstOrDefault();
            if (ts != null && ts is SimpleTableSource && ((SimpleTableSource)ts).IsPartitioned)
            {
                AppendPartitioningConditions(qs, (SimpleTableSource)ts, partitioningKeyFrom, partitioningKeyTo);
            }

            RemoveExtraTokens(selectStatement);
        }

        internal void RemoveExtraTokens(SelectStatement selectStatement)
        {
            // strip off order by, we write to the mydb
            var orderby = selectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                selectStatement.Stack.Remove(orderby);
            }

            // strip off partition by
            foreach (var qs in selectStatement.EnumerateQuerySpecifications())
            {
                // strip off select into
                var into = qs.FindDescendant<IntoClause>();
                if (into != null)
                {
                    qs.Stack.Remove(into);
                }

                foreach (var ts in qs.EnumerateDescendantsRecursive<SimpleTableSource>())
                {
                    var pc = ts.FindDescendant<TablePartitionClause>();

                    if (pc != null)
                    {
                        pc.Parent.Stack.Remove(pc);
                    }
                }
            }
        }

        #region Query partitioning

        protected virtual void AppendPartitioningConditions(QuerySpecification qs, SimpleTableSource ts, object partitioningKeyFrom, object partitioningKeyTo)
        {
            var sc = GetPartitioningConditions(ts.PartitioningColumnReference, partitioningKeyFrom, partitioningKeyTo);
            if (sc != null)
            {
                qs.AppendSearchCondition(sc, "AND");
            }
        }

        public void AppendPartitioningConditionParameters(SqlCommand cmd, object partitioningKeyFrom, object partitioningKeyTo)
        {
            if (!IsPartitioningKeyUnbound(partitioningKeyFrom))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = keyFromParameterName;
                par.Value = partitioningKeyFrom;
                cmd.Parameters.Add(par);
            }

            if (!IsPartitioningKeyUnbound(partitioningKeyTo))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = keyToParameterName;
                par.Value = partitioningKeyTo;
                cmd.Parameters.Add(par);
            }
        }

        public void AppendPartitioningConditionParameters(SourceTableQuery q, object partitioningKeyFrom, object partitioningKeyTo)
        {
            if (!IsPartitioningKeyUnbound(partitioningKeyFrom))
            {
                q.Parameters.Add(keyFromParameterName, partitioningKeyFrom);
            }

            if (!IsPartitioningKeyUnbound(partitioningKeyTo))
            {
                q.Parameters.Add(keyToParameterName, partitioningKeyTo);
            }
        }

        protected virtual bool IsPartitioningKeyUnbound(object key)
        {
            return key == null;
        }

        protected SearchCondition GetPartitioningConditions(ColumnReference partitioningKey, object partitioningKeyFrom, object partitioningKeyTo)
        {
            if (!IsPartitioningKeyUnbound(partitioningKeyFrom) && !IsPartitioningKeyUnbound(partitioningKeyTo))
            {
                var from = GetPartitioningKeyFromCondition(partitioningKey);
                var to = GetPartitioningKeyToCondition(partitioningKey);
                return SearchCondition.Create(from, to, LogicalOperator.CreateAnd());
            }
            else if (!IsPartitioningKeyUnbound(partitioningKeyFrom))
            {
                return GetPartitioningKeyFromCondition(partitioningKey);
            }
            else if (!IsPartitioningKeyUnbound(partitioningKeyTo))
            {
                return GetPartitioningKeyToCondition(partitioningKey);
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
        private SearchCondition GetPartitioningKeyFromCondition(ColumnReference partitioningKey)
        {
            var a = Expression.Create(Variable.Create(keyFromParameterName));
            var b = Expression.Create(ColumnIdentifier.Create(partitioningKey));
            var p = Predicate.CreateLessThanOrEqual(a, b);
            return SearchCondition.Create(false, p);
        }

        /// <summary>
        /// Generates a parsing tree segment for the the partitioning key upper limit
        /// </summary>
        /// <param name="partitioningKey"></param>
        /// <returns></returns>
        private SearchCondition GetPartitioningKeyToCondition(ColumnReference partitioningKey)
        {
            var a = Expression.Create(ColumnIdentifier.Create(partitioningKey));
            var b = Expression.Create(Variable.Create(keyToParameterName));
            var p = Predicate.CreateLessThan(a, b);
            return SearchCondition.Create(false, p);
        }

        #endregion
    }
}
