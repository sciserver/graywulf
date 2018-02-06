using System;
using System.Threading;
using System.ServiceModel;
using System.Data;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyTable))]
    [NetDataContract]
    public interface ICopyTable : ICopyTableBase
    {
        SourceQuery Source
        {
            get;
            set;
        }

        DestinationTable Destination
        {
            get;
            set;
        }

        [OperationContract]
        Task<TableCopyResults> ExecuteAsyncEx(SourceQuery source, DestinationTable destination, TableCopySettings settings);
    }

    /// <summary>
    /// Implements functions to copy the results of a query into a table,
    /// or a table into another table, possibly on another server.
    /// </summary>
    /// <remarks>
    /// If specified in the destination table class, the output tables will
    /// be automatically created based on the schema of the resultset.
    /// Queries might return multiple results, in this case multiple output
    /// tables will be generated.
    /// </remarks>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = true,
        IncludeExceptionDetailInFaults = true)]
    [Serializable]
    public class CopyTable : CopyTableBase, ICopyTable, ICloneable
    {
        private SourceQuery source;
        private DestinationTable destination;

        public SourceQuery Source
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

        protected override TableCopyResult CreateResult()
        {
            return source.CreateResult();
        }

        public async Task<TableCopyResults> ExecuteAsyncEx(SourceQuery source, DestinationTable destination, TableCopySettings settings)
        {
            this.source = source;
            this.destination = destination;
            this.Settings = settings;

            await ExecuteAsync();

            return Results;
        }

        protected override async Task OnExecuteAsync()
        {
            if (source == null)
            {
                throw Error.SourceNull();
            }

            if (destination == null)
            {
                throw Error.DestinationNull();
            }

            // No exception bypass logic here,
            // server to server table copies should always throw an exception
            await CopyToTableAsync(source, destination);
        }
    }
}
