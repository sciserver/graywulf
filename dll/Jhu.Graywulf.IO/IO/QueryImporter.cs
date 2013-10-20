using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class QueryImporter : TableImporterBase, IQueryImporter
    {
        private SourceQueryParameters source;

        public SourceQueryParameters Source
        {
            get { return source; }
            set { source = value; }
        }

        public QueryImporter()
        {
            InitializeMembers();
        }

        public QueryImporter(SourceQueryParameters source, DestinationTableParameters destination)
            : base(destination)
        {
            InitializeMembers();

            this.source = source;
        }

        private void InitializeMembers()
        {
            this.source = null;
        }

        protected override void OnExecute()
        {
            if ((Destination.Operation & DestinationTableOperation.Drop) != 0)
            {
                DropDestinationTable();
            }

            using (var cn = OpenSourceConnection())
            {
                using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (var cmd = CreateSourceCommand(cn, tn))
                    {
                        var guid = Guid.NewGuid();
                        var ccmd = new CancelableDbCommand(cmd);
                        RegisterCancelable(guid, ccmd);

                        ccmd.ExecuteReader(dr =>
                        {
                            // *** TODO: implement other options
                            if ((Destination.Operation & DestinationTableOperation.Append) != 0)
                            {
                                // TODO: compare schema
                            }
                            else if ((Destination.Operation & DestinationTableOperation.Create) != 0)
                            {
                                CreateDestinationTable(dr.GetSchemaTable());
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            ExecuteBulkCopy(dr);
                        });

                        UnregisterCancelable(guid);
                    }

                    tn.Commit();
                }
            }
        }

        public void CreateDestinationTable()
        {
            using (var cn = OpenSourceConnection())
            {
                using (var tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (var cmd = CreateSourceCommand(cn, tn))
                    {
                        using (var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly)) // TODO: removed | CommandBehavior.KeyInfo))
                        {
                            CreateDestinationTable(dr.GetSchemaTable());
                        }
                    }

                    tn.Commit();
                }
            }
        }

        private DbConnection OpenSourceConnection()
        {
            var dbf = DbProviderFactories.GetFactory(source.Dataset.ProviderName);

            var cn = dbf.CreateConnection();

            cn.ConnectionString = source.Dataset.ConnectionString;
            cn.Open();

            return cn;
        }

        private DbCommand CreateSourceCommand(DbConnection cn, DbTransaction tn)
        {
            var cmd = cn.CreateCommand();

            cmd.Connection = cn;
            cmd.Transaction = tn;
            cmd.CommandText = source.Query;
            cmd.CommandTimeout = source.Timeout;

            return cmd;
        }
    }
}
