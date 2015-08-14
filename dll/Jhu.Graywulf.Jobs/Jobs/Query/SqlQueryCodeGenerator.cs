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
            get { return queryObject as SqlQueryPartition; }
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

        public virtual SourceTableQuery GetExecuteQuery(SelectStatement selectStatement)
        {
            var ss = new SelectStatement(selectStatement);

            RewriteQueryForExecute(ss);
            SubstituteDatabaseNames(ss, queryObject.AssignedServerInstance, Partition.Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames(ss, queryObject.TemporaryDataset, queryObject.TemporaryDataset.DefaultSchemaName);

            var source = new SourceTableQuery()
            {
                Dataset = queryObject.TemporaryDataset,
                Query = Execute(ss)
            };

            AppendPartitioningConditionParameters(source);

            return source;
        }

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

        protected void SubstituteDatabaseNames(SelectStatement selectStatement, ServerInstance serverInstance, string databaseVersion)
        {
            SubstituteDatabaseNames(selectStatement, serverInstance, databaseVersion, null);
        }

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        protected void SubstituteDatabaseNames(SelectStatement selectStatement, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            foreach (var tr in selectStatement.EnumerateSourceTableReferences(true))
            {
                if (!tr.IsUdf && !tr.IsSubquery && !tr.IsComputed)
                {
                    SubstituteDatabaseName(tr, serverInstance, databaseVersion, surrogateDatabaseVersion);
                }
            }
        }

        /// <summary>
        /// Substitutes the database name into a table reference.
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>
        /// During query executions, actual database name are not known until a server instance is
        /// assigned to the query partition.
        /// </remarks>
        public void SubstituteDatabaseName(TableReference tr, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            var sc = queryObject.GetSchemaManager();

            if (!tr.IsSubquery && !tr.IsComputed)
            {
                var ds = sc.Datasets[tr.DatasetName];

                // Graywulf datasets have changing database names depending on the server
                // the database is on.
                if (ds is GraywulfDataset)
                {
                    var gwds = ds as GraywulfDataset;
                    gwds.Context = queryObject.Context;

                    DatabaseInstance di;
                    if (gwds.IsSpecificInstanceRequired)
                    {
                        di = gwds.DatabaseInstanceReference.Value;
                    }
                    else
                    {
                        // Find appropriate database instance
                        var dis = queryObject.GetAvailableDatabaseInstances(serverInstance, gwds.DatabaseDefinitionReference.Value, databaseVersion, surrogateDatabaseVersion);
                        di = dis[0];
                    }

                    // Refresh database object, now that the correct database name is set
                    ds = di.GetDataset();
                    tr.DatabaseName = di.DatabaseName;
                    tr.DatabaseObject = ds.GetObject(tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName);
                }
            }
        }

        /// <summary>
        /// Substitutes names of remote tables with name of temporary tables
        /// holding a cached version of remote tables.
        /// </summary>
        /// <remarks></remarks>
        // TODO: This function call must be synchronized! ??
        protected virtual void SubstituteRemoteTableNames(SelectStatement selectStatement, DatasetBase temporaryDataset, string temporarySchemaName)
        {
            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // No remote table support
                    throw new InvalidOperationException();
                case ExecutionMode.Graywulf:
                    var sm = queryObject.GetSchemaManager();

                    // Replace remote table references with temp table references
                    foreach (TableReference tr in selectStatement.EnumerateSourceTableReferences(true))
                    {
                        SubstituteRemoteTableName(sm, tr, temporaryDataset, temporarySchemaName);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Substitutes the name of a remote tables with name of the temporary table
        /// holding a cached version of the remote data.
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="tr"></param>
        /// <param name="temporaryDataset"></param>
        /// <param name="temporarySchemaName"></param>
        private void SubstituteRemoteTableName(SchemaManager sm, TableReference tr, DatasetBase temporaryDataset, string temporarySchemaName)
        {
            // Save unique name because it will change as names are substituted
            var un = tr.UniqueName;

            // TODO: write function to determine if a table is to be copied
            // ie. the condition in the if clause of the following line
            if (tr.IsCachable && queryObject.TemporaryTables.ContainsKey(tr.UniqueName) &&
                queryObject.IsRemoteDataset(sm.Datasets[tr.DatasetName]))
            {
                tr.DatabaseName = temporaryDataset.DatabaseName;
                tr.SchemaName = temporarySchemaName;
                tr.DatabaseObjectName = queryObject.TemporaryTables[un].TableName;
                tr.DatabaseObject = null;
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
