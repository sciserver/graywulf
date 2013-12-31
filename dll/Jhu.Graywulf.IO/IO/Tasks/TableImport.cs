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
        DataFileBase[] Sources
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        Table[] Destinations
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
    public class TableImport : TableImportBase, ITableImport, ICloneable, IDisposable
    {
        private DataFileBase[] sources;
        private Table[] destinations;

        public DataFileBase[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public Table[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
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
            this.destinations = null;
        }

        private void CopyMembers(TableImport old)
        {
            this.sources = Util.DeepCopy.CopyArray(old.sources);
            this.destinations = Util.DeepCopy.CopyArray(old.destinations);
        }

        public override object Clone()
        {
            return new TableImport(this);
        }

        public void Dispose()
        {
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

            // This is the tricky part here...

            // Import each file
            for (int i = 0; i < sources.Length; i++)
            {
                ImportTable(sources[i], destinations[i]);
            }
        }

        private void ImportTable(DataFileBase source, Table destination)
        {
            // Create a command that reads the file
            using (var cmd = new FileCommand(source))
            {
                ImportTable(cmd, destination);
            }
        }
    }
}
