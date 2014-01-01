﻿using System;
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
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public abstract class QueryBase : QueryObject
    {
        #region Property storage member variables

        private int queryTimeout;

        private DestinationTable destination;
        private bool isDestinationTableInitialized;

        private string sourceDatabaseVersionName;
        private string statDatabaseVersionName;

        private List<TableReference> tableStatistics;
        private List<QueryPartitionBase> partitions;

        #endregion
        #region Member variables

        [NonSerialized]
        private EntityProperty<DatabaseInstance> destinationDatabaseInstance;

        [NonSerialized]
        private string partitioningTable;
        [NonSerialized]
        private string partitioningKey;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the timeout of individual queries
        /// </summary>
        /// <remarks>
        /// The overall timeout period is set by the timeout of the job.
        /// </remarks>
        [DataMember]
        public int QueryTimeout
        {
            get { return queryTimeout; }
            set { queryTimeout = value; }
        }

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
        public List<TableReference> TableStatistics
        {
            get { return tableStatistics; }
        }

        [IgnoreDataMember]
        public List<QueryPartitionBase> Partitions
        {
            get { return partitions; }
        }

        [IgnoreDataMember]
        public abstract bool IsPartitioned
        {
            get;
        }

        #endregion
        #region Constructors and initializers

        protected QueryBase()
        {
            InitializeMembers(new StreamingContext());
        }

        protected QueryBase(QueryBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        public QueryBase(Context context)
        {
            InitializeMembers(new StreamingContext());

            this.Context = context;
        }


        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.queryTimeout = 60; // TODO ***

            this.destination = null;
            this.isDestinationTableInitialized = false;

            this.sourceDatabaseVersionName = String.Empty;
            this.statDatabaseVersionName = String.Empty;

            this.tableStatistics = new List<TableReference>();
            this.partitions = new List<QueryPartitionBase>();

            this.destinationDatabaseInstance = new EntityProperty<DatabaseInstance>();

            this.partitioningTable = null;
            this.partitioningKey = null;
        }

        private void CopyMembers(QueryBase old)
        {
            this.queryTimeout = old.queryTimeout;

            this.destination = old.destination;
            this.isDestinationTableInitialized = old.isDestinationTableInitialized;

            this.sourceDatabaseVersionName = old.sourceDatabaseVersionName;
            this.statDatabaseVersionName = old.statDatabaseVersionName;

            this.tableStatistics = new List<TableReference>();  // ***
            this.partitions = new List<QueryPartitionBase>(old.partitions.Select(p => (QueryPartitionBase)p.Clone()));

            this.destinationDatabaseInstance = new EntityProperty<DatabaseInstance>(old.destinationDatabaseInstance);

            this.partitioningTable = old.partitioningTable;
            this.partitioningKey = old.partitioningKey;
        }

        #endregion
        #region Parsing functions

        public void Verify()
        {
            Parse(true);
            Interpret(true);
            Validate();
        }

        protected override void UpdateContext()
        {
            base.UpdateContext();

            if (this.destinationDatabaseInstance != null) this.destinationDatabaseInstance.Context = Context;
        }

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
                    this.destination.TableName = into.TableReference.DatabaseObjectName;
                }

                // remove into clause from query
                into.Parent.Stack.Remove(into);
            }

            base.FinishInterpret(forceReinitialize);
        }

        public override void InitializeQueryObject(Context context, IScheduler scheduler, bool forceReinitialize)
        {
            if (context != null)
            {
                this.Context = context;
                UpdateContext();
            }

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    break;
                case ExecutionMode.Graywulf:
                    LoadDestinationDatabaseInstance(forceReinitialize);
                    break;
                default:
                    throw new NotImplementedException();
            }

            base.InitializeQueryObject(context, scheduler, forceReinitialize);
        }

        protected void AssertValidContext()
        {
            // This call requires a live context
            System.Diagnostics.Debug.Assert(this.ExecutionMode != ExecutionMode.Graywulf || Context != null && Context.IsValid);
        }

        #endregion
        #region Table statistics

        /// <summary>
        /// When overriden, returns tables for which statistics should be collected before
        /// executing the query
        /// </summary>
        /// <returns></returns>
        public abstract void CollectTablesForStatistics();

        public void PrepareComputeTableStatistics(Context context, TableReference tr, out string connectionString, out string sql)
        {
            // Assign a database server to the query
            // TODO: maybe make this function generic
            // TODO: check this part to use appropriate server and database
            var sm = GetSchemaManager(false);
            var ds = sm.Datasets[tr.DatasetName];

            if (ds is GraywulfDataset && !((GraywulfDataset)ds).IsSpecificInstanceRequired)
            {
                var gds = (GraywulfDataset)ds;
                var dd = new DatabaseDefinition(context);
                dd.Guid = gds.DatabaseDefinition.Guid;
                dd.Load();

                // Get a server from the scheduler
                var si = new ServerInstance(Context);
                si.Guid = Scheduler.GetNextServerInstance(new Guid[] { dd.Guid }, StatDatabaseVersionName, null);
                si.Load();

                connectionString = si.GetConnectionString().ConnectionString;

                SubstituteDatabaseName(tr, si.Guid, StatDatabaseVersionName);
                tr.DatabaseObject = null;
            }
            else
            {
                // Run it on the specific database
                connectionString = ds.ConnectionString;
            }

            // Generate statistics query
            var cg = new SqlServerCodeGenerator();
            cg.ResolveNames = true;
            sql = cg.GenerateTableStatisticsQuery(tr);
        }

        /// <summary>
        /// Gather statistics for the table with the specified bin size
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="binSize"></param>
        public virtual void ComputeTableStatistics(TableReference tr, string connectionString, string sql)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@BinCount", SqlDbType.Decimal).Value = tr.Statistics.BinCount;
                    cmd.CommandTimeout = queryTimeout;

                    ExecuteLongCommandReader(cmd, dr =>
                    {
                        long rc = 0;
                        while (dr.Read())
                        {
                            tr.Statistics.KeyValue.Add(dr.GetDouble(0));
                            tr.Statistics.KeyCount.Add(dr.GetInt64(1));

                            rc += dr.GetInt64(1);
                        }
                        tr.Statistics.RowCount = rc;
                    });
                }
            }
        }

        #endregion
        #region Query partitioning functions

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
                    InitializeQueryObject(context, scheduler);

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
                                     select ds.DatabaseDefinition.Guid).ToArray();

                        // Datasets that are only available at a specific server instance
                        var spds = (from ds in dss.Values
                                    where ds.IsSpecificInstanceRequired && !ds.DatabaseInstance.IsEmpty
                                    select ds.DatabaseInstance.Guid).ToArray();


                        var si = new ServerInstance(context);
                        si.Guid = scheduler.GetNextServerInstance(reqds, StatDatabaseVersionName, spds);
                        si.Load();

                        AssignedServerInstance = si;
                        assignedServerInstanceGuid = si.Guid;

                        // *** TODO: find optimal number of partitions
                        // TODO: replace "2" with a value from settings
                        partitionCount = 2 * scheduler.GetServerInstances(reqds, SourceDatabaseVersionName, spds).Length;

                        // Now have to reinitialize to load the assigned server instances
                        InitializeQueryObject(context, scheduler, true);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public abstract void GeneratePartitions(int availableServerCount);

        protected void AppendPartition(QueryPartitionBase partition)
        {
            partition.ID = partitions.Count;
            partitions.Add(partition);
        }

        #endregion
        #region Query execution functions

        public void LoadDestinationDatabaseInstance(bool forceReinitialize)
        {
            if (destinationDatabaseInstance.IsEmpty || forceReinitialize)
            {
                var dd = (GraywulfDataset)destination.Dataset;
                if (destinationDatabaseInstance.IsEmpty && !dd.DatabaseInstance.IsEmpty)
                {
                    dd.Context = Context;
                    destinationDatabaseInstance.Value = dd.DatabaseInstance.Value;
                }

                destinationDatabaseInstance.Value.GetConnectionString();
            }
        }

        public virtual void CheckDestinationTable()
        {
            AssertValidContext();

            var table = destination.GetTable();
            var exists = table.IsExisting;

            if (exists && (destination.Options & TableInitializationOptions.Drop) == 0)
            {
                if ((destination.Options & TableInitializationOptions.Create) == 0)
                {
                    throw new Exception("Output table already exists.");    // *** TODO
                }
            }
        }

        #endregion
    }
}
