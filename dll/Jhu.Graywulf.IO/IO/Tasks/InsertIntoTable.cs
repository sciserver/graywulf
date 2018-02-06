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
    // TODO: this will soon become obsolate, though this is a shortcut to
    // write the results of simple selects into a target table
    // Won't work with scripts

    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyTable))]
    [NetDataContract]
    public interface IInsertIntoTable : ICopyTableBase
    {
    }


    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class InsertIntoTable : CopyTableBase, IInsertIntoTable, ICloneable
    {
        private SourceQuery source;
        private DestinationTable destination;

        protected SourceQuery Source
        {
            get { return source; }
            set { source = value; }
        }

        protected DestinationTable Destination
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

        protected override TableCopyResult CreateResult()
        {
            return source.CreateResult();
        }

        public async Task<TableCopyResults> ExecuteAsync(SourceQuery source, DestinationTable destination, TableCopySettings settings)
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

            var table = destination.GetTable();
            var columns = await source.GetColumnsAsync(CancellationContext.Token);
            table.Initialize(columns, destination.Options);

            // TODO: make schema operation async

            // TODO: this doesn't work with WITH queries
            // Add a token somewhere in the generated query where the
            // insert statement should go

            // TODO: use code generator

            var sql = new StringBuilder();

            switch (source)
            {
                case SourceQuery query:
                    if (query.Header != null)
                    {
                        sql.AppendLine(((SourceQuery)source).Header);
                    }
                    break;
            }

            sql.AppendFormat(
                "INSERT INTO [{0}].[{1}].[{2}] WITH (TABLOCKX) {3}",
                table.DatabaseName,
                table.SchemaName,
                table.TableName,
                source.Query);
            sql.AppendLine();

            switch (source)
            {
                case SourceQuery query:
                    if (query.Footer != null)
                    {
                        sql.AppendLine(query.Footer);
                    }
                    break;
            }

            // Prepare results
            var result = source.CreateResult();
            result.DestinationTable = table.UniqueKey;
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
                        cmd.CommandTimeout = Settings.Timeout;
                        cmd.CommandText = sql.ToString();

                        await cmd.ExecuteNonQueryAsync(CancellationContext.Token);

                        tn.Commit();
                    }
                }
            }
        }
    }
}
