using System;
using System.Threading;
using System.ServiceModel;
using System.Data;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyTable))]
    [NetDataContract]
    public interface ICopyTable : ICopyTableBase
    {
        SourceTableQuery Source
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
    /// Implements functions to copy the results of a query into a table,
    /// possibly on another server.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class CopyTable : CopyTableBase, ICopyTable, ICloneable
    {
        private SourceTableQuery source;
        private DestinationTable destination;

        public SourceTableQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public CopyTable()
        {
            InitializeMembers();
        }

        public CopyTable(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public CopyTable(CopyTable old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(CopyTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new CopyTable(this);
        }

        protected override async Task OnExecuteAsync()
        {
            if (source == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (destination == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            // Prepare results
            var result = new TableCopyResult()
            {
                SchemaName = source.SchemaName,
                TableName = source.ObjectName,
            };

            Results.Add(result);

            // No exception bypass logic here,
            // server to server copies should always throw an exception

            // Create command that reads the table
            using (var cn = await source.OpenConnectionAsync(CancellationContext.Token))
            {
                using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (var cmd = source.CreateCommand())
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;

                        await CopyFromCommandAsync(cmd, destination, result);

                        tn.Commit();
                    }
                }
            }
        }
    }
}
