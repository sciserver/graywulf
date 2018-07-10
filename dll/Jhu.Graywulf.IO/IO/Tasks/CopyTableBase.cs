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
using Jhu.Graywulf.Sql.QueryRendering.SqlServer;
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

        [NonSerialized]
        private Stack<string> messages;

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
            this.messages = new Stack<string>();
        }

        private void CopyMembers(CopyTableBase old)
        {
            this.settings = old.settings;
            this.results = old.results;
            this.messages = new Stack<string>();
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
                // Initialize message logging so that PRINT etc. from queries can be
                // processed
                if (cn is System.Data.SqlClient.SqlConnection)
                {
                    ((System.Data.SqlClient.SqlConnection)cn).InfoMessage += CopyTableBase_InfoMessage;
                }

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

        private void CopyTableBase_InfoMessage(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs e)
        {
            messages.Push(e.Message);
        }

        protected async Task CopyToTableAsync(ISmartCommand cmd, DestinationTable destination)
        {
            var w = System.Diagnostics.Stopwatch.StartNew();

            using (var sdr = await cmd.ExecuteReaderAsync(CommandBehavior.KeyInfo, CancellationContext.Token))
            {
                int resultsetCounter = 0;

                do
                {
                    var result = CreateResult();

                    if (!sdr.HasRows)
                    {
                        result.Status = TableCopyStatus.NoOutput;
                    }

                    try
                    {
                        var table = GetDestinationTable(destination, sdr, resultsetCounter);
                        result.DestinationTable = table.UniqueKey;

                        // TODO: figure out how to save primary key here and
                        // pass back to caller. It's too expensive to create the PK at this point

                        // Certain data readers cannot determine the columns from the data file,
                        // (for instance, SqlServerNativeBinaryReader), hence we need to copy columns
                        // from the destination table instead
                        if ((destination.Options & TableInitializationOptions.Create) == 0 &&
                            (sdr.Columns == null || sdr.Columns.Count == 0))
                        {
                            sdr.MatchColumns(new List<Column>(table.Columns.Values.OrderBy(c => c.ID)));
                        }

                        // Here we need to decide if an auto-generated primary key is required or not.
                        TableInitializationOptions opts;
                        var columns = new List<Column>();
                        bool haskey = false;

                        foreach (var c in sdr.Columns.OrderBy(c => c.ID))
                        {
                            columns.Add(c);
                            haskey |= c.IsKey;
                        }

                        if (!haskey && destination.Options.HasFlag(TableInitializationOptions.CreatePrimaryKey))
                        {
                            // Add an identity column to the table
                            var c = new Column("__ID", DataTypes.SqlBigInt)
                            {
                                IsIdentity = true,
                                IsKey = true,
                            };

                            columns.Insert(0, c);

                            // Make sure indices are only created afterwards but PK, since it is just an identity column
                            // is created now.
                            opts = destination.Options & ~TableInitializationOptions.CreateIndexes;
                        }
                        else
                        {
                            // Make sure indices and PK are only created afterwards
                            opts = destination.Options & ~(TableInitializationOptions.CreateIndexes | TableInitializationOptions.CreatePrimaryKey);
                        }

                        // TODO: make schema operation async
                        table.Initialize(columns, opts);

                        await ExecuteBulkCopyAsync(sdr, table, result);

                        // Only create PK here if no identity column is created on the fly.
                        if (haskey &&
                            destination.Options.HasFlag(TableInitializationOptions.CreatePrimaryKey) &&
                            table.PrimaryKey != null)
                        {
                            table.PrimaryKey.Create();
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, result);
                    }

                    result.Elapsed = w.Elapsed;
                    w.Restart();

                    Results.Add(result);
                    resultsetCounter++;
                }
                while (await sdr.NextResultAsync(CancellationContext.Token));
            }
        }

        private Table GetDestinationTable(DestinationTable destination, ISmartDataReader sdr, int resultsetCounter)
        {
            // DestinationTable has the property TableNameTemplate which needs to
            // be evaluated now

            // TODO: how to deal with multiple tables in files inside archives?
            var queryName = sdr.QueryName;
            var resultsetName = sdr.ResultsetName ?? resultsetCounter.ToString();

            // Figure out target table name. If a special message is send from
            // the server, use that, otherwise use what's defined in the
            // destination object

            var table = destination.GetTable(settings.BatchName, queryName, resultsetName, resultsetCounter, sdr.Metadata);

            while (messages.Count > 0)
            {
                var msg = messages.Pop();
                var msgobj = ServerMessage.Deserialize(msg);

                if (msgobj != null)
                {
                    if (!String.IsNullOrWhiteSpace(msgobj.DestinationSchema))
                    {
                        table.SchemaName = msgobj.DestinationSchema;
                    }

                    if (!String.IsNullOrWhiteSpace(msgobj.DestinationName))
                    {
                        table.ObjectName = msgobj.DestinationName;
                    }

                    break;
                }
            }

            return table;
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
            var w = System.Diagnostics.Stopwatch.StartNew();

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

                    result.Elapsed = w.Elapsed;
                    w.Restart();

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

            var cg = new SqlServerQueryRenderer();
            var dr = (DbDataReader)sdr;

            // Turn on TABLOCK and other important options
            var sbo = System.Data.SqlClient.SqlBulkCopyOptions.TableLock |
                System.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity |
                System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls;

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString, sbo)
            {
                DestinationTableName = cg.GetResolvedTableName(destination),
                BulkCopyTimeout = settings.Timeout,
                NotifyAfter = Math.Max(settings.BatchSize, 1000),
                BatchSize = settings.BatchSize,       // Must be set to 0, otherwise SQL Server will write log
                EnableStreaming = true,
            };

            // Create explicit column mapping from source to destination. This is
            // necessary because the source might contain hidden columns that are
            // not treated nicely by SqlBulkCopy. This usually happens when KeyInfo is
            // turned on on the DataReader but not all key columns are on the select list.
            // In this case the remaining key columns show up as hidden and must be
            // ignored here.
            foreach (var col in sdr.Columns)
            {
                if (!col.IsHidden)
                {
                    var map = new System.Data.SqlClient.SqlBulkCopyColumnMapping(col.Name, col.Name);
                    sbc.ColumnMappings.Add(map);
                }
            }

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
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif

            result.Status = TableCopyStatus.Failed;
            result.Error = ex.Message;

            if (!settings.BypassExceptions)
            {
                throw new TableCopyException("Table copy failed: " + ex.Message, ex);     // TODO
            }
        }
    }
}
