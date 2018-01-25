using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class SqlQueryCodeGenerator : SqlServerCodeGenerator
    {
        #region Constants

        protected const string partitioningKeyMinParameterName = "@keyMin";
        protected const string partitioningKeyMaxParameterName = "@keyMax";

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
        }

        #endregion
        #region Basic query rewrite functions

        public SourceTableQuery GetExecuteQuery(StatementBlock parsingTree)
        {
            // TODO add support for multi-statement queries

            var ss = (SelectStatement)parsingTree.FindDescendantRecursive<SelectStatement>().Clone();
            return OnGetExecuteQuery(ss);
        }

        protected virtual SourceTableQuery OnGetExecuteQuery(SelectStatement selectStatement)
        {
            var sql = new StringBuilder();

            RewriteForExecute(selectStatement);
            RemoveNonStandardTokens(selectStatement);

            if (queryObject.Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                SubstituteServerSpecificDatabaseNames(selectStatement, queryObject.AssignedServerInstance, Partition.Parameters.SourceDatabaseVersionName);
                SubstituteRemoteTableNames(selectStatement);
            }

            sql.AppendLine(Execute(selectStatement));

            var source = new SourceTableQuery()
            {
                Query = sql.ToString(),
            };

            AppendPartitioningConditionParameters(source);

            return source;
        }

        protected virtual void RewriteForExecute(SelectStatement selectStatement)
        {
            int i = 0;
            foreach (var qs in selectStatement.QueryExpression.EnumerateQuerySpecifications())
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

        protected virtual void RemoveNonStandardTokens(SelectStatement selectStatement)
        {
            // strip off partition by and into clauses
            var qe = selectStatement.FindDescendant<QueryExpression>();

            if (qe != null)
            {
                RemoveNonStandardTokens(qe);
            }

            // Strip off order by, we write to the mydb
            var orderby = selectStatement.FindDescendant<OrderByClause>();

            if (orderby != null)
            {
                selectStatement.Stack.Remove(orderby);
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

        /// <summary>
        /// Descends recursively the parsing tree and replaces the occurances of one table
        /// reference with another.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="old"></param>
        /// <param name="other"></param>
        protected void SubstituteTableReference(Node node, TableReference old, TableReference other)
        {
            foreach (var t in node.Stack)
            {
                if (t is Node && !(t is Subquery))
                {
                    SubstituteTableReference((Node)t, old, other);
                }
            }

            var trnode = node as ITableReference;
            if (trnode != null && trnode.TableReference.Compare(old))
            {
                trnode.TableReference = other;
            }
        }

        protected void SubstituteTableReference(ITableSource tableSource, TableReference tr)
        {
            tableSource.TableReference = tr;
        }

        protected void SubstituteServerSpecificDatabaseNames(SelectStatement ss)
        {
            if (queryObject != null && queryObject.AssignedServerInstance != null && queryObject.CodeDataset != null)
            {
                SubstituteServerSpecificDatabaseNames(ss, queryObject.AssignedServerInstance, queryObject.Parameters.SourceDatabaseVersionName);
            }
        }

        private void SubstituteServerSpecificDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion)
        {
            SubstituteServerSpecificDatabaseNames(ss, serverInstance, databaseVersion, null);
        }

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        private void SubstituteServerSpecificDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            switch (queryObject.Parameters.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    throw new InvalidOperationException();
                case ExecutionMode.Graywulf:
                    foreach (var qs in ss.QueryExpression.EnumerateQuerySpecifications())
                    {
                        foreach (var ts in qs.EnumerateSourceTables(true))
                        {
                            var tr = ts.TableReference;
                            SubstituteServerSpecificDatabaseName(tr, serverInstance, databaseVersion, surrogateDatabaseVersion);
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

            if (tr.Type != TableReferenceType.Subquery &&
                tr.Type != TableReferenceType.CommonTable &&
                !tr.IsComputed && 
                tr.DatabaseName != null)
            {
                var ds = sc.Datasets[tr.DatasetName];

                // Graywulf datasets have changing database names depending on the server
                // the database is on.
                if (ds is GraywulfDataset)
                {
                    var gwds = ds as GraywulfDataset;
                    gwds.RegistryContext = queryObject.RegistryContext;

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

        public void SubstituteSystemDatabaseNames(Expression ex)
        {
            SubstituteSystemDatabaseNames((Node)ex);
        }

        public void SubstituteSystemDatabaseNames(Node n)
        {
            foreach (var i in n.Nodes)
            {
                if (i is Node)
                {
                    SubstituteSystemDatabaseNames((Node)i);
                }
            }

            if (n is IFunctionReference)
            {
                SubstituteSystemDatabaseName(((IFunctionReference)n).FunctionReference);
            }
        }

        private void SubstituteSystemDatabaseName(FunctionReference fr)
        {
            if (fr != null && !fr.IsSystem && fr.IsUdf)
            {
                fr.DatabaseName = CodeDataset.DatabaseName;
            }
        }

        /// <summary>
        /// Substitutes names of remote tables with name of temporary tables
        /// holding a cached version of remote tables.
        /// </summary>
        /// <remarks></remarks>
        protected virtual void SubstituteRemoteTableNames(SelectStatement ss)
        {
            if (queryObject != null)
            {
                switch (queryObject.Parameters.ExecutionMode)
                {
                    case ExecutionMode.SingleServer:
                        // Nothing to do here
                        break;
                    case ExecutionMode.Graywulf:
                        foreach (var qs in ss.QueryExpression.EnumerateQuerySpecifications())
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

        /// <summary>
        /// Creates a clone of the expression tree, then descends it  and replaces the occurances
        /// of one table reference with another.
        /// </summary>
        /// <remarks>
        /// This function is used to add a table alias to columns.
        /// </remarks>
        /// <param name="exp"></param>
        /// <param name="original"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        protected Expression SubstituteColumnTableReference(Expression exp, TableReference original, TableReference other)
        {
            exp = (Expression)exp.Clone();

            foreach (var ci in exp.EnumerateDescendantsRecursive<ColumnIdentifier>(typeof(Subquery)))
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

        public virtual ITableSource SubstituteStatisticsDataset(ITableSource tableSource, DatasetBase statisticsDataset)
        {
            if (statisticsDataset != null)
            {
                var nts = (ITableSource)tableSource.Clone();

                var ntr = new TableReference(tableSource.TableReference);
                ntr.DatabaseName = statisticsDataset.DatabaseName;
                ntr.DatabaseObject = statisticsDataset.GetObject(ntr.DatabaseName, ntr.SchemaName, ntr.DatabaseObjectName);

                var nstat = new TableStatistics(queryObject.TableStatistics[tableSource]);
                SubstituteTableReference(nstat.KeyColumn, tableSource.TableReference, ntr);
                SubstituteTableReference(nts, ntr);

                return nts;
            }
            else
            {
                return tableSource;
            }
        }

        /// <summary>
        /// Returns a command initialized for computing table statistics
        /// </summary>
        /// <param name="tableSource"></param>
        /// <returns></returns>
        public virtual SqlCommand GetTableStatisticsCommand(ITableSource tableSource, DatasetBase statisticsDataset)
        {
            if (!(tableSource.TableReference.DatabaseObject is TableOrView))
            {
                throw new ArgumentException();
            }

            if (!queryObject.TableStatistics.ContainsKey(tableSource))
            {
                throw new ArgumentNullException();
            }

            var sql = new StringBuilder(SqlQueryScripts.TableStatistics);
            var statts = SubstituteStatisticsDataset(tableSource, statisticsDataset);
            SubstituteTableStatisticsQueryTokens(sql, statts);

            var cmd = new SqlCommand(sql.ToString());
            AppendTableStatisticsCommandParameters(tableSource, cmd);
            return cmd;
        }

        protected void SubstituteTableStatisticsQueryTokens(StringBuilder sql, ITableSource tableSource)
        {
            var stat = queryObject.TableStatistics[tableSource];

            SubstituteSystemDatabaseNames(stat.KeyColumn);

            var tablename = GenerateEscapedUniqueName(tableSource.TableReference);
            var temptable = queryObject.GetTemporaryTable("stat_" + tablename);
            var keycol = Execute(stat.KeyColumn);
            var keytype = stat.KeyColumnDataType.TypeNameWithLength;
            var where = GetTableSpecificWhereClause(tableSource);

            sql.Replace("[$temptable]", GetResolvedTableName(temptable));
            sql.Replace("[$keytype]", keytype);
            sql.Replace("[$keycol]", keycol);
            sql.Replace("[$tablename]", GetResolvedTableNameWithAlias(tableSource.TableReference));
            sql.Replace("[$where]", Execute(where));
        }

        protected virtual void AppendTableStatisticsCommandParameters(ITableSource tableSource, SqlCommand cmd)
        {
            var stat = queryObject.TableStatistics[tableSource];
            cmd.Parameters.Add("@BinCount", SqlDbType.Int).Value = stat.BinCount;
        }

        protected virtual WhereClause GetTableSpecificWhereClause(ITableSource tableSource)
        {
            /*
            var tr = tableSource.TableReference;

            var cnr = new Sql.LogicalExpressions.SearchConditionNormalizer();
            cnr.CollectConditions(((TableSource)tr.Node).QuerySpecification);
            var where = cnr.GenerateWherePredicatesSpecificToTable(tr);

            return where;
            */

            throw new NotImplementedException();
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
            AppendPartitioningConditionParameters(cmd, Partition.PartitioningKeyMin, Partition.PartitioningKeyMax);
        }

        public void AppendPartitioningConditionParameters(SqlCommand cmd, IComparable partitioningKeyMin, IComparable partitioningKeyMax)
        {
            if (!IsPartitioningKeyUnbound(partitioningKeyMin))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = partitioningKeyMinParameterName;
                par.Value = partitioningKeyMin;
                cmd.Parameters.Add(par);
            }

            if (!IsPartitioningKeyUnbound(partitioningKeyMax))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = partitioningKeyMaxParameterName;
                par.Value = partitioningKeyMax;
                cmd.Parameters.Add(par);
            }
        }

        public void AppendPartitioningConditionParameters(SourceTableQuery q)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMin))
            {
                q.Parameters.Add(partitioningKeyMinParameterName, Partition.PartitioningKeyMin);
            }

            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                q.Parameters.Add(partitioningKeyMaxParameterName, Partition.PartitioningKeyMax);
            }
        }

        protected virtual bool IsPartitioningKeyUnbound(object key)
        {
            return key == null;
        }

        protected BooleanExpression GetPartitioningConditions(Expression partitioningKeyExpression)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMin) && !IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                var from = GetPartitioningKeyMinCondition(partitioningKeyExpression);
                var to = GetPartitioningKeyMaxCondition(partitioningKeyExpression);
                return BooleanExpression.Create(from, to, LogicalOperator.CreateAnd());
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMin))
            {
                return GetPartitioningKeyMinCondition(partitioningKeyExpression);
            }
            else if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
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
            var a = Expression.Create(Sql.Parsing.Variable.Create(partitioningKeyMinParameterName));
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
            var b = Expression.Create(Sql.Parsing.Variable.Create(partitioningKeyMaxParameterName));
            var p = Predicate.CreateLessThan(partitioningKeyExpression, b);
            return BooleanExpression.Create(false, p);
        }

        #endregion
    }
}
