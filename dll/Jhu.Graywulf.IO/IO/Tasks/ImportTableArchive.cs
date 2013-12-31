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
    public interface IImportTableArchive : IImportTableBase
    {
        Uri Uri
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

        [OperationContract]
        void Open();

        [OperationContract(Name="Open_Uri")]
        void Open(Uri uri);

        [OperationContract]
        void Close();
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTableArchive : ImportTableBase, IImportTableArchive, ICloneable, IDisposable
    {
        private Uri uri;
        private DestinationTable destination;

        [NonSerialized]
        private Stream baseStream;

        [NonSerialized]
        private bool ownsBaseStream;

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

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
            this.uri = null;
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        private void CopyMembers(ImportTableArchive old)
        {
            this.uri = old.uri;
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        public override object Clone()
        {
            return new ImportTableArchive(this);
        }

        public void Dispose()
        {
            Close();
        }

        protected virtual void EnsureNotOpen()
        {
            if (ownsBaseStream && baseStream != null)
            {
                throw new InvalidOperationException();
            }
        }

        public void Open()
        {
            EnsureNotOpen();

            if (baseStream == null)
            {
                // Open input stream
                // Check if the archival option is turned on and open archive
                // file if necessary by opening an IArchiveInputStream

                var sf = StreamFactory.Create();
                sf.Mode = DataFileMode.Read;
                sf.Uri = uri;
                sf.Archival = DataFileArchival.Automatic;
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

        protected override void OnExecute()
        {
            if (baseStream == null)
            {
                throw new InvalidOperationException();
            }

            // Make sure it's an archive stream
            if (!(baseStream is IArchiveInputStream))
            {
                throw new InvalidOperationException();
            }

            // Read the archive file by file and import tables
            var ff = FileFormatFactory.Create();
            var ais = (IArchiveInputStream)baseStream;
            IArchiveEntry entry;

            while ((entry = ais.ReadNextFileEntry()) != null)
            {
                if (!entry.IsDirectory)
                {
                    // Create source file
                    using (var format = ff.CreateFile(entry.Filename))
                    {
                        format.Open(baseStream, DataFileMode.Read);

                        using (var cmd = new FileCommand(format))
                        {
                            // TODO: Pass table name here
                            ImportTable(cmd, destination);
                        }
                    }
                }
            }
        }
    }
}
