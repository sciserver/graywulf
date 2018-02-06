using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICopyTableArchiveBase : ICopyTableBase, ICopyDataStream
    {
        TableArchiveSettings ArchiveSettings
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }
    }

    /// <summary>
    /// Extends core table copy functionality with function to read and write tables
    /// from/to archive files containing multiple files.
    /// </summary>
    [Serializable]
    public abstract class CopyTableArchiveBase : CopyTableBase, ICopyTableArchiveBase, ICloneable, IDisposable
    {
        #region Private member variables

        [NonSerialized]
        private TableArchiveSettings archiveSettings;

        [NonSerialized]
        private Stream baseStream;

        /// <summary>
        /// If true, the base stream has been opened by this class and will be disposed
        /// when the class itself is disposed
        /// </summary>
        [NonSerialized]
        private bool ownsBaseStream;

        #endregion
        #region Properties

        public TableArchiveSettings ArchiveSettings
        {
            get { return archiveSettings; }
            set { archiveSettings = value; }
        }

        /// <summary>
        /// Gets the base stream that reads and writes the archive, not its contents
        /// </summary>
        protected Stream BaseStream
        {
            get { return baseStream; }
        }

        #endregion
        #region Constructors and initializers

        public CopyTableArchiveBase()
        {
            InitializeMembers();
        }

        public CopyTableArchiveBase(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public CopyTableArchiveBase(CopyTableArchiveBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.archiveSettings = new TableArchiveSettings();
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        private void CopyMembers(CopyTableArchiveBase old)
        {
            this.archiveSettings = new TableArchiveSettings(old.archiveSettings);
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        public override void Dispose()
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Makes sure that the base stream is not open.
        /// </summary>
        protected void EnsureNotOpen()
        {
            if (ownsBaseStream && baseStream != null)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// When overriden is derived classes, opens the archive pointed by the URI.
        /// </summary>
        public abstract Task OpenAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileMode"></param>
        /// <param name="archival"></param>
        protected async Task OpenAsync(DataFileMode fileMode, DataFileArchival archival)
        {
            EnsureNotOpen();

            if (baseStream == null)
            {
                // Open input stream
                // Check if the archival option is turned on and open archive
                // file if necessary by opening an IArchiveInputStream

                var sf = GetStreamFactory();
                sf.Uri = archiveSettings.Uri;
                sf.Credentials = archiveSettings.Credentials;
                sf.Mode = fileMode;
                sf.Archival = archival;

                baseStream = await sf.OpenAsync();
                ownsBaseStream = true;
            }
            else
            {
                // Do nothing, open stream is passed from outside
            }
        }

        /// <summary>
        /// Opens the archive by taking an already opened stream from outside.
        /// </summary>
        /// <param name="stream"></param>
        public async Task OpenAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");  // TODO
            }

            this.baseStream = stream;
            this.ownsBaseStream = false;

            await OpenAsync();
        }

        /// <summary>
        /// Opens the archive pointed by the URI.
        /// </summary>
        /// <param name="uri"></param>
        public async Task OpenAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri"); // TODO
            }

            this.archiveSettings.Uri = uri;
            this.baseStream = null;
            this.ownsBaseStream = false;

            await OpenAsync();
        }

        /// <summary>
        /// Closes the archive.
        /// </summary>
        public void Close()
        {
            if (ownsBaseStream && baseStream != null)
            {
                baseStream.Close();
                baseStream.Dispose();
                baseStream = null;
                ownsBaseStream = false;
            }
        }
    }
}
