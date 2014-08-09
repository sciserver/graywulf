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

        // TODO: check if it's used, if not, remove
        [OperationContract(Name="Open_Uri")]
        void Open(Uri uri);
    }

    /// <summary>
    /// Extends core table copy functionality with function to read and write tables
    /// from/to archive files containing multiple files.
    /// </summary>
    public abstract class CopyTableArchiveBase : CopyTableBase, ICopyTableArchiveBase, ICloneable, IDisposable
    {
        #region Private member variables

        private Uri uri;

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

        public CopyTableArchiveBase(CopyTableArchiveBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        private void CopyMembers(CopyTableArchiveBase old)
        {
            this.uri = old.uri;
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
        public abstract void Open();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileMode"></param>
        /// <param name="archival"></param>
        protected void Open(DataFileMode fileMode, DataFileArchival archival)
        {
            EnsureNotOpen();

            if (baseStream == null)
            {
                // Open input stream
                // Check if the archival option is turned on and open archive
                // file if necessary by opening an IArchiveInputStream

                var sf = GetStreamFactory();
                sf.Uri = uri;
                sf.Mode = fileMode;
                sf.Archival = archival;

                // TODO: add authentication options here

                baseStream = sf.Open();
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
        public void Open(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");  // TODO
            }

            this.baseStream = stream;
            this.ownsBaseStream = false;
            this.uri = null;

            Open();
        }

        /// <summary>
        /// Opens the archive pointed by the URI.
        /// </summary>
        /// <param name="uri"></param>
        public void Open(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri"); // TODO
            }

            this.baseStream = null;
            this.ownsBaseStream = false;
            this.uri = uri;

            Open();
        }

        /// <summary>
        /// Closes the archive.
        /// </summary>
        public void Close()
        {
            if (ownsBaseStream && baseStream != null)
            {
                baseStream.Dispose();
                baseStream = null;
                ownsBaseStream = false;
            }
        }
    }
}
