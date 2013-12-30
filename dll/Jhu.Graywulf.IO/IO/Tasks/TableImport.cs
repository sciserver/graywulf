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

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(TableImport))]
    public interface ITableImport : ITableImportBase
    {
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class TableImport : TableImportBase, ITableImport, ICloneable
    {
        private DataFileBase[] sources;
        private Uri path;
        private DataFileArchival archival;

        public DataFileBase[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public Uri Path
        {
            get { return path; }
            set { path = value; }
        }

        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        public TableImport()
        {
            InitializeMembers();
        }

        public TableImport(TableImport old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sources = null;
            this.path = null;
            this.archival = DataFileArchival.Automatic;
        }

        private void CopyMembers(TableImport old)
        {
            this.sources = Util.DeepCopy.CopyArray(old.sources);
            this.path = old.path;
            this.archival = old.archival;
        }

        public override object Clone()
        {
            return new TableImport(this);
        }

        protected override void OnExecute()
        {
            if (sources == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (Destinations == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (sources.Length != Destinations.Length)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            // Open input stream
            var sf = StreamFactory.Create();
            sf.Mode = DataFileMode.Read;
            sf.Archival = archival;
            sf.Uri = path;
            // TODO: add authentication options here

            using (var input = sf.Open())
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    ImportTable(sources[i], Destinations[i], input);
                }

                input.Close();
            }
        }

        private void ImportTable(DataFileBase source, Table destination, Stream input)
        {
            try
            {
                // Open file
                source.Open(input, DataFileMode.Read);

                // Create a command that reads the file
                using (var cmd = new FileCommand(source))
                {
                    ImportTable(cmd, destination);
                }
            }
            finally
            {
                source.Close();
            }
        }
    }
}
