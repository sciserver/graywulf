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
    [NetDataContract]
    public interface IExportTableBase : IRemoteService
    {
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
    public abstract class ExportTableBase : RemoteServiceBase, IExportTableBase, ICloneable
    {
        private int timeout;

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public ExportTableBase()
        {
            InitializeMembers();
        }

        public ExportTableBase(ExportTableBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.timeout = 1000;    // *** TODO: use constant or setting
        }

        private void CopyMembers(ExportTableBase old)
        {
            this.timeout = old.timeout;
        }

        public abstract object Clone();

        protected void WriteTable(SourceTableQuery source, DataFileBase destination)
        {
            // Create command that reads the table
            using (var cmd = source.CreateCommand())
            {
                using (var cn = source.OpenConnection())
                {
                    using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;

                        WriteTable(cmd, destination);
                    }
                }
            }
        }

        private void WriteTable(IDbCommand cmd, DataFileBase destination)
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
