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
    public interface ICopyTableArchiveBase : ICopyTableBase
    {
        Uri Uri
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        [OperationContract]
        void Open();

        [OperationContract(Name="Open_Uri")]
        void Open(Uri uri);

        [OperationContract]
        void Close();
    }

    public abstract class CopyTableArchiveBase : CopyTableBase, ICopyTableArchiveBase, ICloneable, IDisposable
    {
        private Uri uri;

        [NonSerialized]
        private Stream baseStream;

        [NonSerialized]
        private bool ownsBaseStream;

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        protected Stream BaseStream
        {
            get { return baseStream; }
        }

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

        protected void EnsureNotOpen()
        {
            if (ownsBaseStream && baseStream != null)
            {
                throw new InvalidOperationException();
            }
        }

        public abstract void Open();

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
                // Do nothing
            }
        }

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
