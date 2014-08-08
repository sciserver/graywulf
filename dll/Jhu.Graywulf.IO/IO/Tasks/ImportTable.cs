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
    [RemoteService(typeof(ImportTable))]
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

    /// <summary>
    /// Extends basic table copy functionality to import the tables from a file
    /// into database tables.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTable : CopyTableBase, IImportTable, ICloneable, IDisposable
    {
        #region Private member variables

        private DataFileBase source;
        private DestinationTable destination;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the data source (data file).
        /// </summary>
        public DataFileBase Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or set the destination (database table).
        /// </summary>
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        #endregion
        #region Constructors and initializers

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

        #endregion

        /// <summary>
        /// Executes the copy operation.
        /// </summary>
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
                // Make sure file is in read mode and uses the right
                // stream factory to open the URI

                // TODO: add authentication options here

                if (source.IsClosed)
                {
                    source.FileMode = DataFileMode.Read;
                    source.StreamFactory = GetStreamFactory();
                    source.Open();
                }

                CopyFromFile(source, destination);
            }
            finally
            {
                source.Close();
            }
        }
    }
}
