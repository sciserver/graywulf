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
    public interface ITableExport : IRemoteService
    {
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class TableExport : RemoteServiceBase, ITableExport
    {
        private TableSourceBase[] sources;
        private DataFileBase[] destinations;
        private Uri path;
        private DataFileArchival archival;
        private int timeout;

        public TableSourceBase[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        public Uri Path {
            get { return path; }
            set { path = value; }
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
            this.path = null;
            this.archival = DataFileArchival.Automatic;
            this.timeout = 1000;    // *** TODO: use constant or setting
        }

        private void CopyMembers(TableExport old)
        {
            this.sources = DeepCopyUtil.CopyArray(old.sources);
            this.destinations = DeepCopyUtil.CopyArray(old.destinations);
            this.path = old.path;
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

            // Open output stream
            var sf = StreamFactory.Create();
            sf.Mode = DataFileMode.Write;
            sf.Archival = archival;
            sf.Uri = path;
            // TODO: add authentication options here

            using (var output = sf.Open())
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    ExportTable(sources[i], destinations[i], output);
                }

                output.Flush();
                output.Close();
            }
        }

        private void ExportTable(TableSourceBase source, DataFileBase destination, Stream output)
        {
            try
            {
                // Open file
                destination.Open(output, DataFileMode.Write);

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
