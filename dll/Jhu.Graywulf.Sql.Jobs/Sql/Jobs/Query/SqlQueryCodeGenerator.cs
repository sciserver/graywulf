﻿using System;
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
        private SchemaManager schemaManager;

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

        public SchemaManager SchemaManager
        {
            get { return schemaManager; }
            set { schemaManager = value; }
        }

        private SqlQueryPartition Partition
        {
            get { return queryObject as SqlQueryPartition; }
        }

        #endregion
        #region Constructors and initializers

        public SqlQueryCodeGenerator()
        {
            InitializeMembers();
        }

        public SqlQueryCodeGenerator(QueryObject queryObject)
        {
            InitializeMembers();

            if (queryObject != null)
            {
                this.queryObject = queryObject;
                this.codeDataset = queryObject.CodeDataset;
                this.tempDataset = queryObject.TemporaryDataset;
                this.schemaManager = queryObject.GetSchemaManager();
            }
        }

        private void InitializeMembers()
        {
            this.queryObject = null;
            this.codeDataset = null;
            this.tempDataset = null;
            this.schemaManager = null;
        }

        #endregion
        #region Basic query rewrite functions

        public SourceQuery GetExecuteQuery()
        {
            // Make a clone so that the parsing tree can be modified
            var query = new QueryDetails(queryObject.QueryDetails);
            return OnGetExecuteQuery(query);
        }

        protected virtual SourceQuery OnGetExecuteQuery(QueryDetails query)
        {
            var sql = new StringBuilder();

            RewriteForExecute(query);
            RemoveNonStandardTokens(query);

            if (queryObject.Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                AddSystemDatabaseMappings();
                AddSourceTableMappings(Partition.Parameters.SourceDatabaseVersionName, null);
                AddOutputTableMappings();
            }

            sql.AppendLine(Execute(query.ParsingTree));

            var source = new SourceQuery()
            {
                Query = sql.ToString(),
            };

            AppendPartitioningConditionParameters(source);

            return source;
        }

        protected virtual void RewriteForExecute(QueryDetails query)
        {
            foreach (var statement in query.ParsingTree.EnumerateSubStatements())
            {
                if (statement.SpecificStatement is SelectStatement)
                {
                    RewriteForExecute((SelectStatement)statement.SpecificStatement);
                }
            }
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

        protected virtual void RemoveNonStandardTokens(QueryDetails query)
        {
            foreach (var statement in query.ParsingTree.EnumerateSubStatements())
            {
                if (statement.SpecificStatement is SelectStatement)
                {
                    RemoveNonStandardTokens((SelectStatement)statement.SpecificStatement);
                }
            }
        }

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
        #region Graywulf table reference mapping

        public void AddSystemDatabaseMappings()
        {
        }

        public void AddSystemDatabaseMappings(ITableSource tableSource)
        {
        }

        public void AddSourceTableMappings(string databaseVersion, string surrogateDatabaseVersion)
        {
            foreach (var key in queryObject.QueryDetails.SourceTables.Keys)
            {
                foreach (var tr in queryObject.QueryDetails.SourceTables[key])
                {
                    AddSourceTableMapping(tr, databaseVersion, surrogateDatabaseVersion);
                }
            }
        }

        private void AddSourceTableMapping(TableReference tr, string databaseVersion, string surrogateDatabaseVersion)
        {
            TableReference ntr = null;

            if (tr.IsCachable && queryObject.IsRemoteDataset(SchemaManager.Datasets[tr.DatasetName]))
            {
                ntr = GetRemoteSourceTableMapping(tr);
            }
            else
            {
                ntr = GetServerSpecificDatabaseMapping(tr, queryObject.AssignedServerInstance, databaseVersion, surrogateDatabaseVersion);
            }


            if (ntr != null)
            {
                TableReferenceMap.Add(tr, ntr);
            }
        }

        private TableReference GetServerSpecificDatabaseMapping(TableReference tr, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            if (tr.Type != TableReferenceType.Subquery &&
                tr.Type != TableReferenceType.CommonTable &&
                !tr.IsComputed &&
                tr.DatasetName != null)
            {
                TableReference ntr = null;
                var ds = SchemaManager.Datasets[tr.DatasetName];

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

                    ntr = new TableReference(tr);
                    ntr.DatabaseName = di.DatabaseName;
                    ntr.DatabaseObject = ds.GetObject(ntr.DatabaseName, ntr.SchemaName, ntr.DatabaseObjectName);
                }

                return ntr;
            }
            else
            {
                return null;
            }
        }

        private TableReference GetRemoteSourceTableMapping(TableReference tr)
        {
            // Save unique name because it will change as names are substituted
            var key = tr.DatabaseObject.UniqueKey;

            // TODO: write function to determine if a table is to be copied
            // ie. the condition in the if clause of the following line
            if (queryObject.TemporaryTables.ContainsKey(key))
            {
                var temporaryDataset = queryObject.TemporaryDataset;
                var temporarySchemaName = queryObject.TemporaryDataset.DefaultSchemaName;

                var ntr = new TableReference(tr)
                {
                    DatabaseName = temporaryDataset.DatabaseName,
                    SchemaName = temporarySchemaName,
                    DatabaseObjectName = queryObject.TemporaryTables[key].TableName,
                    DatabaseObject = null
                };

                return ntr;
            }
            else
            {
                return null;
            }
        }

        public void AddOutputTableMappings()
        {
            foreach (var key in queryObject.QueryDetails.OutputTables.Keys)
            {
                foreach (var tr in queryObject.QueryDetails.OutputTables[key])
                {
                    var ntr = GetRemoteOutputTableMapping(tr);

                    if (ntr != null)
                    {
                        TableReferenceMap.Add(tr, ntr);
                    }
                }
            }
        }

        private TableReference GetRemoteOutputTableMapping(TableReference tr)
        {
            // Save unique name because it will change as names are substituted
            var key = tr.DatabaseObject.UniqueKey;

            if (queryObject.TemporaryTables.ContainsKey(key))
            {
                var temporaryDataset = queryObject.TemporaryDataset;
                var temporarySchemaName = queryObject.TemporaryDataset.DefaultSchemaName;

                var ntr = new TableReference(tr)
                {
                    DatabaseName = temporaryDataset.DatabaseName,
                    SchemaName = temporarySchemaName,
                    DatabaseObjectName = queryObject.TemporaryTables[key].TableName,
                    DatabaseObject = null
                };

                return ntr;
            }
            else
            {
                return null;
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

        /* TODO: delete
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
        */

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

            if (!queryObject.TableStatistics.ContainsKey(tableSource.UniqueKey))
            {
                throw new ArgumentNullException();
            }

            var sql = new StringBuilder(SqlQueryScripts.TableStatistics);

            if (queryObject.Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                AddSystemDatabaseMappings(tableSource);
                AddSourceTableMappings(queryObject.Parameters.SourceDatabaseVersionName, null);
                AddOutputTableMappings();
            }

            SubstituteTableStatisticsQueryTokens(sql, tableSource);

            var cmd = new SqlCommand(sql.ToString());
            AppendTableStatisticsCommandParameters(tableSource, cmd);
            return cmd;
        }
        
        protected virtual WhereClause GetTableSpecificWhereClause(ITableSource tableSource)
        {
            var ts = (SimpleTableSource)tableSource;
            var qs = ts.FindAscendant<QuerySpecification>();
            var scn = new Graywulf.Sql.LogicalExpressions.SearchConditionNormalizer();

            scn.CollectConditions(qs);

            var predicates = scn.GenerateWherePredicatesSpecificToTable(ts.TableReference);
            var where = predicates == null ? null : WhereClause.Create(predicates);

            return where;
        }

        protected void SubstituteTableStatisticsQueryTokens(StringBuilder sql, ITableSource tableSource)
        {
            var stat = queryObject.TableStatistics[tableSource.UniqueKey];

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
            var stat = queryObject.TableStatistics[tableSource.UniqueKey];
            cmd.Parameters.Add("@BinCount", SqlDbType.Int).Value = stat.BinCount;
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

        public void AppendPartitioningConditionParameters(SourceQuery q)
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
