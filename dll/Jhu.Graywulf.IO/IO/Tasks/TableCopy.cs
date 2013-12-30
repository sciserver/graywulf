using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(TableCopy))]
    [NetDataContract]
    public interface ITableCopy : ITableImportBase
    {
        TableSourceQuery[] Sources
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
    public class TableCopy : TableImportBase, ITableCopy, ICloneable
    {
        private TableSourceQuery[] sources;

        public TableSourceQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public TableCopy()
        {
            InitializeMembers();
        }

        public TableCopy(TableCopy old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sources = null;
        }

        private void CopyMembers(TableCopy old)
        {
            this.sources = Util.DeepCopy.CopyArray(old.sources);
        }

        public override object Clone()
        {
            return new TableCopy(this);
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

            for (int i = 0; i < sources.Length; i++)
            {
                CopyTable(sources[i], Destinations[i]);
            }
        }

        private void CopyTable(TableSourceQuery source, Table destination)
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

                        ImportTable(cmd, destination);
                    }
                }
            }
        }
    }
}
