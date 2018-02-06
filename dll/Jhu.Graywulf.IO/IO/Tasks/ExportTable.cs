using System;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(ExportTable))]
    [NetDataContract]
    public interface IExportTable : ICopyTableBase, ICopyDataStream
    {
        SourceQuery Source
        {
            get;
            set;
        }
        
        DataFileBase Destination
        {
            get;
            set;
        }

        [OperationContract]
        Task<TableCopyResults> ExecuteAsyncEx(SourceQuery source, DataFileBase destination, TableCopySettings settings);
    }

    /// <summary>
    /// Implements functions to export tables into data files.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ExportTable : CopyTableBase, IExportTable, ICloneable, IDisposable
    {
        #region Private member variables

        private SourceQuery source;
        private DataFileBase destination;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the source query of the export operation.
        /// </summary>
        public SourceQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the destination file of the export operation.
        /// </summary>
        public DataFileBase Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        #endregion
        #region Constructors and initializers

        public ExportTable()
        {
            InitializeMembers();
        }

        public ExportTable(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public ExportTable(ExportTable old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(ExportTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ExportTable(this);
        }

        public override void Dispose()
        {
        }

        #endregion

        public async Task OpenAsync()
        {
            destination.FileMode = DataFileMode.Write;
            destination.StreamFactory = GetStreamFactory();
            await destination.OpenAsync();
        }

        public void Close()
        {
            destination.Close();
        }

        protected override TableCopyResult CreateResult()
        {
            return new TableCopyResult()
            {
                DestinationFileName = destination.Uri == null ? null : Util.UriConverter.GetFilename(destination.Uri),
            };
        }

        public async Task<TableCopyResults> ExecuteAsyncEx(SourceQuery source, DataFileBase destination, TableCopySettings settings)
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
        /// Executes the table export operation
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
                if (destination.IsClosed)
                {
                    await OpenAsync();
                }

                await CopyToFileAsync(source, destination);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Close();
            }
        }
    }
}
