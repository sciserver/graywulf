using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public abstract class QueryPartitionBase : QueryObject
    {
        #region Property storage variables

        private int id;

        private QueryBase query;

        private double partitioningKeyFrom;
        private double partitioningKeyTo;

        [NonSerialized]
        private Dictionary<string, TableReference> tableSourceReferences;

        [NonSerialized]
        private Dictionary<string, TableReference> remoteTableReferences;

        #endregion
        #region Properties

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public QueryBase Query
        {
            get { return query; }
        }

        public double PartitioningKeyFrom
        {
            get { return partitioningKeyFrom; }
            set { partitioningKeyFrom = value; }
        }

        public double PartitioningKeyTo
        {
            get { return partitioningKeyTo; }
            set { partitioningKeyTo = value; }
        }

        public Dictionary<string, TableReference> TableSourceReferences
        {
            get { return tableSourceReferences; }
        }

        public Dictionary<string, TableReference> RemoteTableReferences
        {
            get { return remoteTableReferences; }
        }

        #endregion
        #region Constructors and initializer functions

        public QueryPartitionBase()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryPartitionBase(QueryPartitionBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        public QueryPartitionBase(QueryBase query, Context context)
            : base(query)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
            this.query = query;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.id = 0;

            this.query = null;

            this.partitioningKeyFrom = double.NegativeInfinity;
            this.partitioningKeyTo = double.PositiveInfinity;

            this.tableSourceReferences = new Dictionary<string, TableReference>(SchemaManager.Comparer);
            this.remoteTableReferences = new Dictionary<string, TableReference>(SchemaManager.Comparer);
        }

        private void CopyMembers(QueryPartitionBase old)
        {
            this.id = old.id;

            this.query = old.query;

            this.partitioningKeyFrom = old.partitioningKeyFrom;
            this.partitioningKeyTo = old.partitioningKeyTo;

            this.tableSourceReferences = new Dictionary<string, TableReference>(old.tableSourceReferences, SchemaManager.Comparer);
            this.remoteTableReferences = new Dictionary<string, TableReference>(old.remoteTableReferences, SchemaManager.Comparer);
        }

        #endregion

        protected void LoadSystemDatabaseInstance(EntityReference<DatabaseInstance> databaseInstance, GraywulfDataset dataset, bool forceReinitialize)
        {
            if (!AssignedServerInstanceReference.IsEmpty && (databaseInstance.IsEmpty || forceReinitialize))
            {
                dataset.Context = Context;
                var dd = dataset.DatabaseVersionReference.Value.DatabaseDefinition;

                dd.LoadDatabaseInstances(false);
                foreach (var di in dd.DatabaseInstances.Values)
                {
                    di.Context = Context;
                }

                // Find database instance that is on the same machine
                try
                {
                    // TODO: only server instance and database definition is checked here, maybe database version would be better
                    databaseInstance.Value = dd.DatabaseInstances.Values.FirstOrDefault(ddi => ddi.ServerInstanceReference.Guid == AssignedServerInstance.Guid);
                    databaseInstance.Value.GetConnectionString();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        String.Format(
                            "Cannot find instance of system database: {0} ({1}) on server {2}/{3} ({4}).",
                            dataset.Name, dataset.DatabaseName,
                            AssignedServerInstance.Machine.Name, AssignedServerInstance.Name, AssignedServerInstance.GetCompositeName()),
                        ex); // TODO ***
                }
            }
            else if (AssignedServerInstanceReference.IsEmpty)
            {
                databaseInstance.Value = null;
            }
        }

        public SqlConnectionStringBuilder GetTemporaryDatabaseConnectionString()
        {
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    return new SqlConnectionStringBuilder(((SqlServerDataset)query.TemporaryDataset).ConnectionString);
                case ExecutionMode.Graywulf:
                    return TemporaryDatabaseInstanceReference.Value.GetConnectionString();
                default:
                    throw new NotImplementedException();
            }
        }

        public SqlServerDataset GetTemporaryDatabaseDataset()
        {
            SqlServerDataset tempds;

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    tempds = query.TemporaryDataset;
                    break;
                case ExecutionMode.Graywulf:
                    // *** TODO: this throws null exception after persist and restore
                    tempds = TemporaryDatabaseInstanceReference.Value.GetDataset();
                    break;
                default:
                    throw new NotImplementedException();
            }

            tempds.IsMutable = true;
            return tempds;
        }

        public Table GetTemporaryTable(string tableName)
        {
            string tempname;
            var tempds = GetTemporaryDatabaseDataset();

            switch (query.ExecutionMode)
            {
                case Jobs.Query.ExecutionMode.SingleServer:
                    tempname = String.Format("skyquerytemp_{0}_{1}", id.ToString(), tableName);
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    tempname = String.Format("{0}_{1}_{2}_{3}", Context.UserName, Context.JobID, id.ToString(), tableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new Table()
            {
                Dataset = tempds,
                DatabaseName = tempds.DatabaseName,
                SchemaName = tempds.DefaultSchemaName,
                TableName = tempname,
            };
        }

        public Table GetOutputTable()
        {
            return GetTemporaryTable("output"); // *** TODO
        }

        /// <summary>
        /// Interprets the parsed query
        /// </summary>
        protected override void FinishInterpret(bool forceReinitialize)
        {
            // --- Retrieve target table information
            var into = SelectStatement.FindDescendantRecursive<IntoClause>();
            if (into != null)
            {
                // remove into clause from query
                into.Parent.Stack.Remove(into);
            }

            // Cache table source references
            tableSourceReferences = new Dictionary<string, TableReference>();
            foreach (var tr in SelectStatement.EnumerateSourceTableReferences(true))
            {
                tableSourceReferences.Add(tr.UniqueName, tr);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public override void InitializeQueryObject(Context context, IScheduler scheduler, bool forceReinitialize)
        {
            base.InitializeQueryObject(context, scheduler, forceReinitialize);

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    break;
                case ExecutionMode.Graywulf:
                    LoadSystemDatabaseInstance(TemporaryDatabaseInstanceReference, (GraywulfDataset)query.TemporaryDataset, forceReinitialize);
                    LoadSystemDatabaseInstance(CodeDatabaseInstanceReference, (GraywulfDataset)query.CodeDataset, forceReinitialize);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Remote table caching functions

        private string EscapeIdentifierName(string name)
        {
            var res = name.Replace(".", "_");

            return res;
        }

        private string GetEscapedUniqueName(TableReference table)
        {
            if (table.IsSubquery || table.IsComputed)
            {
                return EscapeIdentifierName(table.Alias);
            }
            else
            {
                string res = String.Empty;

                if (table.DatasetName != null)
                {
                    res += String.Format("{0}_", EscapeIdentifierName(table.DatasetName));
                }

                if (table.DatabaseName != null)
                {
                    res += String.Format("{0}_", EscapeIdentifierName(table.DatabaseName));
                }

                if (table.SchemaName != null)
                {
                    res += String.Format("{0}_", EscapeIdentifierName(table.SchemaName));
                }

                if (table.DatabaseObjectName != null)
                {
                    res += String.Format("{0}", EscapeIdentifierName(table.DatabaseObjectName));
                }

                return res;
            }
        }

        /// <summary>
        /// Finds those tables that are required to execute the query but had to be
        /// copied from a remote source
        /// </summary>
        /// <returns></returns>
        public void FindRemoteTableReferences()
        {
            if (ExecutionMode == ExecutionMode.Graywulf /*&& query.CacheRemoteTables*/)
            {
                SchemaManager sc = GetSchemaManager();

                foreach (var tr in this.SelectStatement.EnumerateSourceTableReferences(true))
                {
                    if (tr.IsCachable && !RemoteTableReferences.ContainsKey(tr.UniqueName) &&
                        IsRemoteDataset(sc.Datasets[tr.DatasetName]))
                    {
                        RemoteTableReferences.Add(tr.UniqueName, tr);
                    }
                }
            }
            else
            {
                // nothing to do here
            }
        }

        /// <summary>
        /// Composes a source query for a remote table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public SourceTableQuery PrepareCopyRemoteTable(TableReference table)
        {
            // -- Load schema
            var sm = this.GetSchemaManager();
            var ds = sm.Datasets[table.DatasetName];

            // Graywulf dataset has to be converted to prevent registry access
            if (ds is GraywulfDataset)
            {
                ds = new SqlServerDataset(ds);
            }

            // --- Generate most restrictive query

            // Find the query specification this table belongs to
            var qs = ((TableSource)table.Node).QuerySpecification;

            // Run the normalizer to convert where clause to a normal form
            var cnr = new SearchConditionNormalizer();
            cnr.NormalizeQuerySpecification(qs);

            var cg = SqlCodeGeneratorFactory.CreateCodeGenerator(ds);
            var sql = cg.GenerateMostRestrictiveTableQuery(table, true, 0);

            return new SourceTableQuery()
            {
                Dataset = ds,
                Query = sql
            };
        }

        /// <summary>
        /// Copies a table from a remote data source by creating and
        /// executing a table copy task.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="source"></param>
        public void CopyRemoteTable(TableReference table, SourceTableQuery source)
        {
            // Create a target table name
            var temptable = GetTemporaryTable(GetEscapedUniqueName(table));
            TemporaryTables.TryAdd(table.UniqueName, temptable);

            var dest = new DestinationTable(
                temptable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create);

            var tc = CreateTableCopyTask(source, dest, false);

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, tc);

            tc.Execute();

            UnregisterCancelable(guid);
        }

        /// <summary>
        /// Checks if table is a remote table cached locally and if so,
        /// substitutes corresponding temp table name
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string SubstituteRemoteTableName(TableReference tr)
        {
            if (RemoteTableReferences.ContainsKey(tr.UniqueName))
            {
                return CodeGenerator.GetResolvedTableName(TemporaryTables[tr.UniqueName]);
            }
            else
            {
                return CodeGenerator.GetResolvedTableName(tr);
            }
        }

        #endregion
        #region Destination table functions and final query execution

        /// <summary>
        /// Generates the query that can be used to perform the final execution
        /// step.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetExecuteQueryText();

        private SourceTableQuery GetExecuteSourceQuery()
        {
            return new SourceTableQuery()
            {
                Dataset = GetTemporaryDatabaseDataset(),
                Query = GetExecuteQueryText()
            };
        }

        /// <summary>
        /// Generates the query that can be used to copy the results to the final
        /// destination table (usually in mydb)
        /// </summary>
        /// <returns></returns>
        protected virtual string GetOutputQueryText()
        {
            return String.Format(
                "SELECT __tablealias.* FROM {0} AS __tablealias",
                CodeGenerator.GetResolvedTableName(GetOutputTable()));
        }

        /// <summary>
        /// Gets a query that can be used to figure out the schema of
        /// the destination table.
        /// </summary>
        /// <returns></returns>
        private SourceTableQuery GetOutputSourceQuery()
        {
            return new SourceTableQuery()
            {
                Dataset = GetTemporaryDatabaseDataset(),
                Query = GetOutputQueryText()
            };
        }

        /// <summary>
        /// Creates or truncates destination table in the output database (usually MYDB)
        /// </summary>
        /// <remarks>
        /// This has to be in the QueryPartition class because the Query class does not
        /// have information about the database server the partition is executing on and
        /// the temporary tables are required to generate the destination table schema.
        /// 
        /// The destination table is created by the very first partition that gets to
        /// the point of copying results. This is when the name of the target table is
        /// determined in case only a table name pattern is specified and automatic
        /// unique naming is turned on.
        /// </remarks>
        public void PrepareDestinationTable(Context context, IScheduler scheduler)
        {
            switch (query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Output is already written to the target table
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    {
                        InitializeQueryObject(context, scheduler);

                        lock (query.syncRoot)
                        {
                            // Only initialize target table if it's still uninitialzed
                            if (!query.IsDestinationTableInitialized)
                            {
                                var source = GetOutputSourceQuery();

                                // TODO: figure out metadata from query
                                var table = query.Destination.GetTable(query.BatchName, query.QueryName, null, null);
                                table.Initialize(source.GetColumns(), query.Destination.Options);

                                // At this point the name of the destination is determined
                                // mark it as the output
                                query.Output = table;
                            }

                            query.IsDestinationTableInitialized = true;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public virtual void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            InitializeQueryObject(context, scheduler);
        }

        public void ExecuteQuery()
        {
            var source = GetExecuteSourceQuery();
            Table destination;

            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // In single-server mode results are directly written into destination table
                    destination = Query.Destination.GetTable();
                    break;
                case ExecutionMode.Graywulf:
                    // In graywulf mode results are written into a temporary table first
                    destination = GetOutputTable();
                    TemporaryTables.TryAdd(destination.TableName, destination);

                    // Drop destination table, in case it already exists for some reason
                    destination.Drop();
                    break;
                default:
                    throw new NotImplementedException();
            }

            ExecuteSelectInto(source, destination, Query.QueryTimeout);
        }

        public virtual void PrepareCopyResultset(Context context)
        {
            query.InitializeQueryObject(context);
            this.InitializeQueryObject(context);
        }

        /// <summary>
        /// Copies resultset from the output temporary table to the destination database (MYDB)
        /// </summary>
        public void CopyResultset()
        {
            switch (Query.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    // Do nothing as execute writes results directly into destination table
                    break;
                case ExecutionMode.Graywulf:
                    {
                        var source = GetOutputSourceQuery();

                        var destination = new DestinationTable(Query.Output, TableInitializationOptions.Append);

                        DumpSqlCommand(source.Query);

                        // Create bulk copy task and execute it
                        var tc = CreateTableCopyTask(source, destination, false);

                        var guid = Guid.NewGuid();
                        RegisterCancelable(guid, tc);

                        tc.Execute();

                        UnregisterCancelable(guid);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        #region Clean-up functions

        public void DropTemporaryTables(bool suppressErrors)
        {
            foreach (var table in TemporaryTables.Values)
            {
                // This function is called in the cancel branch of certain workflows
                // where it's not supposed to fail in any circumstances.

                try
                {
                    table.Drop();
                }
                catch (Exception)
                {
                    if (!suppressErrors)
                    {
                        throw;
                    }
                }
            }

            TemporaryTables.Clear();
        }

        public void DropTemporaryViews(bool suppressErrors)
        {
            foreach (var view in TemporaryViews.Values)
            {
                // This function is called in the cancel branch of certain workflows
                // where it's not supposed to fail in any circumstances.

                try
                {
                    view.Drop();
                }
                catch (Exception)
                {
                    if (!suppressErrors)
                    {
                        throw;
                    }
                }
            }

            TemporaryViews.Clear();
        }

        #endregion
        #region Query dump functions

        private string GetDumpFileName()
        {
            string server = GetTemporaryDatabaseConnectionString().DataSource;
            return String.Format("dump_{0}_{1}.sql", server, this.id);
        }

        protected void DumpSqlCommand(string sql)
        {
#if DUMPQUERIES
            string filename = GetDumpFileName();
            var sw = new StringWriter();

            // Time stamp
            sw.WriteLine("-- {0}\r\n", DateTime.Now);
            sw.WriteLine(sql);
            sw.WriteLine("GO");
            sw.WriteLine();

            File.AppendAllText(filename, sw.ToString());
#endif
        }

        protected void DumpSqlCommand(SqlCommand cmd)
        {
#if DUMPQUERIES
            var filename = GetDumpFileName();
            var sw = new StringWriter();

            // Time stamp
            sw.WriteLine("-- {0}\r\n", DateTime.Now);

            // Database name
            var csb = new SqlConnectionStringBuilder(cmd.Connection.ConnectionString);
            
            if (!String.IsNullOrWhiteSpace(csb.InitialCatalog))
            {
                sw.WriteLine("USE [{0}]", csb.InitialCatalog);
                sw.WriteLine("GO");
                sw.WriteLine();
            }

            // Command parameters
            foreach (SqlParameter par in cmd.Parameters)
            {
                sw.WriteLine(String.Format("DECLARE {0} {1} = {2}",
                    par.ParameterName,
                    par.SqlDbType.ToString(),
                    par.Value.ToString()));
            }

            sw.WriteLine(cmd.CommandText);
            sw.WriteLine("GO");

            File.AppendAllText(filename, sw.ToString());
#endif
        }

        #endregion
        #region Actual query execution functions

        protected void ExecuteSqlCommandOnTemporaryDatabase(string sql)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.CommandText = sql;
                ExecuteSqlCommandOnTemporaryDatabase(cmd);
            }
        }

        protected void ExecuteSqlCommandOnTemporaryDatabase(SqlCommand cmd)
        {
            var csb = GetTemporaryDatabaseConnectionString();

            using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                cmd.Connection = cn;
                cmd.CommandTimeout = query.QueryTimeout;

                DumpSqlCommand(cmd);

                ExecuteLongCommandNonQuery(cmd);
            }
        }

        protected object ExecuteSqlCommandOnTemporaryDatabaseScalar(SqlCommand cmd)
        {

            var csb = GetTemporaryDatabaseConnectionString();

            using (var cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                cmd.Connection = cn;
                cmd.CommandTimeout = query.QueryTimeout;

                DumpSqlCommand(cmd);

                return ExecuteLongCommandScalar(cmd);
            }
        }

        #endregion

        protected SearchCondition GetPartitioningConditions(string column, double buffering)
        {
            string format;

            if (!double.IsInfinity(PartitioningKeyFrom) && !double.IsInfinity(PartitioningKeyTo))
            {
                format = "({1} <= {0} AND {0} < {2})";
            }
            else if (!double.IsInfinity(PartitioningKeyTo))
            {
                format = "({0} < {2})";
            }
            else if (!double.IsInfinity(PartitioningKeyFrom))
            {
                format = "({1} <= {0})";
            }
            else
            {
                return null;
            }

            string sql = String.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                format,
                column,
                PartitioningKeyFrom - buffering,
                PartitioningKeyTo + buffering);

            var parser = new Jhu.Graywulf.SqlParser.SqlParser();
            var sc = (SearchCondition)parser.Execute(new SearchCondition(), sql);

            return sc;
        }
    }
}
