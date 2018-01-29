using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
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

        protected CopyTableBase(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        protected CopyTableBase(CopyTableBase old)
            : base(old.CancellationContext)
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
            this.results = new TableCopyResults(old.results);
        }

        public abstract object Clone();

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

        protected abstract TableCopyResult CreateResult();

        #region Import functions

        protected async Task CopyToTableAsync(SourceQuery source, DestinationTable destination)
        {
            // Create command that reads the table
            using (var cn = await source.OpenConnectionAsync(CancellationContext.Token))
            {
                using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (var cmd = source.CreateCommand())
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;

                        await CopyToTableAsync(cmd, destination);

                        tn.Commit();
                    }
                }
            }
        }

        protected async Task CopyToTableAsync(ISmartCommand cmd, DestinationTable destination)
        {
            using (var sdr = await cmd.ExecuteReaderAsync(CommandBehavior.KeyInfo, CancellationContext.Token))
            {
                int q = 0;

                do
                {
                    var result = CreateResult();

                    // Take name from smart data reader or generate automatically
                    if (String.IsNullOrWhiteSpace(sdr.Name) && q != 0)
                    {
                        sdr.Name = q.ToString();
                    }

                    await CopyToTableAsync(sdr, destination, result);
                    Results.Add(result);
                    q++;
                }
                while (await sdr.NextResultAsync(CancellationContext.Token));
            }
        }

        /// <summary>
        /// Copies recordsets by executing a command and stores results
        /// in destination tables.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destination"></param>
        private async Task CopyToTableAsync(ISmartDataReader sdr, DestinationTable destination, TableCopyResult result)
        {
            try
            {
                // DestinationTable has the property TableNameTemplate which needs to
                // be evaluated now
                var table = destination.GetTable(batchName, sdr.Name, null, sdr.Metadata);
                result.TargetTable = table;

                // Certain data readers cannot determine the columns from the data file,
                // (for instance, SqlServerNativeBinaryReader), hence we need to copy columns
                // from the destination table instead
                if ((destination.Options & TableInitializationOptions.Create) == 0 &&
                    sdr is FileDataReader &&
                    (sdr.Columns == null || sdr.Columns.Count == 0))
                {
                    var fdr = (FileDataReader)sdr;
                    fdr.CreateColumns(new List<Column>(table.Columns.Values.OrderBy(c => c.ID)));
                }

                // TODO: make schema operation async
                table.Initialize(sdr.Columns, destination.Options);

                await ExecuteBulkCopyAsync(sdr, table, result);
            }
            catch (Exception ex)
            {
                HandleException(ex, result);
            }
        }

        #endregion
        #region Export functions

        /// <summary>
        /// Copies the results of a query into a file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected async Task CopyToFileAsync(SourceQuery source, DataFileBase destination)
        {
            // Create command that reads the table
            using (var cmd = source.CreateCommand())
            {
                using (var cn = await source.OpenConnectionAsync(CancellationContext.Token))
                {
                    using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;

                        await CopyToFileAsync(cmd, destination);

                        tn.Commit();
                    }
                }
            }
        }

        private async Task CopyToFileAsync(ISmartCommand cmd, DataFileBase destination)
        {
            using (var sdr = await cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationContext.Token))
            {
                int q = 0;

                do
                {
                    var result = CreateResult();

                    // Take name from smart data reader or generate automatically
                    if (String.IsNullOrWhiteSpace(sdr.Name) && q != 0)
                    {
                        sdr.Name = q.ToString();
                    }

                    await CopyToFileAsync(sdr, destination, result);
                    Results.Add(result);

                    q++;
                }
                while (await sdr.NextResultAsync(CancellationContext.Token));
            }
        }

        /// <summary>
        /// Copies the resultsets of a command into files.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destination"></param>
        private async Task CopyToFileAsync(ISmartDataReader sdr, DataFileBase destination, TableCopyResult result)
        {
            // TODO multiple resultsets?

            try
            {
                await destination.WriteFromDataReaderAsync(sdr);
                result.RecordsAffected = sdr.RecordsAffected;
            }
            catch (Exception ex)
            {
                HandleException(ex, result);
            }
        }

        #endregion

        /// <summary>
        /// Executest bulk copy to ingest data from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        private async Task ExecuteBulkCopyAsync(ISmartDataReader sdr, Table destination, TableCopyResult result)
        {
            // Bulk insert is a tricky animal. To get best performance, batch size
            // has to be set to zero and table locking has to be set on. This prevents
            // writing the data into the transaction log prior to copying it to
            // the table. The database recovery model needs to be set to simple.            

            // TODO: it can only import the first resultset from dr
            var cg = new SqlServerCodeGenerator();
            var dr = (DbDataReader)sdr;

            // Turn on TABLOCK
            var sbo = System.Data.SqlClient.SqlBulkCopyOptions.TableLock;

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString, sbo)
            {
                DestinationTableName = cg.GetResolvedTableName(destination),
                BulkCopyTimeout = timeout,
                NotifyAfter = Math.Max(batchSize, 1000),
                BatchSize = batchSize,       // Must be set to 0, otherwise SQL Server will write log
                EnableStreaming = true    // TODO: add, new in .net 4.5
            };

            // Initialize events
            sbc.SqlRowsCopied += delegate (object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
            {
                e.Abort = IsCancellationRequested;
                result.RecordsAffected = e.RowsCopied;
            };

            try
            {
                await sbc.WriteToServerAsync(dr, CancellationContext.Token);
            }
            catch (OperationAbortedException)
            {
                // TODO: test this because tasks throw aggregateexception too

                // This is normal behavior, happens when bulk-copy is
                // forcibly canceled.
            }
        }

        protected void HandleException(Exception ex, TableCopyResult result)
        {
            // Put a breakpoint here when debuggin bypassed exceptions

            result.Status = TableCopyStatus.Failed;
            result.Error = ex.Message;

            if (!BypassExceptions)
            {
                throw new TableCopyException("Table copy failed: " + ex.Message, ex);     // TODO
            }
        }
    }
}
