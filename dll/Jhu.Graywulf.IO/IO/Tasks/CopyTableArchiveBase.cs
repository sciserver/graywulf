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
        Uri Uri
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        Credentials Credentials
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

        private Uri uri;
        private Credentials credentials;

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

        /// <summary>
        /// Gets or sets the URI pointing to the archive, either source or destination
        /// </summary>
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Gets or sets the credentials to be used to access the source or destination URI
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
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
            this.uri = null;
            this.credentials = null;
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        private void CopyMembers(CopyTableArchiveBase old)
        {
            this.uri = old.uri;
            this.credentials = old.credentials;
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
                sf.Uri = uri;
                sf.Credentials = credentials;
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
            this.uri = null;

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

            this.baseStream = null;
            this.ownsBaseStream = false;
            this.uri = uri;

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
