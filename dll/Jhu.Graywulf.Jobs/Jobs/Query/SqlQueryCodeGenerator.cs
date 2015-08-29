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
        #region Constants

        protected const string keyFromParameterName = "@keyFrom";
        protected const string keyToParameterName = "@keyTo";

        #endregion
        #region Private member variables

        protected QueryObject queryObject;
        private SqlServerDataset codeDataset;
        private SqlServerDataset tempDataset;

        #endregion
        #region Properties

        public SqlServerDataset CodeDataset
        {
            get { return codeDataset; }
            set { codeDataset = value; }
        }

        public SqlServerDataset TempDataset
        {
            get { return tempDataset; }
            set { tempDataset = value; }
        }

        private SqlQueryPartition Partition
        {
            get { return queryObject as SqlQueryPartition; }
        }

        #endregion
        #region Constructors and initializers

        // TODO: used only by tests, delete
        public SqlQueryCodeGenerator()
        {
        }

        public SqlQueryCodeGenerator(QueryObject queryObject)
        {
            this.queryObject = queryObject;
            this.codeDataset = queryObject.CodeDataset;
            this.tempDataset = queryObject.TemporaryDataset;

            this.ResolveNames = true;
        }

        #endregion
        #region Basic query rewrite functions

        public SourceTableQuery GetExecuteQuery(SelectStatement selectStatement)
        {
            return GetExecuteQuery(selectStatement, CommandMethod.Select, null);
        }

        public SourceTableQuery GetExecuteQuery(SelectStatement selectStatement, CommandMethod method, Table destination)
        {
            var ss = (SelectStatement)selectStatement.Clone();
            return OnGetExecuteQuery(ss, method, destination);
        }

        protected virtual SourceTableQuery OnGetExecuteQuery(SelectStatement selectStatement, CommandMethod method, Table destination)
        {
            var sql = new StringBuilder();

            RewriteForExecute(selectStatement);
            RemoveNonStandardTokens(selectStatement, method);

            if (queryObject.ExecutionMode == ExecutionMode.Graywulf)
            {
                SubstituteServerSpecificDatabaseNames(selectStatement, queryObject.AssignedServerInstance, Partition.Query.SourceDatabaseVersionName);
                SubstituteRemoteTableNames(selectStatement);
            }

            AppendQuery(sql, selectStatement, method, destination);

            var source = new SourceTableQuery()
            {
                Dataset = queryObject.TemporaryDataset,
                Query = sql.ToString(),
            };

            AppendPartitioningConditionParameters(source);

            return source;
        }

        protected void AppendQuery(StringBuilder sql, SelectStatement ss, CommandMethod method, Table destination)
        {
            // TODO: this is a temporary trick until full SQL grammar is implemented
            switch (method)
            {
                case CommandMethod.Select:
                    sql.AppendLine(Execute(ss));
                    break;
                case CommandMethod.SelectInto:
                    sql.Append("SELECT __tablealias.* INTO ");
                    sql.Append(GetResolvedTableName(destination));
                    sql.AppendLine(" FROM (");
                    sql.AppendLine(Execute(ss));
                    sql.AppendLine(") AS __tablealias");
                    break;
                case CommandMethod.Insert:
                    sql.Append("INSERT ");
                    sql.Append(GetResolvedTableName(destination));
                    sql.AppendLine("WITH (TABLOCKX) ");
                    sql.AppendLine("SELECT __tablealias.*");
                    sql.AppendLine("FROM (");
                    sql.AppendLine(Execute(ss));
                    sql.AppendLine(") AS __tablealias");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void RewriteForExecute(SelectStatement selectStatement)
        {
            int i = 0;
            foreach (var qs in selectStatement.EnumerateQuerySpecifications())
            {
                RewriteForExecute(qs, i);
                i++;
            }

            RewriteForExecute(selectStatement.OrderByClause);
        }

        protected virtual void RewriteForExecute(QuerySpecification qs, int i)
        {
            // Check if it is a partitioned query and append partitioning conditions, if necessary
            var ts = qs.EnumerateSourceTables(false).FirstOrDefault();

            if (ts != null && ts is SimpleTableSource && ((SimpleTableSource)ts).IsPartitioned)
            {
                if (i > 0)
                {
                    throw new InvalidOperationException();
                }

                AppendPartitioningConditions(qs, (SimpleTableSource)ts);
            }
        }

        protected virtual void RewriteForExecute(OrderByClause orderBy)
        {
            // TODO: we remove this for now but later can be implemented
        }

        #endregion
        #region Remove non-standard tokens

        protected virtual void RemoveNonStandardTokens(SelectStatement selectStatement, CommandMethod method)
        {
            // strip off partition by and into clauses
            var qe = selectStatement.FindDescendant<QueryExpression>();

            if (qe != null)
            {
                RemoveNonStandardTokens(qe);
            }

            // Strip off order by, we write to the mydb
            if (method == CommandMethod.Insert || method == CommandMethod.SelectInto)
            {
                var orderby = selectStatement.FindDescendant<OrderByClause>();

                if (orderby != null)
                {
                    selectStatement.Stack.Remove(orderby);
                }
            }
        }

        protected virtual void RemoveNonStandardTokens(QueryExpression qe)
        {
            // QueryExpressionBrackets
            var qeb = qe.FindDescendant<QueryExpressionBrackets>();

            if (qeb != null)
            {
                var qee = qeb.FindDescendant<QueryExpression>();
                RemoveNonStandardTokens(qee);
            }

            // QueryExpression
            var qer = qe.FindDescendant<QueryExpression>();

            if (qer != null)
            {
                RemoveNonStandardTokens(qer);
            }

            // QuerySpecification
            var qs = qe.FindDescendant<QuerySpecification>();

            if (qs != null)
            {
                RemoveNonStandardTokens(qs);
            }
        }

        protected virtual void RemoveNonStandardTokens(QuerySpecification qs)
        {
            // strip off select into
            var into = qs.FindDescendant<IntoClause>();
            if (into != null)
            {
                qs.Stack.Remove(into);
            }

            // strip off partition by
            foreach (var ts in qs.EnumerateDescendantsRecursive<SimpleTableSource>())
            {
                var pc = ts.FindDescendant<TablePartitionClause>();

                if (pc != null)
                {
                    pc.Parent.Stack.Remove(pc);
                }
            }
        }

        #endregion
        #region Name substitution

        protected void SubstituteServerSpecificDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion)
        {
            SubstituteServerSpecificDatabaseNames(ss, serverInstance, databaseVersion, null);
        }

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        protected void SubstituteServerSpecificDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    throw new InvalidOperationException();
                case ExecutionMode.Graywulf:
                    foreach (var qs in ss.EnumerateQuerySpecifications())
                    {
                        foreach (var tr in qs.EnumerateSourceTableReferences(true))
                        {
                            if (!tr.IsUdf && !tr.IsSubquery && !tr.IsComputed)
                            {
                                SubstituteServerSpecificDatabaseName(tr, serverInstance, databaseVersion, surrogateDatabaseVersion);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
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
        public void SubstituteServerSpecificDatabaseName(TableReference tr, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
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
        protected virtual void SubstituteRemoteTableNames(SelectStatement ss)
        {
            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // No remote table support
                    throw new InvalidOperationException();
                case ExecutionMode.Graywulf:
                    foreach (var qs in ss.EnumerateQuerySpecifications())
                    {
                        // Replace remote table references with temp table references
                        foreach (TableReference tr in qs.EnumerateSourceTableReferences(true))
                        {
                            SubstituteRemoteTableName(tr);
                        }
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
        protected void SubstituteRemoteTableName(TableReference tr)
        {
            var sm = queryObject.GetSchemaManager();
            var temporaryDataset = queryObject.TemporaryDataset;
            var temporarySchemaName = queryObject.TemporaryDataset.DefaultSchemaName;

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

        protected Expression SubstituteTableName(Expression exp, TableReference original, TableReference other)
        {
            exp = (Expression)exp.Clone();

            foreach (var ci in exp.EnumerateDescendantsRecursive<ColumnIdentifier>(typeof(Jhu.Graywulf.SqlParser.Subquery)))
            {
                var cr = ci.ColumnReference;

                if (original.Compare(cr.TableReference))
                {
                    // TODO. this might be needed
                    //cr.ColumnName = EscapePropagatedColumnName(cr.TableReference, cr.ColumnName);
                    cr.TableReference = other;
                }
            }

            return exp;
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

            var sql = new StringBuilder(SqlQueryScripts.TableStatistics);
            SubstituteTableStatisticsQueryTokens(sql, tableSource);

            var cmd = new SqlCommand(sql.ToString());
            AppendTableStatisticsCommandParameters(tableSource, cmd);
            return cmd;
        }

        protected void SubstituteTableStatisticsQueryTokens(StringBuilder sql, ITableSource tableSource)
        {
            var tablename = GenerateEscapedUniqueName(tableSource.TableReference);
            var temptable = queryObject.GetTemporaryTable("stat_" + tablename);
            var keycol = Execute(tableSource.TableReference.Statistics.KeyColumn);
            var keytype = tableSource.TableReference.Statistics.KeyColumnDataType.NameWithLength;
            var where = GetTableSpecificWhereClause(tableSource);

            sql.Replace("[$temptable]", GetResolvedTableName(temptable));
            sql.Replace("[$keytype]", keytype);
            sql.Replace("[$keycol]", keycol);
            sql.Replace("[$tablename]", GetResolvedTableNameWithAlias(tableSource.TableReference));
            sql.Replace("[$where]", Execute(where));
        }

        protected virtual void AppendTableStatisticsCommandParameters(ITableSource tableSource, SqlCommand cmd)
        {
            cmd.Parameters.Add("@BinCount", SqlDbType.Int).Value = tableSource.TableReference.Statistics.BinCount;
        }

        protected virtual WhereClause GetTableSpecificWhereClause(ITableSource tableSource)
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
            var sc = GetPartitioningConditions(ts.PartitioningKeyExpression);
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

        protected SearchCondition GetPartitioningConditions(Expression partitioningKeyExpression)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom) && !IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
            {
                var from = GetPartitioningKeyFromCondition(partitioningKeyExpression);
                var to = GetPartitioningKeyToCondition(partitioningKeyExpression);
                return SearchCondition.Create(from, to, LogicalOperator.CreateAnd());
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyFrom))
            {
                return GetPartitioningKeyFromCondition(partitioningKeyExpression);
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyTo))
            {
                return GetPartitioningKeyToCondition(partitioningKeyExpression);
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
        private SearchCondition GetPartitioningKeyFromCondition(Expression partitioningKeyExpression)
        {
            var a = Expression.Create(SqlParser.Variable.Create(keyFromParameterName));
            var p = Predicate.CreateLessThanOrEqual(a, partitioningKeyExpression);
            return SearchCondition.Create(false, p);
        }

        /// <summary>
        /// Generates a parsing tree segment for the the partitioning key upper limit
        /// </summary>
        /// <param name="partitioningKey"></param>
        /// <returns></returns>
        private SearchCondition GetPartitioningKeyToCondition(Expression partitioningKeyExpression)
        {
            var b = Expression.Create(SqlParser.Variable.Create(keyToParameterName));
            var p = Predicate.CreateLessThan(partitioningKeyExpression, b);
            return SearchCondition.Create(false, p);
        }

        #endregion
    }
}
