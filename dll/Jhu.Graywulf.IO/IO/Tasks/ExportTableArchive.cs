using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(ExportTableArchive))]
    [NetDataContract]
    public interface IExportTableArchive : IExportTableBase
    {
        Uri Uri
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        SourceTableQuery[] Sources
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        DataFileBase[] Destinations
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
    public class ExportTableArchive : ExportTableBase, IExportTableArchive, ICloneable, IDisposable
    {
        private Uri uri;
        private SourceTableQuery[] sources;
        private DataFileBase[] destinations;

        [NonSerialized]
        private Stream baseStream;

        [NonSerialized]
        private bool ownsBaseStream;

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public SourceTableQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        public ExportTableArchive()
        {
            InitializeMembers();
        }

        public ExportTableArchive(ExportTableArchive old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.uri = null;
            this.sources = null;
            this.destinations = null;
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        private void CopyMembers(ExportTableArchive old)
        {
            this.uri = old.uri;
            this.sources = Util.DeepCopy.CopyArray(old.sources);
            this.destinations = Util.DeepCopy.CopyArray(old.destinations);
            this.baseStream = null;
            this.ownsBaseStream = false;
        }

        public override object Clone()
        {
            return new ExportTableArchive(this);
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
                sf.Mode = DataFileMode.Write;
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
            if (!(baseStream is IArchiveOutputStream))
            {
                throw new InvalidOperationException();
            }

            if (sources == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (destinations == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (sources.Length != destinations.Length)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            // Write individual tables into the archive


            for (int i = 0; i < sources.Length; i++)
            {
                var aos = (IArchiveOutputStream)baseStream;

                var entry = aos.CreateFileEntry(destinations[i].Uri.ToString(), 0);
                aos.WriteNextEntry(entry);

                try
                {
                    destinations[i].Open(baseStream, DataFileMode.Write);
                    WriteTable(sources[i], destinations[i]);
                }
                finally
                {
                    destinations[i].Close();
                }
            }
        }
    }
}
