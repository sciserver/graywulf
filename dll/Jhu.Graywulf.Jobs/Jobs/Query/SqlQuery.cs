using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;
using gw = Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    [DataContract(Name = "Query", Namespace = "")]
    public class SqlQuery : QueryBase
    {
        #region Properties

        /// <summary>
        /// Gets whether the query is partitioned
        /// </summary>
        [IgnoreDataMember]
        public override bool IsPartitioned
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

        public SqlQuery(gw.Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(SqlQuery old)
        {
        }

        public override object Clone()
        {
            return new SqlQuery(this);
        }

        #endregion

        public override void Verify()
        {
            base.Verify();

            Destination.CheckTableExistence();
        }

        #region Table statistics

        public override void CollectTablesForStatistics()
        {
            TableStatistics.Clear();

            if (IsPartitioned)
            {
                // Partitioning is always done on the table specified right after the FROM keyword
                // TODO: what if more than one QS?
                // *** TODO: test this here, will not work with functions etc!
                var qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault();
                var ts = (SimpleTableSource)qs.EnumerateSourceTables(false).First();
                var tr = ts.TableReference;

                tr.Statistics = new SqlParser.TableStatistics(ts.TableReference);
                tr.Statistics.KeyColumn = CodeGenerator.GetResolvedColumnName(ts.PartitioningColumnReference);
                tr.Statistics.KeyColumnDataType = ts.PartitioningColumnReference.DataType;

                TableStatistics.Add(ts.TableReference);
            }
        }

        #endregion

        public override void GeneratePartitions(int partitionCount)
        {
            // Partitioning is only supperted using Graywulf mode, single server mode always
            // falls back to a single partition

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        var sqp = new SqlQueryPartition(this, null);
                        AppendPartition(sqp);
                    }
                    break;
                case ExecutionMode.Graywulf:
                    if (!SelectStatement.IsPartitioned)
                    {
                        var sqp = new SqlQueryPartition(this, this.Context);
                        AppendPartition(sqp);
                    }
                    else
                    {
                        // --- determine partition limits based on the first table's statistics
                        if (TableStatistics == null || TableStatistics.Count == 0)
                        {
                            throw new InvalidOperationException();
                        }

                        var stat = TableStatistics[0].Statistics;
                        GeneratePartitions(partitionCount, stat);
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
        private void GeneratePartitions(int partitionCount, SqlParser.TableStatistics stat)
        {
            SqlQueryPartition qp = null;
            int s = stat.KeyValue.Count / partitionCount;

            if (s == 0)
            {
                qp = new SqlQueryPartition(this, this.Context);

                AppendPartition(qp);
            }
            else
            {
                for (int i = 0; i < partitionCount; i++)
                {
                    qp = new SqlQueryPartition(this, this.Context);
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
    }
}
