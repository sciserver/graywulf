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

        public virtual SourceTableQuery GetExecuteQuery(SelectStatement selectStatement, CommandMethod method, Table destination)
        {
            var sql = new StringBuilder();
            var ss = new SelectStatement(selectStatement);

            RewriteForExecute(ss);
            RemoveNonStandardTokens(ss, method);

            SubstituteDatabaseNames(ss, queryObject.AssignedServerInstance, Partition.Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames(ss, queryObject.TemporaryDataset, queryObject.TemporaryDataset.DefaultSchemaName);

            AppendQuery(sql, ss, method, destination);

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

        protected void SubstituteDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion)
        {
            SubstituteDatabaseNames(ss, serverInstance, databaseVersion, null);
        }

        /// <summary>
        /// Looks up actual database instance names on the specified server instance
        /// </summary>
        /// <param name="serverInstance"></param>
        /// <param name="databaseVersion"></param>
        /// <remarks>This function call must be synchronized!</remarks>
        protected void SubstituteDatabaseNames(SelectStatement ss, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            foreach (var qs in ss.EnumerateQuerySpecifications())
            {
                foreach (var tr in qs.EnumerateSourceTableReferences(true))
                {
                    if (!tr.IsUdf && !tr.IsSubquery && !tr.IsComputed)
                    {
                        SubstituteDatabaseName(tr, serverInstance, databaseVersion, surrogateDatabaseVersion);
                    }
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
        protected virtual void SubstituteRemoteTableNames(SelectStatement ss, DatasetBase temporaryDataset, string temporarySchemaName)
        {
            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // No remote table support
                    throw new InvalidOperationException();
                case ExecutionMode.Graywulf:
                    var sm = queryObject.GetSchemaManager();

                    foreach (var qs in ss.EnumerateQuerySpecifications())
                    {
                        // Replace remote table references with temp table references
                        foreach (TableReference tr in qs.EnumerateSourceTableReferences(true))
                        {
                            SubstituteRemoteTableName(sm, tr, temporaryDataset, temporarySchemaName);
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

            var sql = new StringBuilder(SqlQueryScripts.TableStatistics);
            SubstituteTableStatisticsQueryTokens(sql, tableSource);
            return new SqlCommand(sql.ToString());
        }

        protected void SubstituteTableStatisticsQueryTokens(StringBuilder sql, ITableSource tableSource)
        {
            var tablename = GetEscapedUniqueName(tableSource.TableReference);
            var temptable = queryObject.GetTemporaryTable("stat_" + tablename);
            var keycol = tableSource.TableReference.Statistics.KeyColumn;
            var keytype = tableSource.TableReference.Statistics.KeyColumnDataType.NameWithLength;
            var where = GetTableSpecificWhereClause(tableSource);

            sql.Replace("[$temptable]", GetResolvedTableName(temptable));
            sql.Replace("[$keytype]", keytype);
            sql.Replace("[$keycol]", keycol);
            sql.Replace("[$tablename]", GetResolvedTableNameWithAlias(tableSource.TableReference));
            sql.Replace("[$where]", where != null ? where.ToString() : "");
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
            var partitioningKeyExpression = Expression.Create(ColumnIdentifier.Create(partitioningKey));
            return GetPartitioningConditions(partitioningKeyExpression);
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
        #region Column propagator functions

        public Dictionary<string, Column> GetColumnList(ITableSource table, ColumnListInclude include)
        {
            var res = new Dictionary<string, Column>();
            var t = (TableOrView)table.TableReference.DatabaseObject;

            if ((include & ColumnListInclude.PrimaryKey) != 0)
            {
                foreach (var cd in t.PrimaryKey.Columns.Values)
                {
                    if (!res.ContainsKey(cd.ColumnName))
                    {
                        res.Add(cd.ColumnName, cd);
                    }
                }
            }

            if ((include & ColumnListInclude.Referenced) != 0)
            {
                foreach (var cr in table.TableReference.ColumnReferences)
                {
                    if (cr.IsReferenced && !res.ContainsKey(cr.ColumnName))
                    {
                        res.Add(cr.ColumnName, t.Columns[cr.ColumnName]);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns a SQL snippet with the list of primary keys
        /// and propagated columns belonging to the table.
        /// </summary>
        /// <param name="table">Reference to the table.</param>
        /// <param name="type">Column list type.</param>
        /// <param name="nullType">Column nullable type.</param>
        /// <param name="tableAlias">Optional table alias prefix, specify null to omit.</param>
        /// <returns>A SQL snippet with the list of columns.</returns>
        public string GeneratePropagatedColumnList(ITableSource table, string tableAlias, ColumnListInclude include, ColumnListType type, ColumnListNullType nullType, bool leadingComma)
        {
            // ---
            string nullstring = null;

            switch (nullType)
            {
                case ColumnListNullType.Nothing:
                    nullstring = String.Empty;
                    break;
                case ColumnListNullType.Null:
                    nullstring = "NULL";
                    break;
                case ColumnListNullType.NotNull:
                    nullstring = "NOT NULL";
                    break;
                default:
                    throw new NotImplementedException();
            }

            // ---
            string format = null;

            switch (type)
            {
                case ColumnListType.ForCreateTable:
                    format = "[{1}] {3} {4}";
                    break;
                case ColumnListType.ForCreateView:
                case ColumnListType.ForInsert:
                    format = "[{1}]";
                    break;
                case ColumnListType.ForSelectWithOriginalName:
                    format = "{0}[{2}] AS [{1}]";
                    break;
                case ColumnListType.ForSelectWithEscapedName:
                    format = "{0}[{1}] AS [{1}]";
                    break;
                case ColumnListType.ForSelectNoAlias:
                    format = "{0}[{1}]";
                    break;
                default:
                    throw new NotImplementedException();
            }

            var columnlist = new StringBuilder();
            var columns = GetColumnList(table, include);

            foreach (var column in columns.Values)
            {
                if (leadingComma || columnlist.Length != 0)
                {
                    columnlist.Append(", ");
                }

                columnlist.AppendFormat(format,
                                        tableAlias == null ? String.Empty : QuoteIdentifier(tableAlias),
                                        EscapePropagatedColumnName(table.TableReference, column.Name),
                                        column.Name,
                                        column.DataType.NameWithLength,
                                        nullstring);
            }

            return columnlist.ToString();
        }

        

        private void IncludeReferencedColumns(ITableSource table, StringBuilder columnList, HashSet<string> referencedColumns, string tableAlias, string format, string nullString)
        {
            foreach (ColumnReference cr in table.TableReference.ColumnReferences)
            {
                if (cr.IsReferenced)
                {
                    string key = EscapeColumnName(table.TableReference, cr.ColumnName);
                    string escapedname = EscapePropagatedColumnName(table.TableReference, cr.ColumnName);

                    if (!referencedColumns.Contains(key))
                    {
                        if (columnList.Length != 0)
                        {
                            columnList.Append(", ");
                        }

                        columnList.AppendFormat(format,
                                            tableAlias == null ? String.Empty : String.Format("[{0}].", tableAlias),
                                            escapedname,
                                            cr.ColumnName,
                                            cr.DataType.NameWithLength,
                                            nullString);

                        referencedColumns.Add(key);
                    }
                }
            }
        }

        #endregion
        #region Column name escaping

        /// <summary>
        /// Generates and escaped name for a column that should be
        /// propagated.
        /// </summary>
        /// <remarks>
        /// Will generate a name like DS_schema_table_column that
        /// is unique in a table.
        /// </remarks>
        /// <param name="table">Reference to the source table.</param>
        /// <param name="column">Reference to the column.</param>
        /// <returns>The excaped name of the temporary table column.</returns>
        protected string EscapeColumnName(TableReference table, string columnName)
        {
            return String.Format("{0}_{1}_{2}_{3}_{4}",
                                 table.DatasetName,
                                 table.SchemaName,
                                 table.DatabaseObjectName,
                                 table.Alias,
                                 columnName);
        }

        protected string EscapePropagatedColumnName(TableReference table, string columnName)
        {
            return String.Format("_{0}", EscapeColumnName(table, columnName));
        }

        #endregion
    }
}
