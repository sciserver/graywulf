using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
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

        public override void PrepareExecuteQuery(Context context)
        {
            base.PrepareExecuteQuery(context);

            // --- strip off orderBy clause
            OrderByClause orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }

            SubstituteDatabaseNames(Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames();

            // Prepare query for execution
            QuerySpecification qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault<QuerySpecification>();
            
            // *** TODO: test this here. Will not work with functions, etc
            AnyTableSource ats = qs.EnumerateTableSources(false).First();
            TableSource ts = ats.FindDescendant<TableSource>();

            // --- append partitioning condition
            if (ts.IsPartitioned)
            {
                string sql = String.Format("{0} >= {1} AND {0} < {2}",
                    ts.PartitioningColumnReference.FullyQualifiedName,
                    PartitioningKeyFrom.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    PartitioningKeyTo.ToString(System.Globalization.CultureInfo.InvariantCulture));


                WhereClause where = qs.FindDescendant<WhereClause>();
                if (where == null)
                {
                    where = new WhereClause();

                    qs.Stack.AddAfter(qs.Stack.Find(qs.FindDescendant<FromClause>()), where);
                }
                else
                {
                    where.AppendCondition(sql, "AND");
                }

                // --- remove partition clause
                ts.Stack.Remove(ts.FindDescendant<TablePartitionClause>());
            }

            //Interpret(true);

            var cg = new SqlServerCodeGenerator();
            var sw = new StringWriter();
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
                    DropTable(AssignedServerInstanceReference.Value.GetConnectionString().ConnectionString,
                        TemporaryDatabaseInstanceReference.Value.DatabaseName,
                        Query.TemporarySchemaName,
                        temptable);

                    // Execute query directly on assigned server storing results
                    // in a temporary table
                    ExecuteSelectInto(AssignedServerInstanceReference.Value.GetConnectionString().ConnectionString,
                                      InterpretedQueryString,
                                      TemporaryDatabaseInstanceReference.Value.DatabaseName,
                                      Query.TemporarySchemaName,
                                      temptable,
                                      Query.QueryTimeout);
                    break;
                default:
                    throw new NotImplementedException();
            }

            while (!TemporaryTables.TryAdd(temptable, temptable))
            {
            }
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
