using System;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(ImportTable))]
    [NetDataContract]
    public interface IImportTable : ICopyTableBase, ICopyDataStream
    {
        DataFileBase Source
        {
            get;
            set;
        }

        DestinationTable Destination
        {
            get;
            set;
        }

        [OperationContract]
        Task<TableCopyResults> ExecuteAsyncEx(DataFileBase source, DestinationTable destination, TableCopySettings settings);
    }

    /// <summary>
    /// Extends basic table copy functionality to import the tables from a file
    /// into database tables.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTable : CopyTableBase, IImportTable, ICloneable, IDisposable
    {
        #region Private member variables

        private DataFileBase source;
        private DestinationTable destination;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the data source (data file).
        /// </summary>
        public DataFileBase Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or set the destination (database table).
        /// </summary>
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        
        #endregion
        #region Constructors and initializers

        public ImportTable()
        {
            InitializeMembers();
        }

        public ImportTable(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public ImportTable(ImportTable old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(ImportTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ImportTable(this);
        }

        public override void Dispose()
        {
        }

        #endregion

        public async Task OpenAsync()
        {
            source.FileMode = DataFileMode.Read;
            source.StreamFactory = GetStreamFactory();
            await source.OpenAsync();
        }
        
        public void Close()
        {
            source.Close();
        }

        protected override TableCopyResult CreateResult()
        {
            return new TableCopyResult()
            {
                SourceFileName = source.Uri == null ? null : Util.UriConverter.GetFilename(source.Uri),
            };
        }

        [OperationBehavior]
        public async Task<TableCopyResults> ExecuteAsyncEx(DataFileBase source, DestinationTable destination, TableCopySettings settings)
        {
            this.source = source;
            this.destination = destination;
            this.Settings = settings;

            await OpenAsync();
            await ExecuteAsync();
            Close();

            return Results;
        }

        /// <summary>
        /// Executes the copy operation.
        /// </summary>
        protected override async Task OnExecuteAsync()
        {
            if (source == null)
            {
                throw Error.SourceNull();
            }

            if (destination == null)
            {
                throw Error.DestinationNull();
            }

            try
            {
                // Make sure file is in read mode and uses the right
                // stream factory to open the URI
                if (source.IsClosed)
                {
                    await OpenAsync();
                }

                // Import the file by wrapping it into a dummy command
                using (var cmd = new FileCommand(source))
                {
                    await CopyToTableAsync(cmd, destination);
                }
            }
            finally
            {
                Close();
            }
        }
    }
}
