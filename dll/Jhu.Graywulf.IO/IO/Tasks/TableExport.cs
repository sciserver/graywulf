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
    [RemoteServiceClass(typeof(TableExport))]
    [NetDataContract]
    public interface ITableExport : IRemoteService
    {
        TableSourceQuery[] Sources
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

        Uri Uri
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        DataFileArchival Archival
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        int Timeout
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class TableExport : RemoteServiceBase, ITableExport
    {
        private TableSourceQuery[] sources;
        private DataFileBase[] destinations;
        private Uri uri;
        private DataFileArchival archival;
        private int timeout;

        public TableSourceQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public TableExport()
        {
            InitializeMembers();
        }

        public TableExport(TableExport old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sources = null;
            this.destinations = null;
            this.uri = null;
            this.archival = DataFileArchival.Automatic;
            this.timeout = 1000;    // *** TODO: use constant or setting
        }

        private void CopyMembers(TableExport old)
        {
            this.sources = Util.DeepCopy.CopyArray(old.sources);
            this.destinations = Util.DeepCopy.CopyArray(old.destinations);
            this.uri = old.uri;
            this.archival = old.archival;
            this.timeout = old.timeout;
        }

        protected override void OnExecute()
        {
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

            // Check if the archival option is turned on and create archive
            // file if necessary by opening an IArchiveOutputStream
            Stream output = null;

            try
            {
                if (archival == DataFileArchival.None)
                {
                    // No stream opened
                    // Path will be treated as directory path
                    output = null;
                }
                else
                {
                    // Open output stream using a stream factory
                    var sf = StreamFactory.Create();
                    sf.Mode = DataFileMode.Write;
                    sf.Archival = archival;
                    sf.Uri = uri;
                    // TODO: add authentication options here

                    output = sf.Open();
                }

                // Export tables one by one
                for (int i = 0; i < sources.Length; i++)
                {
                    ExportTable(sources[i], destinations[i], output);
                }
            }
            finally
            {
                if (output != null)
                {
                    output.Flush();
                    output.Close();
                    output.Dispose();
                }
            }
        }

        private void ExportTable(TableSourceQuery source, DataFileBase destination, Stream output)
        {
            try
            {
                // Individual files have to openned differently when writing into
                // an archive and when not. For archives, create a new entry for the
                // file based on it's own filename.

                if (output is IArchiveOutputStream)
                {
                    // Files are saved into an archive
                    var aos = (IArchiveOutputStream)output;
                    var entry = aos.CreateFileEntry(destination.Uri.ToString(), 0);
                    aos.WriteNextEntry(entry);

                    destination.Open(output, DataFileMode.Write);
                }
                else
                {
                    // Files saved individually

                    // If file name is relative, it should be combined with the
                    // path set for the table exporter
                    var fileuri = Util.UriConverter.Combine(uri, destination.Uri);

                    destination.Open(fileuri, DataFileMode.Write);
                }

                // Create command that reads the table
                using (var cmd = source.CreateCommand())
                {
                    using (var cn = source.OpenConnection())
                    {
                        using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            cmd.Connection = cn;
                            cmd.Transaction = tn;
                            cmd.CommandTimeout = timeout;

                            ExportTable(cmd, destination);
                        }
                    }
                }
            }
            finally
            {
                destination.Close();
            }
        }

        private void ExportTable(IDbCommand cmd, DataFileBase destination)
        {
            // Wrap command into a cancellable task
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            // Pass data reader to the file formatter
            ccmd.ExecuteReader(dr =>
            {
                destination.WriteFromDataReader(dr);
            });

            UnregisterCancelable(guid);
        }
    }
}
