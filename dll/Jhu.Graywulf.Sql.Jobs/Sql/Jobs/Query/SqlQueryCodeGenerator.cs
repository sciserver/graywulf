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

        private const string partitionCountParameterName = "@__partCount";
        private const string partitionIdParameterName = "@__partId";
        private const string partitionKeyMinParameterName = "@__partKeyMin";
        private const string partitionKeyMaxParameterName = "@__partKeyMax";

        private static readonly Dictionary<string, string> systemVariableMap = new Dictionary<string, string>(SqlParser.ComparerInstance)
        {
            { NameResolution.Constants.SystemVariableNamePartCount, partitionCountParameterName },
            { NameResolution.Constants.SystemVariableNamePartId, partitionIdParameterName},
        };

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

            if (queryObject.Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                AddSystemDatabaseMappings();
                AddSourceTableMappings(Partition.Parameters.SourceDatabaseVersionName, null);
                AddOutputTableMappings();
            }

            return OnGetExecuteQuery(query);
        }

        protected virtual SourceQuery OnGetExecuteQuery(QueryDetails query)
        {
            RewriteForExecute(query);
            RemoveNonStandardTokens(query);

            var sql = new StringBuilder();
            sql.AppendLine(Execute(query.ParsingTree));

            var source = new SourceQuery()
            {
                Query = sql.ToString(),
            };

            AppendPartitionParameters(source);

            return source;
        }

        protected virtual void RewriteNodes(Node node)
        {
            if (node is SystemVariable)
            {
                var name = node.Value.TrimStart('@');
                if (systemVariableMap.ContainsKey(name))
                {
                    var nn = UserVariable.Create(systemVariableMap[name]);
                    node.ExchangeWith(nn);
                }
            }
            else
            {
                var n = node.Stack.First;
                while (n != null)
                {
                    if (n.Value is Node)
                    {
                        RewriteNodes((Node)n.Value);
                    }

                    n = n.Next;
                }

            }
        }

        protected virtual void RewriteForExecute(QueryDetails query)
        {
            RewriteNodes(query.ParsingTree);

            foreach (var s in query.ParsingTree.EnumerateSubStatements())
            {
                RewriteForExecute(s);
            }
        }

        protected virtual void RewriteForExecute(Statement statement)
        {
            if (statement.SpecificStatement is SelectStatement)
            {
                RewriteForExecute((SelectStatement)statement.SpecificStatement);
            }
            else
            {
                foreach (var s in statement.SpecificStatement.EnumerateSubStatements())
                {
                    RewriteForExecute(s);
                }
            }
        }

        protected virtual void RewriteForExecute(SelectStatement selectStatement)
        {
            // Exchange INTO with magic message
            var into = selectStatement.FindDescendantRecursive<IntoClause>();

            if (into != null)
            {
                var parent = selectStatement.FindAscendant<StatementBlock>();
                var tr = MapTableReference(into.TableName.TableReference);

                into.Parent.Stack.Remove(into);

                // Create a magic statement and insert before the SELECT
                var msg = new IO.Tasks.ServerMessage()
                {
                    DestinationSchema = tr.SchemaName,
                    DestinationName = tr.DatabaseObjectName,
                };
                var print = PrintStatement.Create("'" + msg.Serialize().Replace("'", "''") + "'");
                var sb = StatementBlock.Create(print, selectStatement);
                var be = BeginEndStatement.Create(sb);

                selectStatement.ExchangeWith(be);
            }

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

            // TODO: maybe keep the order by clause around so results
            // can be displayed in right order
            // Or use order by clause to generate identity column
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
            foreach (var key in queryObject.QueryDetails.SourceTableReferences.Keys)
            {
                foreach (var tr in queryObject.QueryDetails.SourceTableReferences[key])
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

            if (ntr != null && !TableReferenceMap.ContainsKey(tr))
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
            var key = tr.DatabaseObject.UniqueKey;

            // TODO: write function to determine if a table is to be copied
            // ie. the condition in the if clause of the following line
            if (queryObject.TemporaryTables.ContainsKey(key))
            {
                var temporaryDataset = queryObject.TemporaryDataset;
                var temporarySchemaName = queryObject.TemporaryDataset.DefaultSchemaName;

                var ntr = new TableReference(tr)
                {
                    DatasetName = temporaryDataset.Name,
                    DatabaseName = temporaryDataset.DatabaseName,
                    SchemaName = temporarySchemaName,
                    DatabaseObjectName = queryObject.TemporaryTables.GetValue(key).TableName,
                    DatabaseObject = null
                };

                // TODO: verify if this call here breaks any logic elsewhere, because
                // the temp table might not yes exist
                ntr.DatabaseObject = temporaryDataset.Tables[ntr.DatabaseName, ntr.SchemaName, ntr.DatabaseObjectName];

                return ntr;
            }
            else
            {
                return null;
            }
        }

        public void AddOutputTableMappings()
        {
            foreach (var key in queryObject.QueryDetails.OutputTableReferences.Keys)
            {
                foreach (var tr in queryObject.QueryDetails.OutputTableReferences[key])
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
            var key = tr.DatabaseObject.UniqueKey;

            if (queryObject.TemporaryTables.ContainsKey(key))
            {
                var temporaryDataset = queryObject.TemporaryDataset;
                var temporarySchemaName = queryObject.TemporaryDataset.DefaultSchemaName;

                var ntr = new TableReference(tr)
                {
                    DatabaseName = temporaryDataset.DatabaseName,
                    SchemaName = temporarySchemaName,
                    DatabaseObjectName = queryObject.TemporaryTables.GetValue(key).TableName,
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
        protected void AddColumnTableReferenceMappings(Expression exp, TableReference original, TableReference other)
        {
            foreach (var ci in exp.EnumerateDescendantsRecursive<ColumnIdentifier>(typeof(Subquery)))
            {
                var cr = ci.ColumnReference;

                if (original.Compare(cr.TableReference) && !ColumnReferenceMap.ContainsKey(cr))
                {
                    var ncr = new ColumnReference(cr);
                    ncr.TableReference = other;
                    ColumnReferenceMap.Add(cr, ncr);
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
                AddSourceTableMappings(queryObject.Parameters.StatDatabaseVersionName, queryObject.Parameters.SourceDatabaseVersionName);
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

            var table = MapTableReference(tableSource.TableReference);
            var tablename = GenerateEscapedUniqueName(table);
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
                par.ParameterName = partitionKeyMinParameterName;
                par.Value = partitioningKeyMin;
                cmd.Parameters.Add(par);
            }

            if (!IsPartitioningKeyUnbound(partitioningKeyMax))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = partitionKeyMaxParameterName;
                par.Value = partitioningKeyMax;
                cmd.Parameters.Add(par);
            }
        }

        public void AppendPartitionParameters(SourceQuery q)
        {
            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMin))
            {
                q.Parameters.Add(partitionKeyMinParameterName, Partition.PartitioningKeyMin);
            }

            if (!IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                q.Parameters.Add(partitionKeyMaxParameterName, Partition.PartitioningKeyMax);
            }

            if (Partition.Query != null)
            {
                q.Parameters.Add(partitionCountParameterName, Partition.Query.Partitions.Count);
                q.Parameters.Add(partitionIdParameterName, Partition.ID + 1);
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
            var a = Expression.Create(Sql.Parsing.Variable.Create(partitionKeyMinParameterName));
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
            var b = Expression.Create(Sql.Parsing.Variable.Create(partitionKeyMaxParameterName));
            var p = Predicate.CreateLessThan(partitioningKeyExpression, b);
            return BooleanExpression.Create(false, p);
        }

        #endregion
    }
}
