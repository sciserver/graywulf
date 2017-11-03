using System;
using System.Runtime.Serialization;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.RemoteService;


namespace Jhu.Graywulf.Jobs.CopyTables
{
    [Serializable]
    [DataContract(Name = "CopyTablesItem", Namespace = "")]
    public class CopyTablesItem
    {
        #region Private member variables

        private SourceTableQuery source;
        private DestinationTable destination;
        private bool dropSourceTable;

        #endregion
        #region Properties

        [DataMember]
        public SourceTableQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        [DataMember]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        [DataMember]
        public bool DropSourceTable
        {
            get { return dropSourceTable; }
            set { dropSourceTable = value; }
        }

        #endregion
        #region Constructors and initializers

        public CopyTablesItem()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.source = null;
            this.destination = null;
            this.dropSourceTable = false;
        }

        #endregion

        public ICopyTable GetInitializedCopyTableTask(CancellationContext cancellationContext, CopyTablesParameters parameters)
        {
            // Get server name from the data source
            // This will be the database server responsible for executing the table copy
            var host = ((Jhu.Graywulf.Schema.SqlServer.SqlServerDataset)source.Dataset).HostName;
            var task = RemoteServiceHelper.CreateObject<ICopyTable>(cancellationContext, host, true);

            task.Source = this.source;
            task.Destination = this.destination;
            task.Timeout = parameters.Timeout;
            
            return task;
        }
    }
}
