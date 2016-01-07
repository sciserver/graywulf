using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

#if BULKINSERTFIX

namespace Jhu.Graywulf.IO.CmdLineUtil
{
    class ImportTableFix : ImportTable
    {
        public string Order { get; set; }

        protected override void ExecuteBulkCopy(System.Data.Common.DbDataReader dr, Table destination, TableCopyResult result)
        {
            var cg = new SqlServerCodeGenerator();

            isBulkCopyCancelRequested = false;
            bulkCopyFinishedEvent = new AutoResetEvent(false);

            // Turn on TABLOCK
            var sbo = System.Data.SqlClient.SqlBulkCopyOptions.TableLock;

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.Custom.SqlBulkCopy(destination.Dataset.ConnectionString, sbo)
            {
                DestinationTableName = cg.GetResolvedTableName(destination),
                BulkCopyTimeout = Timeout,
                NotifyAfter = BatchSize,
                BatchSize = BatchSize,       // Must be set to 0, otherwise SQL Server will write log
                EnableStreaming = true,    // TODO: add, new in .net 4.5
                Order = Order
            };

            // Initialize events
            sbc.SqlRowsCopied += delegate(object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
            {
                e.Abort = isBulkCopyCancelRequested;
                result.RecordsAffected = e.RowsCopied;
            };

            try
            {
                sbc.WriteToServer(dr);
            }
            finally
            {
                bulkCopyFinishedEvent.Set();
            }
        }
    }
}

#endif