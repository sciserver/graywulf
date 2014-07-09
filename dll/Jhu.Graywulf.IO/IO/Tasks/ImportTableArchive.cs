using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(ImportTableArchive))]
    public interface IImportTableArchive : ICopyTableArchiveBase
    {
        DestinationTable Destination
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

        #endregion
        #region Constructors and initializers

        public ImportTableArchive()
        {
            InitializeMembers();
        }

        public ImportTableArchive(ImportTableArchive old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.destination = null;
        }

        private void CopyMembers(ImportTableArchive old)
        {
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ImportTableArchive(this);
        }

        #endregion

        /// <summary>
        /// Opens the archive.
        /// </summary>
        public override void Open()
        {
            Open(DataFileMode.Read, DataFileArchival.Automatic);
        }

        /// <summary>
        /// Executes the import operation
        /// </summary>
        protected override void OnExecute()
        {
            // Make sure stream is open
            if (BaseStream == null)
            {
                throw new InvalidOperationException();
            }

            // Make sure it's an archive stream
            if (!(BaseStream is IArchiveInputStream))
            {
                throw new InvalidOperationException();
            }

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
                    // Use the file format factory to open the file
                    
                    // TODO: what to do with unknown file extensions?
                    // TODO: what to do if a file is malformed?

                    using (var file = ff.CreateFile(entry.Filename))
                    {
                        // Open the file. It's read directly from the archive stream.
                        file.Open(BaseStream, DataFileMode.Read);

                        // Wrap the file into a dummy command that will expose it as
                        // a data reader for bulk insert operations.
                        using (var cmd = new FileCommand(file))
                        {
                            // TODO: implement table auto-naming
                            // TODO: Pass table name here
                            CopyFromCommand(cmd, destination);
                        }
                    }
                }
            }
        }
    }
}
