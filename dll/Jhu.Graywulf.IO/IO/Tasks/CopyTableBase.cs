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
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICopyTableBase : IRemoteService
    {
        string BatchName
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

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

        bool BypassExceptions
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

        TableCopyResults Results
        {
            [OperationContract]
            get;
        }
    }

    /// <summary>
    /// Implements core functions to copy tables to/from files and databases.
    /// </summary>
    public abstract class CopyTableBase : RemoteServiceBase, ICopyTableBase, ICloneable, IDisposable
    {
        #region Private member variables

        [NonSerialized]
        private string batchName;

        [NonSerialized]
        private int batchSize;

        [NonSerialized]
        private int timeout;

        [NonSerialized]
        private bool bypassExceptions;

        [NonSerialized]
        private string fileFormatFactoryType;

        [NonSerialized]
        private string streamFactoryType;

        [NonSerialized]
        private TableCopyResults results;

        /// <summary>
        /// When set to true, informs the class that the executing bulk copy operation
        /// is to be cancelled.
        /// </summary>
        [NonSerialized]
        private bool isBulkCopyCancelRequested;

        /// <summary>
        /// Synchronizes the class to the end of the bulk copy operation.
        /// </summary>
        [NonSerialized]
        private EventWaitHandle bulkCopyFinishedEvent;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the name of the batch. Used when
        /// importing and exporting archives.
        /// </summary>
        public string BatchName
        {
            get { return batchName; }
            set { batchName = value; }
        }

        /// <summary>
        /// Gets or sets the batch size of bulk insert operations.
        /// </summary>
        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of bulk insert operations.
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Gets or sets if the task ignores problems and proceeds with table
        /// copy even when an exception is thrown. Exception bypass logic is
        /// implemented differently in derived classes.
        /// </summary>
        public bool BypassExceptions
        {
            get { return bypassExceptions; }
            set { bypassExceptions = value; }
        }

        /// <summary>
        /// Gets or sets the file format factory to use when creating output files
        /// or opening input files.
        /// </summary>
        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        /// <summary>
        /// Gets or sets the stream factory to use when opening input and output
        /// streams to read and write files.
        /// </summary>
        public string StreamFactoryType
        {
            get { return streamFactoryType; }
            set { streamFactoryType = value; }
        }

        public TableCopyResults Results
        {
            get { return results; }
        }

        #endregion
        #region Constructors and initializers

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
            this.batchName = null;
            this.batchSize = Constants.DefaultBulkInsertBatchSize;
            this.timeout = Constants.DefaultBulkInsertTimeout;
            this.bypassExceptions = false;
            this.fileFormatFactoryType = null;
            this.streamFactoryType = null;
            this.results = new TableCopyResults();
        }

        private void CopyMembers(CopyTableBase old)
        {
            this.batchName = old.batchName;
            this.batchSize = old.batchSize;
            this.timeout = old.timeout;
            this.bypassExceptions = old.bypassExceptions;
            this.fileFormatFactoryType = old.fileFormatFactoryType;
            this.streamFactoryType = old.streamFactoryType;
            this.results = new TableCopyResults();
        }

        public abstract object Clone();

        public abstract void Dispose();

        #endregion

        /// <summary>
        /// Returns an instantiated file format factory object.
        /// </summary>
        /// <returns></returns>
        protected FileFormatFactory GetFileFormatFactory()
        {
            return FileFormatFactory.Create(fileFormatFactoryType);
        }

        /// <summary>
        /// Returns an instantiated stream factory object.
        /// </summary>
        /// <returns></returns>
        protected StreamFactory GetStreamFactory()
        {
            return StreamFactory.Create(streamFactoryType);
        }

        /// <summary>
        /// Copies recordsets from a file into destination tables.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected void CopyFromFile(DataFileBase source, DestinationTable destination, TableCopyResult result)
        {
            // Import the file by wrapping it into a dummy command
            using (var cmd = new FileCommand(source))
            {
                CopyFromCommand(cmd, destination, result);
            }
        }

        /// <summary>
        /// Copies recordsets by executing a command and stores results
        /// in destination tables.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destination"></param>
        protected void CopyFromCommand(ISmartCommand cmd, DestinationTable destination, TableCopyResult result)
        {
            // Run bulk insert wrapped into a cancelable task
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            ccmd.ExecuteReader(CommandBehavior.KeyInfo, dr =>
            {
                // TODO: implement multi-table copy, might require converting results to a
                // a collection
                //do
                //{
                    var sdr = (ISmartDataReader)dr;

                    // DestinationTable has the property TableNameTemplate which needs to
                    // be evaluated now
                    var table = destination.GetTable(batchName, cmd.Name, sdr.Name, sdr.Metadata);
                    table.Initialize(sdr.Columns, destination.Options);

                    result.SchemaName = table.SchemaName;
                    result.TableName = table.ObjectName;

                    ExecuteBulkCopy(dr, table, result);
                //}
                //while (dr.NextResult());
            });

            UnregisterCancelable(guid);
        }

        /// <summary>
        /// Copies the results of a query into a file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected void CopyToFile(SourceTableQuery source, DataFileBase destination, TableCopyResult result)
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

                        CopyToFile(cmd, destination, result);
                    }
                }
            }
        }

        /// <summary>
        /// Copies the resultsets of a command into files.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destination"></param>
        private void CopyToFile(ISmartCommand cmd, DataFileBase destination, TableCopyResult result)
        {
            // Wrap command into a cancellable task
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            // Pass data reader to the file formatter
            result.RecordsAffected = ccmd.ExecuteReader(dr =>
            {
                destination.WriteFromDataReader((SmartDataReader)dr);
            });

            UnregisterCancelable(guid);
        }

        /// <summary>
        /// Executest bulk copy to ingest data from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        protected void ExecuteBulkCopy(IDataReader dr, Table destination, TableCopyResult result)
        {
            // Bulk insert is a tricky animal. To get best performance, batch size
            // has to be set to zero and table locking has to be set on. This prevents
            // writing the data into the transaction log prior to copying it to
            // the table. The database recovery model needs to be set to simple.            
            
            // TODO: it can only import the first resultset from dr
            var cg = new SqlServerCodeGenerator();

            isBulkCopyCancelRequested = false;
            bulkCopyFinishedEvent = new AutoResetEvent(false);

            // Turn on TABLOCK
            var sbo = System.Data.SqlClient.SqlBulkCopyOptions.TableLock;

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString, sbo)
            {
                DestinationTableName = cg.GetResolvedTableName(destination),
                BulkCopyTimeout = timeout,
                NotifyAfter = batchSize,
                BatchSize = batchSize,       // Must be set to 0, otherwise SQL Server will write log
                // EnableStreaming = true    // TODO: add, new in .net 4.5
            };

            // Initialize events
            sbc.SqlRowsCopied += delegate(object sender, SqlRowsCopiedEventArgs e)
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

        /// <summary>
        /// Send a cancel request to the bulk copy operation via isBulkCopyCancelRequested
        /// and synchronizes execution to the end of the bulk copy.
        /// </summary>
        public override void Cancel()
        {
            if (bulkCopyFinishedEvent != null)
            {
                isBulkCopyCancelRequested = true;
                bulkCopyFinishedEvent.WaitOne();
            }

            base.Cancel();
        }

        protected void HandleException(Exception ex, TableCopyResult result)
        {
            // Put a breakpoint here when debuggin bypassed exceptions

            result.Status = TableCopyStatus.Failed;
            result.Error = ex.Message;

            if (!BypassExceptions)
            {
                throw new TableCopyException("Table copy failed.", ex);     // TODO
            }
        }
    }
}
