using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICopyTableBase : IRemoteService
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

        string FileFormatFactoryType
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        string StreamFactoryType
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    public abstract class CopyTableBase : RemoteServiceBase, ICopyTableBase, ICloneable, IDisposable
    {
        private int batchSize;
        private int timeout;
        private string fileFormatFactoryType;
        private string streamFactoryType;

        [NonSerialized]
        private bool isBulkCopyCanceled;
        [NonSerialized]
        private EventWaitHandle bulkCopyFinishedEvent;

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

        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        public string StreamFactoryType
        {
            get { return streamFactoryType; }
            set { streamFactoryType = value; }
        }

        protected CopyTableBase()
        {
            InitializeMembers();
        }

        protected CopyTableBase(CopyTableBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.batchSize = 10000;
            this.timeout = 1000;    // *** TODO: use constant or setting
            this.fileFormatFactoryType = null;
            this.streamFactoryType = null;
        }

        private void CopyMembers(CopyTableBase old)
        {
            this.batchSize = old.batchSize;
            this.timeout = old.timeout;
            this.fileFormatFactoryType = old.fileFormatFactoryType;
            this.streamFactoryType = old.streamFactoryType;
        }

        public abstract object Clone();

        public virtual void Dispose()
        {
        }

        protected FileFormatFactory GetFileFormatFactory()
        {
            return FileFormatFactory.Create(fileFormatFactoryType);
        }

        protected StreamFactory GetStreamFactory()
        {
            return StreamFactory.Create(streamFactoryType);
        }

        protected void ReadTable(DataFileBase source, DestinationTable destination)
        {
            // Import the file by wrapping it into a dummy command
            using (var cmd = new FileCommand(source))
            {
                ImportTable(cmd, destination);
            }
        }

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

        protected void WriteTable(SourceTableQuery source, DataFileBase destination)
        {
            // Create command that reads the table
            using (var cmd = source.CreateCommand())
            {
                using (var cn = source.OpenConnection())
                {
                    using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;

                        WriteTable(cmd, destination);
                    }
                }
            }
        }

        private void WriteTable(IDbCommand cmd, DataFileBase destination)
        {
            // Wrap command into a cancellable task
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            // Pass data reader to the file formatter
            ccmd.ExecuteReader(dr =>
            {
                destination.WriteFromDataReader(dr);
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
