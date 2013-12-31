using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using System.Threading;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface IImportTableBase : IRemoteService
    {
        int BatchSize
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        int Timeout
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class ImportTableBase : RemoteServiceBase, IImportTableBase, ICloneable
    {
        private int batchSize;
        private int timeout;

        [NonSerialized]
        private bool isBulkCopyCanceled;
        [NonSerialized]
        EventWaitHandle bulkCopyFinishedEvent;

        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        protected ImportTableBase()
        {
            InitializeMembers();
        }

        protected ImportTableBase(ImportTableBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.batchSize = 10000;
            this.timeout = 1000;    // *** TODO: use constant or setting
        }

        private void CopyMembers(ImportTableBase old)
        {
            this.batchSize = old.batchSize;
            this.timeout = old.timeout;
        }

        public abstract object Clone();

        protected void ImportTable(IDbCommand cmd, DestinationTable destination)
        {
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            ccmd.ExecuteReader(dr =>
            {
                // TODO: Add multiple results logic

                // TODO: Add table naming logic here, maybe...
                var table = destination.GetTable();

                table.Initialize(dr.GetSchemaTable(), destination.Options);
                ExecuteBulkCopy(dr, table);
            });

            UnregisterCancelable(guid);
        }

        /// <summary>
        /// Executest bulk copy to ingest data from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        protected void ExecuteBulkCopy(IDataReader dr, Table destination)
        {
            // TODO: it can only import the first resultset from dr

            isBulkCopyCanceled = false;
            bulkCopyFinishedEvent = new AutoResetEvent(false);

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString);
            sbc.DestinationTableName = destination.GetFullyResolvedName();
            sbc.BatchSize = batchSize;
            sbc.BulkCopyTimeout = timeout;

            // Initialize events
            sbc.NotifyAfter = batchSize;
            sbc.SqlRowsCopied += delegate(object sender, SqlRowsCopiedEventArgs e)
            {
                //RowsAffected = e.RowsCopied;  // TODO: delete if not used
                e.Abort = isBulkCopyCanceled;
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

        /// <summary>
        /// Cancels the bulk insert operation
        /// </summary>
        public override void Cancel()
        {
            if (bulkCopyFinishedEvent != null)
            {
                isBulkCopyCanceled = true;
                bulkCopyFinishedEvent.WaitOne();
            }

            base.Cancel();
        }
    }
}
