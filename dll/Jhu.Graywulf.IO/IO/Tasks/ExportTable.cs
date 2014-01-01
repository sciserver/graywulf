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
    [RemoteServiceClass(typeof(ExportTable))]
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
        private SourceTableQuery source;
        private DataFileBase destination;

        public SourceTableQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        public DataFileBase Destination
        {
            get { return destination; }
            set { destination = value; }
        }

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

        public void Dispose()
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

            WriteTable(source, destination);
        }
    }
}
