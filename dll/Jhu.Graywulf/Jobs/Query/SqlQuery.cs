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
        [IgnoreDataMember]
        public override bool IsPartitioned
        {
            get { return SelectStatement.IsPartitioned; }
        }

        protected SqlQuery()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQuery(SqlQuery old)
            :base(old)
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

        public override void GeneratePartitions(int partitionCount)
        {
            // Partitioning is only supperted using Graywulf mode, single server mode always
            // falls back to a single partition

            switch (ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    {
                        SqlQueryPartition sqp = new SqlQueryPartition(this, null);
                        sqp.ID = 0;
                        AppendPartition(sqp);
                    }
                    break;
                case ExecutionMode.Graywulf:
                    if (!SelectStatement.IsPartitioned)
                    {
                        SqlQueryPartition sqp = new SqlQueryPartition(this, this.Context);

                        sqp.ID = 0;

                        AppendPartition(sqp);
                    }
                    else
                    {
                        // --- compute table statistics
                        decimal binsize = 1.0m;   // histogram bin size in Dec
                        TableStatistics stat;

                        QuerySpecification qs = SelectStatement.EnumerateQuerySpecifications().FirstOrDefault();
                        // *** TODO: test this here, will not work with functions etc!
                        AnyTableSource ats = qs.EnumerateTableSources(false).First();
                        TableSource ts = ats.FindDescendant<TableSource>();

                        // **** tweak this to get appropriate histogram resolution and be compatible with small search areas with few object
                        do
                        {
                            stat = ComputeTableStatistics(ts, binsize);

                            if (stat.BinCount.Count == 0) break;

                            binsize /= 10.0m;
                        }
                        while (stat.BinCount.Count / partitionCount < 10 && binsize > 0);

                        // --- determine partition limits based on the first table's statistics
                        SqlQueryPartition qp = null;
                        int cnt = 0;
                        int bin = 0;
                        while (bin < stat.BinCount.Count)
                        {
                            if (qp == null)
                            {
                                cnt = 0;
                                qp = new SqlQueryPartition(this, this.Context);
                                qp.ID = Partitions.Count;
                                qp.PartitioningKeyFrom = (double)stat.BinMin[bin];
                            }

                            cnt += stat.BinCount[bin];

                            if (cnt >= (int)(stat.Count / (double)partitionCount) || (bin == stat.BinCount.Count - 1))
                            {
                                qp.PartitioningKeyTo = (double)stat.BinMax[bin];
                                AppendPartition(qp);
                                qp = null;
                            }

                            bin++;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        public override object Clone()
        {
            return new SqlQuery(this);
        }

    }
}
