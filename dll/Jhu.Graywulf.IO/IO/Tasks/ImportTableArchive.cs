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
        DataFileBase[] Sources
        {
            get;
            set;
        }

        DestinationTable[] Destinations
        {
            get;
            set;
        }

        [OperationContract]
        Task<TableCopyResults> ExecuteAsyncEx(DataFileBase[] sources, DestinationTable[] destinations, TableCopySettings settings, TableArchiveSettings archiveSettings);
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

        private DataFileBase[] sources;

        private DestinationTable[] destinations;

        private string currentFilename;

        #endregion
        #region Properties

        public DataFileBase[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        /// <summary>
        /// Gets or sets the destination of the file import operation.
        /// </summary>
        public DestinationTable[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
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
            this.sources = null;
            this.destinations = null;
        }

        private void CopyMembers(ImportTableArchive old)
        {
            this.sources = old.sources;
            this.destinations = old.destinations;
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

        public async Task<TableCopyResults> ExecuteAsyncEx(DataFileBase[] sources, DestinationTable[] destinations, TableCopySettings settings, TableArchiveSettings archiveSettings)
        {
            this.sources = sources;
            this.destinations = destinations;
            this.Settings = settings;
            this.ArchiveSettings = archiveSettings;

            await OpenAsync();
            await ExecuteAsync();
            Close();

            return Results;
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

            // TODO: add logic to handle single or multiple destinations and
            // predefined source tables

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
                            file.GenerateIdentityColumn = ArchiveSettings.GenerateIdentityColumn;

                            // Open the file. It's read directly from the archive stream.
                            try
                            {
                                file.Open(BaseStream, DataFileMode.Read);

                                using (var cmd = new FileCommand(file))
                                {
                                    await CopyToTableAsync(cmd, destinations[0]);
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
