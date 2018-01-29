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
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        DestinationTable Destination
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        ImportTableOptions Options
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    /// <summary>
    /// Extends basic table copy functionality to import the tables from a file
    /// into database tables.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTable : CopyTableBase, IImportTable, ICloneable, IDisposable
    {
        #region Private member variables

        private DataFileBase source;
        private DestinationTable destination;
        private ImportTableOptions options;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the data source (data file).
        /// </summary>
        public DataFileBase Source
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return source; }

            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            set { source = value; }
        }

        /// <summary>
        /// Gets or set the destination (database table).
        /// </summary>
        public DestinationTable Destination
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return destination; }

            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            set { destination = value; }
        }

        public ImportTableOptions Options
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return options; }

            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            set { options = value; }
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
            this.options = null;
        }

        private void CopyMembers(ImportTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
            this.options = old.options;
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

            if (options != null)
            {
                source.GenerateIdentityColumn = options.GenerateIdentityColumn;
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
