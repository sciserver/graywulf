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
        TableCopySettings Settings
        {
            get;
            set;
        }

        TableCopyResults Results
        {
            get;
        }
    }

    /// <summary>
    /// Implements core functions to copy tables to/from files and databases.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class CopyTableBase : RemoteServiceBase, ICopyTableBase, ICloneable, IDisposable
    {
        #region Private member variables

        [NonSerialized]
        private TableCopySettings settings;

        [NonSerialized]
        private TableCopyResults results;

        #endregion
        #region Properties

        public TableCopySettings Settings
        {
            get { return Settings; }
            set { settings = value; }
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
            this.settings = null;
            this.results = new TableCopyResults();
        }

        private void CopyMembers(CopyTableBase old)
        {
            this.settings = old.settings;
            this.results = old.results;
        }

        public abstract object Clone();

        #endregion

        /// <summary>
        /// Returns an instantiated file format factory object.
        /// </summary>
        /// <returns></returns>
        protected FileFormatFactory GetFileFormatFactory()
        {
            return FileFormatFactory.Create(settings.FileFormatFactoryType);
        }

        /// <summary>
        /// Returns an instantiated stream factory object.
        /// </summary>
        /// <returns></returns>
        protected StreamFactory GetStreamFactory()
        {
            return StreamFactory.Create(settings.StreamFactoryType);
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
                        cmd.CommandTimeout = settings.Timeout;

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

                    if (!sdr.HasRows)
                    {
                        result.Status = TableCopyStatus.NoOutput;
                    }
                    else
                    {
                        try
                        {
                            // DestinationTable has the property TableNameTemplate which needs to
                            // be evaluated now

                            // TODO: how to deal with multiple tables in files inside archives?
                            var queryName = sdr.QueryName;
                            var resultsetName = sdr.ResultsetName ?? q.ToString();

                            var table = destination.GetTable(settings.BatchName, queryName, resultsetName, sdr.Metadata);
                            result.DestinationTable = table.UniqueKey;

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

                    Results.Add(result);
                    q++;
                }
                while (await sdr.NextResultAsync(CancellationContext.Token));
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
                        cmd.CommandTimeout = settings.Timeout;

                        await CopyToFileAsync(cmd, destination);

                        tn.Commit();
                    }
                }
            }
        }

        private async Task CopyToFileAsync(ISmartCommand cmd, DataFileBase destination)
        {
            await destination.WriteHeaderAsync();

            using (var sdr = await cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationContext.Token))
            {
                int q = 0;

                do
                {
                    if (q > 0 && !destination.Description.CanHoldMultipleDatasets)
                    {
                        throw Error.MultipleDatasetsUnsupported();
                    }

                    var result = CreateResult();

                    // Take name from smart data reader or generate automatically
                    if (String.IsNullOrWhiteSpace(sdr.ResultsetName) && q != 0)
                    {
                        sdr.ResultsetName = q.ToString();
                    }

                    await CopyToFileAsync(sdr, destination, result);

                    Results.Add(result);

                    q++;
                }
                while (await sdr.NextResultAsync(CancellationContext.Token));
            }

            await destination.WriteFooterAsync();
        }

        /// <summary>
        /// Copies the resultsets of a command into files.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destination"></param>
        private async Task CopyToFileAsync(ISmartDataReader sdr, DataFileBase destination, TableCopyResult result)
        {
            try
            {
                await destination.WriteFromDataReaderAsync(sdr);
                result.RecordsAffected = sdr.RecordsAffected;
                result.Status = TableCopyStatus.Success;
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

            var cg = new SqlServerCodeGenerator();
            var dr = (DbDataReader)sdr;

            // Turn on TABLOCK
            var sbo = System.Data.SqlClient.SqlBulkCopyOptions.TableLock;

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString, sbo)
            {
                DestinationTableName = cg.GetResolvedTableName(destination),
                BulkCopyTimeout = settings.Timeout,
                NotifyAfter = Math.Max(settings.BatchSize, 1000),
                BatchSize = settings.BatchSize,       // Must be set to 0, otherwise SQL Server will write log
                EnableStreaming = true    // TODO: add, new in .net 4.5
            };

            // Initialize events
            sbc.SqlRowsCopied += delegate (object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
            {
                e.Abort = IsCancellationRequested;
                result.RecordsAffected = e.RowsCopied;
            };

            result.DestinationTable = destination.UniqueKey;

            try
            {
                await sbc.WriteToServerAsync(dr, CancellationContext.Token);
                result.RecordsAffected = sbc.RecordsAffected();
                result.Status = TableCopyStatus.Success;
            }
            catch (OperationAbortedException)
            {
                // TODO: test this because tasks throw aggregateexception too

                // This is normal behavior, happens when bulk-copy is
                // forcibly canceled.
                result.Status = TableCopyStatus.Cancelled;
            }
        }

        protected void HandleException(Exception ex, TableCopyResult result)
        {
            // Put a breakpoint here when debuggin bypassed exceptions

            result.Status = TableCopyStatus.Failed;
            result.Error = ex.Message;

            if (!settings.BypassExceptions)
            {
                throw new TableCopyException("Table copy failed: " + ex.Message, ex);     // TODO
            }
        }
    }
}
