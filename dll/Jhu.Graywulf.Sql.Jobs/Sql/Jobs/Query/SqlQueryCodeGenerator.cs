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
using Jhu.Graywulf.Sql.QueryGeneration.SqlServer;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class SqlQueryCodeGenerator : SqlServerQueryGenerator
    {
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

        public SourceQuery GetExecuteQuery(QueryDetails details)
        {
            if (queryObject.Parameters.ExecutionMode == ExecutionMode.Graywulf)
            {
                AddSystemDatabaseMappings();
                AddSourceTableMappings(Partition.Parameters.SourceDatabaseVersionName, null);
                AddOutputTableMappings();
                AddSystemVariableMappings();
            }

            return OnGetExecuteQuery(details);
        }

        protected virtual SourceQuery OnGetExecuteQuery(QueryDetails details)
        {
            var sql = new StringBuilder();
            sql.AppendLine(Renderer.Execute(details.ParsingTree));

            var source = new SourceQuery()
            {
                Query = sql.ToString(),
            };

            AppendPartitionParameters(source);

            return source;
        }

        #endregion
        #region Graywulf table reference mapping

        public void AddSystemDatabaseMappings()
        {
        }

        public void AddSystemDatabaseMappings(TableSource tableSource)
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

            if (ntr != null && !Renderer.TableReferenceMap.ContainsKey(tr))
            {
                Renderer.TableReferenceMap.Add(tr, ntr);
            }
        }

        private TableReference GetServerSpecificDatabaseMapping(TableReference tr, ServerInstance serverInstance, string databaseVersion, string surrogateDatabaseVersion)
        {
            if (!tr.TableContext.HasFlag(TableContext.Subquery) &&
                !tr.TableContext.HasFlag(TableContext.CommonTable) &&
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
                        Renderer.TableReferenceMap.Add(tr, ntr);
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

        public void AddSystemVariableMappings()
        {
            // TODO: this is alredy covered in query rewriter

            throw new NotImplementedException();

            foreach (var pair in Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.SystemVariableMappings)
            {
                var sysvar = new VariableReference()
                {
                    VariableName = "@@" + pair.Key,
                };

                var subsvar = new VariableReference()
                {
                    VariableName = pair.Value,
                    IsUserDefined = true,
                };

                Renderer.VariableReferenceMap.Add(sysvar, subsvar);
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
            if (trnode != null && trnode.TableReference.TryMatch(old))
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
            if (fr != null && !fr.IsSystem && fr.IsUndefined)
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

                if (original.TryMatch(cr.TableReference) && !Renderer.ColumnReferenceMap.ContainsKey(cr))
                {
                    var ncr = new ColumnReference(cr);
                    ncr.TableReference = other;
                    Renderer.ColumnReferenceMap.Add(cr, ncr);
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
        public virtual SqlCommand GetTableStatisticsCommand(TableSource tableSource, DatasetBase statisticsDataset)
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


        protected virtual WhereClause GetTableSpecificWhereClause(TableSource tableSource)
        {
            var ts = (SimpleTableSource)tableSource;
            var qs = ts.FindAscendant<QuerySpecification>();
            var scn = new Graywulf.Sql.LogicalExpressions.SearchConditionNormalizer();

            scn.CollectConditions(qs);

            var predicates = scn.GenerateWherePredicatesSpecificToTable(ts.TableReference);
            var where = predicates == null ? null : WhereClause.Create(predicates);

            return where;
        }

        protected void SubstituteTableStatisticsQueryTokens(StringBuilder sql, TableSource tableSource)
        {
            var stat = queryObject.TableStatistics[tableSource.UniqueKey];

            SubstituteSystemDatabaseNames(stat.KeyColumn);

            var table = Renderer.MapTableReference(tableSource.TableReference);
            var tablename = GenerateEscapedUniqueName(table);
            var temptable = queryObject.GetTemporaryTable("stat_" + tablename);
            var keycol = Renderer.Execute(stat.KeyColumn);
            var keytype = stat.KeyColumnDataType.TypeNameWithLength;
            var where = GetTableSpecificWhereClause(tableSource);

            sql.Replace("[$temptable]", Renderer.GetResolvedTableName(temptable));
            sql.Replace("[$keytype]", keytype);
            sql.Replace("[$keycol]", keycol);
            sql.Replace("[$tablename]", Renderer.GetResolvedTableNameWithAlias(tableSource.TableReference));
            sql.Replace("[$where]", Renderer.Execute(where));
        }

        protected virtual void AppendTableStatisticsCommandParameters(TableSource tableSource, SqlCommand cmd)
        {
            var stat = queryObject.TableStatistics[tableSource.UniqueKey];
            cmd.Parameters.Add("@BinCount", SqlDbType.Int).Value = stat.BinCount;
        }

        #endregion
        #region Query partitioning

        public void AppendPartitioningConditionParameters(SqlCommand cmd)
        {
            AppendPartitioningConditionParameters(cmd, Partition.PartitioningKeyMin, Partition.PartitioningKeyMax);
        }

        public void AppendPartitioningConditionParameters(SqlCommand cmd, IComparable partitioningKeyMin, IComparable partitioningKeyMax)
        {
            if (!Partition.IsPartitioningKeyUnbound(partitioningKeyMin))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionKeyMinParameterName;
                par.Value = partitioningKeyMin;
                cmd.Parameters.Add(par);
            }

            if (!Partition.IsPartitioningKeyUnbound(partitioningKeyMax))
            {
                var par = cmd.CreateParameter();
                par.ParameterName = Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionKeyMaxParameterName;
                par.Value = partitioningKeyMax;
                cmd.Parameters.Add(par);
            }
        }

        public void AppendPartitionParameters(SourceQuery q)
        {
            if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMin))
            {
                q.Parameters.Add(Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionKeyMinParameterName, Partition.PartitioningKeyMin);
            }

            if (!Partition.IsPartitioningKeyUnbound(Partition.PartitioningKeyMax))
            {
                q.Parameters.Add(Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionKeyMaxParameterName, Partition.PartitioningKeyMax);
            }

            if (Partition.Query != null)
            {
                q.Parameters.Add(Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionCountParameterName, Partition.Query.Partitions.Count);
                q.Parameters.Add(Jhu.Graywulf.Sql.Extensions.QueryRewriting.Constants.PartitionIdParameterName, Partition.ID + 1);
            }
        }

        #endregion
    }
}
