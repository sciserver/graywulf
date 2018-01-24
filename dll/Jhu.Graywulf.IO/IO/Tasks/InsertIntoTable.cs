using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Sql.Schema;
using System.Threading.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyTable))]
    [NetDataContract]
    public interface IInsertIntoTable : ICopyTableBase
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


    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class InsertIntoTable : CopyTableBase, IInsertIntoTable, ICloneable
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

        public InsertIntoTable()
        {
            InitializeMembers();
        }

        public InsertIntoTable(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public InsertIntoTable(InsertIntoTable old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        private void CopyMembers(InsertIntoTable old)
        {
            this.source = old.source;
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new InsertIntoTable(this);
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

            var table = destination.GetTable();
            var columns = await source.GetColumnsAsync(CancellationContext.Token);
            table.Initialize(columns, destination.Options);

            // TODO: make schema operation async

            // TODO: this doesn't work with WITH queries
            // Add a token somewhere in the generated query where the
            // insert statement should go

            // TODO: use code generator

            var sql = new StringBuilder();

            if (source.Header != null)
            {
                sql.AppendLine(source.Header);
            }

            if (source.Query != null)
            {

                sql.AppendFormat(
                    "INSERT INTO [{0}].[{1}].[{2}] WITH (TABLOCKX) {3}",
                    table.DatabaseName,
                    table.SchemaName,
                    table.TableName,
                    source.Query);
                sql.AppendLine();
            }

            if (source.Footer != null)
            {
                sql.AppendLine(source.Footer);
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
            using (var cmd = source.CreateCommand())
            {
                using (var cn = await source.OpenConnectionAsync(CancellationContext.Token))
                {
                    using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        cmd.Connection = cn;
                        cmd.Transaction = tn;
                        cmd.CommandTimeout = Timeout;
                        cmd.CommandText = sql.ToString();

                        await cmd.ExecuteNonQueryAsync(CancellationContext.Token);

                        tn.Commit();
                    }
                }
            }
        }
    }
}
