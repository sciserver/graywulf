using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryCodeGenerator : SqlServerCodeGenerator
    {
        protected const string keyFromParameterName = "@keyFrom";
        protected const string keyToParameterName = "@keyTo";

        protected QueryObject queryObject;

        private SqlQueryPartition Partition
        {
            get { return (SqlQueryPartition)queryObject; }
        }

        // TODO: used only by tests, delete
        public SqlQueryCodeGenerator()
        {
        }
               
        public SqlQueryCodeGenerator(QueryObject queryObject)
        {
            this.queryObject = queryObject;
            this.ResolveNames = true;
        }

        #region Basic query rewrite functions

        public void RewriteQueryForExecute(SelectStatement selectStatement)
        {
            var qs = selectStatement.EnumerateQuerySpecifications().First<QuerySpecification>();

            // Check if it is a partitioned query and append partitioning conditions, if necessary
            var ts = qs.EnumerateSourceTables(false).FirstOrDefault();
            if (ts != null && ts is SimpleTableSource && ((SimpleTableSource)ts).IsPartitioned)
            {
                AppendPartitioningConditions(qs, (SimpleTableSource)ts);
            }

            RemoveExtraTokens(selectStatement);
        }

        public virtual void RemoveExtraTokens(SelectStatement selectStatement)
        {
            // strip off order by, we write to the mydb
            var orderby = selectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                selectStatement.Stack.Remove(orderby);
            }

            // strip off partition by and into clauses
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

        #endregion
        #region Table statistics

        /// <summary>
        /// Returns a command initialized for computing table statistics
        /// </summary>
        /// <param name="tableSource"></param>
        /// <returns></returns>
        public virtual SqlCommand GetTableStatisticsCommand(ITableSource tableSource)
        {
            if (tableSource.TableReference.Statistics == null)
            {
                throw new ArgumentNullException();
            }

            if (!(tableSource.TableReference.DatabaseObject is TableOrView))
            {
                throw new ArgumentException();
            }

            var table = (TableOrView)tableSource.TableReference.DatabaseObject;
            var keycol = tableSource.TableReference.Statistics.KeyColumn;
            var keytype = tableSource.TableReference.Statistics.KeyColumnDataType.NameWithLength;
            var temptable = queryObject.GetTemporaryTable(GetEscapedUniqueName(tableSource.TableReference));
            var where = GetTableStatisticsWhereClause(tableSource);

            var sql = new StringBuilder(SqlQueryScripts.TableStatistics);

            sql.Replace("[$temptable]", GetResolvedTableName(temptable));
            sql.Replace("[$keytype]", keytype);
            sql.Replace("[$keycol]", keycol);
            sql.Replace("[$tablename]", GetResolvedTableNameWithAlias(tableSource.TableReference));
            sql.Replace("[$where]", where != null ? where.ToString() : "");

            return new SqlCommand(sql.ToString());
        }

        protected virtual WhereClause GetTableStatisticsWhereClause(ITableSource tableSource)
        {
            var tr = tableSource.TableReference;
            
            var cnr = new SearchConditionNormalizer();
            cnr.CollectConditions(((TableSource)tr.Node).QuerySpecification);
            var where = cnr.GenerateWhereClauseSpecificToTable(tr);

            return where;
        }

        #endregion
        #region Query partitioning

        protected virtual void AppendPartitioningConditions(QuerySpecification qs, SimpleTableSource ts)
        {
            var sc = GetPartitioningConditions(ts.PartitioningColumnReference);
            if (sc != null)
            {
                qs.AppendSearchCondition(sc, "AND");
            }
        }

        public void AppendPartitioningConditionParameters(SqlCommand cmd)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = keyFromParameterName;
                par.Value = Partition.PartitioningKeyFrom;
                cmd.Parameters.Add(par);
            }

            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = keyToParameterName;
                par.Value = Partition.PartitioningKeyTo;
                cmd.Parameters.Add(par);
            }
        }

        public void AppendPartitioningConditionParameters(SourceTableQuery q)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom))
            {
                q.Parameters.Add(keyFromParameterName, Partition.PartitioningKeyFrom);
            }

            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
            {
                q.Parameters.Add(keyToParameterName, Partition.PartitioningKeyTo);
            }
        }

        protected virtual bool IsPartitioningKeyUnbound(object key)
        {
            return key == null;
        }

        protected SearchCondition GetPartitioningConditions(ColumnReference partitioningKey)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom) && !IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
            {
                var from = GetPartitioningKeyFromCondition(partitioningKey);
                var to = GetPartitioningKeyToCondition(partitioningKey);
                return SearchCondition.Create(from, to, LogicalOperator.CreateAnd());
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom))
            {
                return GetPartitioningKeyFromCondition(partitioningKey);
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
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
            var a = Expression.Create(SqlParser.Variable.Create(keyFromParameterName));
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
            var b = Expression.Create(SqlParser.Variable.Create(keyToParameterName));
            var p = Predicate.CreateLessThan(a, b);
            return SearchCondition.Create(false, p);
        }

        #endregion
    }
}
