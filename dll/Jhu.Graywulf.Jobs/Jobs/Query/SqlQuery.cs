using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public class SqlQuery : QueryObject, ICloneable
    {
        #region Property storage member variables

        /// <summary>
        /// Destination table including target table naming pattern.
        /// Output table names are either automatically generater or
        /// taken from the INTO clause.
        /// </summary>
        private DestinationTable destination;

        /// <summary>
        /// If true, query destination table has already been initialized.
        /// TODO: this will need to be changed for multi-select queries
        /// </summary>
        private bool isDestinationTableInitialized;

        /// <summary>
        /// Points to the output table of the query.
        /// </summary>
        private Table output;

        /// <summary>
        /// Database version to be used to execute the queries (HOT)
        /// </summary>
        private string sourceDatabaseVersionName;

        /// <summary>
        /// Database version to be used to calculate statistics (STAT)
        /// </summary>
        private string statDatabaseVersionName;

        /// <summary>
        /// Holds table statistics gathered for all the tables in the query
        /// </summary>
        private List<ITableSource> tableSourceStatistics;

        /// <summary>
        /// Holds the individual partitions. Usually many, but for simple queries
        /// only one.
        /// </summary>
        private List<SqlQueryPartition> partitions;

        /// <summary>
        /// Name of the table used for partitioning
        /// TODO: use Table class or TableReference here!
        /// </summary>
        [NonSerialized]
        private string partitioningTable;

        /// <summary>
        /// Name of the column to partition on
        /// TODO: use ColumnReference here
        /// </summary>
        [NonSerialized]
        private string partitioningKey;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination table of the query
        /// </summary>
        [DataMember]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets whether the destination table is initialized.
        /// </summary>
        [IgnoreDataMember]
        public bool IsDestinationTableInitialized
        {
            get { return isDestinationTableInitialized; }
            internal set { isDestinationTableInitialized = value; }
        }

        [DataMember]
        public Table Output
        {
            get { return output; }
            set { output = value; }
        }

        [DataMember]
        public string SourceDatabaseVersionName
        {
            get { return sourceDatabaseVersionName; }
            set { sourceDatabaseVersionName = value; }
        }

        [DataMember]
        public string StatDatabaseVersionName
        {
            get { return statDatabaseVersionName; }
            set { statDatabaseVersionName = value; }
        }

        [IgnoreDataMember]
        public List<ITableSource> TableSourceStatistics
        {
            get { return tableSourceStatistics; }
        }

        [IgnoreDataMember]
        public List<SqlQueryPartition> Partitions
        {
            get { return partitions; }
        }

        /// <summary>
        /// Gets whether the query is partitioned
        /// </summary>
        [IgnoreDataMember]
        public bool IsPartitioned
        {
            get { return SelectStatement.IsPartitioned; }
        }

        #endregion
        #region Constructors and initializer

        protected SqlQuery()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQuery(SqlQuery old)
            : base(old)
        {
            CopyMembers(old);
        }

        public SqlQuery(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.destination = null;
            this.isDestinationTableInitialized = false;
            this.output = null;

            this.sourceDatabaseVersionName = String.Empty;
            this.statDatabaseVersionName = String.Empty;

            this.tableSourceStatistics = new List<ITableSource>();
            this.partitions = new List<SqlQueryPartition>();

            this.partitioningTable = null;
            this.partitioningKey = null;
        }

        private void CopyMembers(SqlQuery old)
        {
            this.destination = old.destination;
            this.isDestinationTableInitialized = old.isDestinationTableInitialized;
            this.output = old.output;

            this.sourceDatabaseVersionName = old.sourceDatabaseVersionName;
            this.statDatabaseVersionName = old.statDatabaseVersionName;

            this.tableSourceStatistics = new List<ITableSource>();
            this.partitions = new List<SqlQueryPartition>(old.partitions.Select(p => (SqlQueryPartition)p.Clone()));

            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
        }

        public override object Clone()
        {
            return new SqlQuery(this);
        }

        #endregion
        #region Query interpretation and validation

        protected override void FinishInterpret(bool forceReinitialize)
        {
            // --- Retrieve target table information
            IntoClause into = SelectStatement.FindDescendantRecursive<IntoClause>();
            if (into != null)
            {

                // **** TODO: test this with dataset name
                //if (into.TableReference.DatasetName != null) this.destinationTable.Table.Dataset.Name = into.TableReference.DatasetName;

                if (into.TableReference.SchemaName != null)
                {
                    this.destination.SchemaName = into.TableReference.SchemaName;
                }

                if (into.TableReference.DatabaseObjectName != null)
                {
                    this.destination.TableNamePattern = into.TableReference.DatabaseObjectName;
                }

                // Turn off unique name generation in case an into clause is used
                this.destination.Options &= ~TableInitializationOptions.GenerateUniqueName;

                // remove into clause from query
                into.Parent.Stack.Remove(into);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public virtual void Verify()
        {
            Parse(true);
            Interpret(true);
            Validate();

            Destination.CheckTableExistence();
        }


        #endregion
        #region Table statistics

        /// <summary>
        /// Collects tables for which statistics should be computed before
        /// executing the query
        /// </summary>
        /// <returns></returns>
        public virtual void CollectTablesForStatistics()
        {
            TableSourceStatistics.Clear();

            if (IsPartitioned)
            {
                // Partitioning is always done on the table specified right after the FROM keyword
                // TODO: what if more than one QS?
                // *** TODO: test this here, will not work with functions etc!
                var qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault();
                var ts = (SimpleTableSource)qs.EnumerateSourceTables(false).First();
                var tr = ts.TableReference;

                tr.Statistics = new SqlParser.TableStatistics();
                tr.Statistics.KeyColumn = CodeGenerator.GetResolvedColumnName(ts.PartitioningColumnReference);
                tr.Statistics.KeyColumnDataType = ts.PartitioningColumnReference.DataType;

                TableSourceStatistics.Add(ts);
            }
        }

        /// <summary>
        /// Gets a connection string and a query to compute table statistics.
        /// </summary>
        /// <remarks>
        /// The server to run the statistics is chosen by the scheduler based on
        /// database availability.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="tableSource"></param>
        /// <param name="connectionString"></param>
        /// <param name="cmd"></param>
        /// <param name="multiplier"></param>
        public void PrepareComputeTableStatistics(Context context, ITableSource tableSource, out string connectionString, out SqlCommand cmd, out int multiplier)
        {
            // Assign a database server to the query
            var sm = GetSchemaManager();
            var ds = sm.Datasets[tableSource.TableReference.DatasetName];

            if (ds is GraywulfDataset && !((GraywulfDataset)ds).IsSpecificInstanceRequired)
            {
                // Use the MINI version of the database definition to get statistics

                var gds = (GraywulfDataset)ds;
                var dd = new DatabaseDefinition(context);
                dd.Guid = gds.DatabaseDefinitionReference.Guid;
                dd.Load();

                // Get a server from the scheduler
                var si = GetNextServerInstance(dd, StatDatabaseVersionName, SourceDatabaseVersionName);
                AssignServer(si);

                connectionString = si.GetConnectionString().ConnectionString;

                SubstituteDatabaseName(tableSource.TableReference, si, StatDatabaseVersionName, SourceDatabaseVersionName);

                // TODO: multiplier depends on whether statistics were gathered from the
                // sample table or a full table of the surrogate full database
                multiplier = 10000; // TODO: MINI version is sampled 10000 to 1 but not always :-(
            }
            else if (ds is GraywulfDataset)
            {
                // TODO: test!
                var gds = (GraywulfDataset)ds;
                var si = gds.DatabaseInstanceReference.Value.ServerInstance;
                AssignServer(si);

                connectionString = ds.ConnectionString;
                multiplier = 1;
            }
            else
            {
                // Run it on the specific database
                connectionString = ds.ConnectionString;
                multiplier = 1;
            }

            // Generate statistics query
            cmd = CodeGenerator.GetTableStatisticsCommand(tableSource);
        }

        /// <summary>
        /// Gather statistics for the table with the specified bin size
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="binSize"></param>
        public void ComputeTableStatistics(ITableSource tableSource, string connectionString, SqlCommand cmd, int multiplier)
        {
            var stat = tableSource.TableReference.Statistics;

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                cmd.Connection = cn;
                cmd.Parameters.Add("@BinCount", SqlDbType.Int).Value = stat.BinCount;
                cmd.CommandTimeout = QueryTimeout;

                ExecuteSqlCommandReader(cmd, CommandTarget.Code, dr =>
                {
                    long rc = 0;
                    while (dr.Read())
                    {
                        stat.KeyCount.Add(dr.GetInt64(0));
                        stat.KeyValue.Add((IComparable)dr.GetValue(1));

                        rc = dr.GetInt64(0);    // the very last value will give row count
                    }
                    stat.RowCount = rc * multiplier;
                });

                cmd.Dispose();
            }
        }

        #endregion
        #region Query partitioning

        public void DeterminePartitionCount(Context context, IScheduler scheduler, out int partitionCount, out Guid assignedServerInstanceGuid)
        {
            partitionCount = 1;
            assignedServerInstanceGuid = Guid.Empty;

            // Single server mode will run on one partition by definition,
            // Graywulf mode has to look at the registry for available machines
            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    InitializeQueryObject(null);
                    break;
                case ExecutionMode.Graywulf:
                    InitializeQueryObject(context, scheduler, false);

                    // If query is partitioned, statistics must be gathered
                    if (IsPartitioned)
                    {
                        // Assign a server that will run the statistics queries
                        // Try to find a server that contains all required datasets. This is true right now for
                        // SkyQuery where all databases are mirrored but will have to be updated later

                        // Collect all datasets that are required to answer the query
                        var dss = FindRequiredDatasets();

                        // Datasets that are mirrored and can be on any server
                        var reqds = (from ds in dss.Values
                                     where !ds.IsSpecificInstanceRequired
                                     select ds.DatabaseDefinitionReference.Value);

                        // Datasets that are only available at a specific server instance
                        var spds = (from ds in dss.Values
                                    where ds.IsSpecificInstanceRequired && !ds.DatabaseInstanceReference.IsEmpty
                                    select ds.DatabaseInstanceReference.Value);


                        var si = GetNextServerInstance(reqds, SourceDatabaseVersionName, null, spds);

                        AssignServer(si);
                        assignedServerInstanceGuid = si.Guid;

                        // *** TODO: find optimal number of partitions
                        // TODO: replace "4" with a value from settings
                        var sis = GetAvailableServerInstances(reqds, SourceDatabaseVersionName, null, spds);
                        partitionCount = 4 * sis.Length;

                        // Now have to reinitialize to load the assigned server instances
                        InitializeQueryObject(context, scheduler, true);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual SqlQueryPartition CreatePartition(SqlQuery query, Context context)
        {
            return new SqlQueryPartition((SqlQuery)query, context);
        }

        public virtual void GeneratePartitions(int partitionCount)
        {
            // Partitioning is only supperted using Graywulf mode, single server mode always
            // falls back to a single partition

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        var sqp = CreatePartition(this, null);
                        AppendPartition(sqp);
                    }
                    break;
                case ExecutionMode.Graywulf:
                    if (!SelectStatement.IsPartitioned)
                    {
                        var sqp = CreatePartition(this, this.Context);
                        AppendPartition(sqp);
                    }
                    else
                    {
                        // --- determine partition limits based on the first table's statistics
                        if (TableSourceStatistics == null || TableSourceStatistics.Count == 0)
                        {
                            throw new InvalidOperationException();
                        }

                        var stat = TableSourceStatistics[0].TableReference.Statistics;
                        GeneratePartitionsInternal(partitionCount, stat);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Generate partitions based on table statistics.
        /// </summary>
        /// <param name="partitionCount"></param>
        /// <param name="stat"></param>
        private void GeneratePartitionsInternal(int partitionCount, SqlParser.TableStatistics stat)
        {
            // TODO: fix issue with repeating keys!
            // Maybe just throw those partitions away?

            SqlQueryPartition qp = null;
            int s = stat.KeyValue.Count / partitionCount;

            if (s == 0)
            {
                qp = (SqlQueryPartition)CreatePartition(this, this.Context);

                AppendPartition(qp);
            }
            else
            {
                for (int i = 0; i < partitionCount; i++)
                {
                    qp = (SqlQueryPartition)CreatePartition(this, this.Context);
                    qp.PartitioningKeyTo = stat.KeyValue[Math.Min((i + 1) * s, stat.KeyValue.Count - 1)];

                    if (i == 0)
                    {
                        qp.PartitioningKeyFrom = null;
                    }
                    else
                    {
                        qp.PartitioningKeyFrom = Partitions[i - 1].PartitioningKeyTo;
                    }

                    AppendPartition(qp);
                }

                Partitions[Partitions.Count - 1].PartitioningKeyTo = null;
            }
        }

        protected void AppendPartition(SqlQueryPartition partition)
        {
            partition.ID = partitions.Count;
            partitions.Add(partition);
        }

        #endregion
    }
}
