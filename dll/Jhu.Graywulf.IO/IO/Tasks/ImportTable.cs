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
    [RemoteServiceClass(typeof(ImportTable))]
    [NetDataContract]
    public interface IImportTable : ICopyTableBase
    {
        DataFileBase Source
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
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTable : CopyTableBase, IImportTable, ICloneable, IDisposable
    {
        private DataFileBase source;
        private DestinationTable destination;

        public DataFileBase Source
        {
            get { return source; }
            set { source = value; }
        }

        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public ImportTable()
        {
            InitializeMembers();
        }

        public ImportTable(ImportTable old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(ImportTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ImportTable(this);
        }

        public override void Dispose()
        {
        }

        protected override void OnExecute()
        {
            if (source == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (destination == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            try
            {
                source.FileMode = DataFileMode.Read;
                source.StreamFactoryType = StreamFactoryType;
                source.Open();
                ReadTable(source, destination);
            }
            finally
            {
                source.Close();
            }
        }
    }
}
