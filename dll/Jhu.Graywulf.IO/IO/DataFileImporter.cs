using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class DataFileImporter : TableImporterBase, IDataFileImporter
    {
        private DataFileBase source;

        public DataFileBase Source
        {
            get { return source; }
            set { source = value; }
        }

        public DataFileImporter()
        {
            InitializeMembers();
        }

        public DataFileImporter(DataFileBase source, DestinationTableParameters destination)
            : base(destination)
        {
            InitializeMembers();

            this.source = source;
        }

        private void InitializeMembers()
        {
            this.source = null;
        }

        protected override void OnExecute()
        {
            using (var dr = source.OpenDataReader())
            {
                // TODO: implement other operations
                switch (Destination.Operation)
                {
                    case DestinationTableOperation.Create:
                        CreateDestinationTable(dr.GetSchemaTable());
                        break;
                    default:
                        throw new NotImplementedException();
                }

                ExecuteBulkCopy(dr);
            }
        }

        public void CreateDestinationTable()
        {
            using (var dr = source.OpenDataReader())
            {
                CreateDestinationTable(dr.GetSchemaTable());
            }
        }
    }
}
