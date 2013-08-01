using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlParser.SqlCodeGen;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryPartition : QueryPartitionBase, ICloneable
    {
        #region Constructors and initializers

        public SqlQueryPartition()
            : base()
        {
            InitializeMembers();
        }

        public SqlQueryPartition(SqlQuery query, Context context)
            : base(query, context)
        {
            InitializeMembers();
        }

        public SqlQueryPartition(SqlQueryPartition old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(SqlQueryPartition old)
        {
        }

        public override object Clone()
        {
            return new SqlQueryPartition(this);
        }

        #endregion

        public override void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            base.PrepareExecuteQuery(context, scheduler);

            // --- strip off orderBy clause
            OrderByClause orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }

            // Prepare query for execution
            QuerySpecification qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault<QuerySpecification>();
            
            // *** TODO: test this here. Will not work with functions, etc
            var ts = (SimpleTableSource)qs.EnumerateSourceTables(false).First();

            // --- append partitioning condition
            if (ts.IsPartitioned)
            {
                if (!double.IsInfinity(PartitioningKeyFrom) || !double.IsInfinity(PartitioningKeyTo))
                {
                    string format;
                    if (double.IsInfinity(PartitioningKeyFrom) && double.IsInfinity(PartitioningKeyTo))
                    {
                        format = "{1} <= {0} AND {0} < {2}";
                    }
                    else if (double.IsInfinity(PartitioningKeyFrom))
                    {
                        format = "{0} < {2}";
                    }
                    else
                    {
                        format = "{1} <= {0}";
                    }

                    string sql = String.Format(format,
                        ts.PartitioningColumnReference.GetFullyResolvedName(),
                        PartitioningKeyFrom.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        PartitioningKeyTo.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    var parser = new Jhu.Graywulf.SqlParser.SqlParser();
                    var sc = (SearchCondition)parser.Execute(new SearchCondition(), sql);

                    var where = qs.FindDescendant<WhereClause>();
                    if (where == null)
                    {
                        where = WhereClause.Create(sc);
                        var ws = Whitespace.Create();

                        var wsn = qs.Stack.AddAfter(qs.Stack.Find(qs.FindDescendant<FromClause>()), ws);
                        qs.Stack.AddAfter(wsn, where);
                    }
                    else
                    {
                        where.AppendCondition(sc, "AND");
                    }
                }

                // --- remove partition clause
                ts.Stack.Remove(ts.FindDescendant<TablePartitionClause>());
            }

            SubstituteDatabaseNames(AssignedServerInstance.Guid, Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames(TemporaryDatabaseInstanceReference.Value.GetDataset(), Query.TemporarySchemaName);
            
            var sw = new StringWriter();
            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = true;
            cg.Execute(sw, SelectStatement);

            InterpretedQueryString = sw.ToString();
        }

        public override void ExecuteQuery()
        {
            // ***** TODO: Check if BulkCopy can be run in parallel (possibly)
            
            string temptable = GetTemporaryTableName(Query.TemporaryDestinationTableName);

            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    SqlServerDataset ddd = (SqlServerDataset)Query.Destination.Table.Dataset;
                    SqlServerDataset tdd = (SqlServerDataset)Query.TemporaryDataset;

                    ExecuteSelectInto(ddd.ConnectionString,
                                    InterpretedQueryString,
                                    tdd.DatabaseName,
                                    Query.TemporarySchemaName,
                                    temptable,
                                    Query.QueryTimeout);
                    break;
                case ExecutionMode.Graywulf:

                    // Try drop result temp table
                    DropTable(AssignedServerInstance.GetConnectionString().ConnectionString,
                        TemporaryDatabaseInstanceReference.Value.DatabaseName,
                        Query.TemporarySchemaName,
                        temptable);

                    // Execute query directly on assigned server storing results
                    // in a temporary table
                    ExecuteSelectInto(AssignedServerInstance.GetConnectionString().ConnectionString,
                                      InterpretedQueryString,
                                      TemporaryDatabaseInstanceReference.Value.DatabaseName,
                                      Query.TemporarySchemaName,
                                      temptable,
                                      Query.QueryTimeout);
                    break;
                default:
                    throw new NotImplementedException();
            }

            TemporaryTables.TryAdd(temptable, temptable);
        }

        public override void PrepareCopyResultset(Context context)
        {
            base.PrepareCopyResultset(context);

            // --- strip off orderBy clause
            OrderByClause orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }
        }

        /// <summary>
        /// Copies resultset from the output temporary table to the destination database (MYDB)
        /// </summary>
        public override void CopyResultset()
        {
            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        SqlServerDataset tdd = (SqlServerDataset)Query.TemporaryDataset;
                        SqlServerDataset ddd = (SqlServerDataset)Query.Destination.Table.Dataset;

                        string sql = String.Format("SELECT tablealias.* FROM [{0}].[{1}].[{2}] AS tablealias",
                            tdd.DatabaseName,
                            Query.TemporarySchemaName,
                            GetTemporaryTableName(Query.TemporaryDestinationTableName));

                        if ((Query.ResultsetTarget & (ResultsetTarget.TemporaryTable | ResultsetTarget.DestinationTable)) != 0)
                        {
                            ExecuteInsertInto(
                                tdd.ConnectionString,
                                sql,
                                ddd.DatabaseName,
                                Query.Destination.Table.SchemaName,
                                Query.Destination.Table.TableName,
                                Query.QueryTimeout);
                        }
                    }
                    break;
                case ExecutionMode.Graywulf:
                    {
                        string sql = String.Format("SELECT tablealias.* FROM [{0}].[{1}].[{2}] AS tablealias",
                            TemporaryDatabaseInstanceReference.Value.DatabaseName,
                            Query.TemporarySchemaName,
                            GetTemporaryTableName(Query.TemporaryDestinationTableName));

                        if ((Query.ResultsetTarget & (ResultsetTarget.TemporaryTable | ResultsetTarget.DestinationTable)) != 0)
                        {
                            ExecuteBulkCopy(
                                GetTemporaryDatabaseDataset(),
                                sql,
                                Query.GetDestinationDatabaseConnectionString().ConnectionString,
                                Query.Destination.Table.SchemaName,
                                Query.Destination.Table.TableName,
                                Query.QueryTimeout);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
