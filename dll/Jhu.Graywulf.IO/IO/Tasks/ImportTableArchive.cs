using System;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(ImportTableArchive))]
    public interface IImportTableArchive : ICopyTableArchiveBase
    {
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
    /// Implements function to import table from a data archive, potentially containing
    /// multiple files and data tables within.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTableArchive : CopyTableArchiveBase, IImportTableArchive, ICloneable, IDisposable
    {
        #region Private member variables

        private DestinationTable destination;
        private ImportTableOptions options;

        private string currentFilename;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the destination of the file import operation.
        /// </summary>
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public ImportTableOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion
        #region Constructors and initializers

        public ImportTableArchive()
        {
            InitializeMembers();
        }

        public ImportTableArchive(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public ImportTableArchive(ImportTableArchive old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.destination = null;
            this.options = null;
        }

        private void CopyMembers(ImportTableArchive old)
        {
            this.destination = old.destination;
            this.options = old.options;
        }

        public override object Clone()
        {
            return new ImportTableArchive(this);
        }

        #endregion

        /// <summary>
        /// Opens the archive.
        /// </summary>
        public override Task OpenAsync()
        {
            return OpenAsync(DataFileMode.Read, DataFileArchival.Automatic);
        }

        protected override TableCopyResult CreateResult()
        {
            return new TableCopyResult()
            {
                SourceFileName = currentFilename,
            };
        }

        /// <summary>
        /// Executes the import operation
        /// </summary>
        protected override async Task OnExecuteAsync()
        {
            // Make sure stream is open
            if (BaseStream == null)
            {
                throw Error.StreamNull();
            }

            // Make sure it's an archive stream
            if (!(BaseStream is IArchiveInputStream))
            {
                throw Error.FileNotArchine();
            }

            try
            {
                // Create the file format factory. This will be used to open
                // the individual files within the archive.
                var ff = GetFileFormatFactory();

                // Read the archive file by file and import tables
                var ais = (IArchiveInputStream)BaseStream;
                IArchiveEntry entry;

                // Iterate through the files in the archive. Not need to traverse
                // the internal directory tree as files are listed with full paths.
                while ((entry = ais.ReadNextFileEntry()) != null)
                {
                    // Skip directory entries, we read files only
                    if (!entry.IsDirectory)
                    {
                        currentFilename = entry.Filename;

                        // Use the file format factory to open the file
                        string filename, extension;
                        DataFileCompression compression;
                        DataFileBase file = null;

                        // We simply skip unrecognized files
                        if (ff.TryCreateFile(Util.UriConverter.FromFilePath(entry.Filename), out filename, out extension, out compression, out file))
                        {
                            if (options != null)
                            {
                                file.GenerateIdentityColumn = options.GenerateIdentityColumn;
                            }

                            // Open the file. It's read directly from the archive stream.
                            try
                            {
                                file.Open(BaseStream, DataFileMode.Read);

                                using (var cmd = new FileCommand(file))
                                {
                                    await CopyToTableAsync(cmd, destination);
                                }
                            }
                            catch (Exception ex)
                            {
                                var result = CreateResult();
                                HandleException(ex, result);
                                Results.Add(result);
                                continue;
                            }
                            finally
                            {
                                file.Dispose();
                            }
                        }
                        else
                        {
                            var result = CreateResult();
                            result.Status = TableCopyStatus.Skipped;
                            Results.Add(result);
                        }
                    }
                }
            }
            finally
            {
                Close();
            }
        }
    }
}
