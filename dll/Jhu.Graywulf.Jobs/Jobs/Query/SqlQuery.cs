using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.CodeGeneration;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public class SqlQuery : QueryObject, ICloneable
    {
        #region Property storage member variables

        /// <summary>
        /// Destination tables including target table naming patterns.
        /// Output table names are either automatically generated or
        /// taken from the INTO clauses.
        /// </summary>
        private DestinationTable destination;

        /// <summary>
        /// Holds the individual partitions. Usually many, but for simple queries
        /// only one.
        /// </summary>
        private List<SqlQueryPartition> partitions;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination table naming pattern of the query
        /// </summary>
        [DataMember]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        
        [IgnoreDataMember]
        public List<SqlQueryPartition> Partitions
        {
            get { return partitions; }
        }

        #endregion
        #region Constructors and initializer

        public SqlQuery()
        {
            InitializeMembers(new StreamingContext());
        }

        protected SqlQuery(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQuery(CancellationContext cancellationContext, RegistryContext registryContext)
            : base(cancellationContext, registryContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQuery(SqlQuery old)
            : base(old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.destination = null;
            this.partitions = new List<SqlQueryPartition>();
        }

        private void CopyMembers(SqlQuery old)
        {
            this.destination = old.destination;
            this.partitions = new List<SqlQueryPartition>(old.partitions.Select(p => (SqlQueryPartition)p.Clone()));
        }

        public override object Clone()
        {
            return new SqlQuery(this);
        }

        #endregion
        #region Query parsing and interpretation

        public virtual void Verify()
        {
            // TODO: this is used to validate query before scheduling
            // this needs to be merged with with InitializeQuery.
            // Also: add mechanism to collect validation messages
            InitializeQueryObject(null, null, null, true);
        }

        public override void Validate()
        {
            base.Validate();

            // Perform validation on the query string
            var validator = QueryFactory.CreateValidator();

            // TODO: run it on the info class
            validator.Execute(QueryDetails.ParsingTree);

            // TODO: add additional validation here
            Destination.CheckTableExistence();
        }


        #endregion
        #region Table statistics

        /// <summary>
        /// Collects tables for which statistics should be computed before
        /// executing the query
        /// </summary>
        /// <returns></returns>
        public virtual void IdentifyTablesForStatistics()
        {
            TableStatistics.Clear();

            // TODO: add multi-statement support
            // TODO: move all this to name resolver

            if (QueryDetails.IsPartitioned)
            {
                // Partitioning is always done on the table specified right after the FROM keyword
                // TODO: what if more than one QS?
                var qs = QueryDetails.ParsingTree.FindDescendantRecursive<QueryExpression>().EnumerateQuerySpecifications().FirstOrDefault();
                var ts = (SimpleTableSource)qs.EnumerateSourceTables(false).First();

                // TODO: modify this when expression output type functions are implemented
                // and figure out data type directly from expression
                var stat = new TableStatistics()
                {
                    KeyColumn = ts.PartitioningKeyExpression,
                    KeyColumnDataType = ts.PartitioningKeyDataType,
                };

                TableStatistics.Add(ts, stat);
            }
        }

        public Jhu.Graywulf.Schema.DatasetBase GetStatisticsDataset(ITableSource tableSource)
        {
            if (!String.IsNullOrEmpty(StatDatabaseVersionName))
            {
                var nts = (ITableSource)tableSource.Clone();
                var sm = GetSchemaManager();
                var ds = sm.Datasets[tableSource.TableReference.DatasetName];

                if (ds is GraywulfDataset)
                {
                    var dd = ((GraywulfDataset)ds).DatabaseDefinitionReference.Value;
                    var sis = GetAvailableDatabaseInstances(AssignedServerInstance, dd, StatDatabaseVersionName, SourceDatabaseVersionName);

                    if (sis.Length > 0)
                    {
                        var nds = sis[0].GetDataset();
                        return nds;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gather statistics for the table with the specified bin size
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="binSize"></param>
        public async Task ComputeTableStatisticsAsync(ITableSource tableSource, DatasetBase statisticsDataset)
        {
            var stat = TableStatistics[tableSource];
            var cg = CreateCodeGenerator();

            using (var cmd = cg.GetTableStatisticsCommand(tableSource, statisticsDataset))
            {
                await ExecuteSqlOnAssignedServerReaderAsync(cmd, CommandTarget.Code, async (dr, ct) =>
                {
                    long rc = 0;
                    while (await dr.ReadAsync(ct))
                    {
                        stat.KeyCount.Add(dr.GetInt64(0));
                        stat.KeyValue.Add((IComparable)dr.GetValue(1));

                        rc = dr.GetInt64(0);    // the very last value will give row count
                    }
                    stat.RowCount = rc;
                });
            }
        }

        #endregion
        #region Query partitioning

        protected virtual SqlQueryCodeGenerator CreateCodeGenerator()
        {
            return new SqlQueryCodeGenerator(this);
        }

        protected virtual SqlQueryPartition CreatePartition()
        {
            return new SqlQueryPartition(this);
        }

        private int DeterminePartitionCount()
        {
            int partitionCount = 1;

            switch (ExecutionMode)
            {
                case Query.ExecutionMode.SingleServer:
                    break;
                case Query.ExecutionMode.Graywulf:
                    // Single server mode will run on one partition by definition,
                    // Graywulf mode has to look at the registry for available machines
                    // If query is partitioned, statistics must be gathered
                    if (QueryDetails.IsPartitioned)
                    {
                        var mirroredDatasets = FindMirroredGraywulfDatasets().Values.Select(i => i.DatabaseDefinitionReference.Value).ToArray();
                        var specificDatasets = FindServerSpecificGraywulfDatasets().Values.Select(i => i.DatabaseInstanceReference.Value).ToArray();

                        if (mirroredDatasets.Length == 0)
                        {
                            partitionCount = 1;
                        }
                        else
                        {
                            // *** TODO: find optimal number of partitions
                            // TODO: replace "4" with a value from settings
                            var sis = GetAvailableServerInstances(mirroredDatasets, SourceDatabaseVersionName, null, specificDatasets);
                            partitionCount = 4 * sis.Length;

                            if (MaxPartitions > 0)
                            {
                                partitionCount = Math.Max(partitionCount, MaxPartitions);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            return partitionCount;
        }

        public void GeneratePartitions()
        {
            // Partitioning is only supperted using Graywulf mode, single server mode always
            // falls back to a single partition

            int partitionCount = DeterminePartitionCount();

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        OnGeneratePartitions(1, null);
                    }
                    break;
                case ExecutionMode.Graywulf:
                    if (!QueryDetails.IsPartitioned)
                    {
                        OnGeneratePartitions(1, null);
                    }
                    else
                    {
                        // See if maxmimum number of partitions is limited
                        if (MaxPartitions != 0)
                        {
                            partitionCount = Math.Min(partitionCount, MaxPartitions);
                        }

                        // Determine partition limits based on the first table's statistics
                        // In certaint cases all tables of the query are remote tables which
                        // means no statistics are generated at all. In this case a single
                        // partition will be used.
                        if (TableStatistics == null || TableStatistics.Count == 0)
                        {
                            OnGeneratePartitions(1, null);
                        }
                        else
                        {
                            // TODO: how to pick the first table? What's the first table?

                            var stat = TableStatistics.Values.FirstOrDefault();
                            OnGeneratePartitions(partitionCount, stat);
                        }
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
        protected virtual void OnGeneratePartitions(int partitionCount, TableStatistics stat)
        {
            // TODO: fix issue with repeating keys!
            // Maybe just throw those partitions away?

            SqlQueryPartition qp = null;


            if (stat == null || stat.KeyValue.Count / partitionCount == 0)
            {
                qp = CreatePartition();
                AppendPartition(qp);
            }
            else
            {
                int s = stat.KeyValue.Count / partitionCount;

                for (int i = 0; i < partitionCount; i++)
                {
                    qp = CreatePartition();
                    qp.PartitioningKeyMax = stat.KeyValue[Math.Min((i + 1) * s, stat.KeyValue.Count - 1)];

                    if (i == 0)
                    {
                        qp.PartitioningKeyMin = null;
                    }
                    else
                    {
                        qp.PartitioningKeyMin = Partitions[i - 1].PartitioningKeyMax;
                    }

                    AppendPartition(qp);
                }

                Partitions[Partitions.Count - 1].PartitioningKeyMax = null;
            }
        }

        protected void AppendPartition(SqlQueryPartition partition)
        {
            partition.ID = partitions.Count;
            partitions.Add(partition);
        }

        #endregion
        #region Temporary table logic

        public override Table GetTemporaryTable(string tableName)
        {
            string tempname;

            switch (ExecutionMode)
            {
                case Jobs.Query.ExecutionMode.SingleServer:
                    tempname = String.Format("temp_{0}", tableName);
                    break;
                case Jobs.Query.ExecutionMode.Graywulf:
                    tempname = String.Format("{0}_{1}_{2}", RegistryContext.User.Name, JobContext.Current.JobID, tableName);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return GetTemporaryTableInternal(tempname);
        }

        #endregion
    }
}
