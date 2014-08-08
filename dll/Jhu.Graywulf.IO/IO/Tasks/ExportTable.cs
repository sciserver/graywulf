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
    [RemoteService(typeof(ExportTable))]
    [NetDataContract]
    public interface IExportTable : ICopyTableBase
    {
        SourceTableQuery Source
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        DataFileBase Destination
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    /// <summary>
    /// Implements functions to export tables into data files.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ExportTable : CopyTableBase, IExportTable, ICloneable, IDisposable
    {
        #region Private member variables

        private SourceTableQuery source;
        private DataFileBase destination;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the source query of the export operation.
        /// </summary>
        public SourceTableQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the destination file of the export operation.
        /// </summary>
        public DataFileBase Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        #endregion
        #region Constructors and initializers

        public ExportTable()
        {
            InitializeMembers();
        }

        public ExportTable(ExportTable old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(ExportTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ExportTable(this);
        }

        public override void Dispose()
        {
        }

        #endregion

        /// <summary>
        /// Executes the table export operation
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
                // TODO: add user access logic here
                if (destination.IsClosed)
                {
                    destination.FileMode = DataFileMode.Write;
                    destination.StreamFactory = GetStreamFactory();
                    destination.Open();
                }

                CopyToFile(source, destination);
            }
            finally
            {
                destination.Close();
            }
        }
    }
}
