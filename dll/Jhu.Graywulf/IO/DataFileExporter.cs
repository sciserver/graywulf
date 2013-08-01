using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class DataFileExporter : RemoteServiceBase, IDataFileExporter
    {
        private SourceQueryParameters source;
        private DataFileBase destination;

        public SourceQueryParameters Source
        {
            get { return source; }
            set { source = value; }
        }

        public DataFileBase Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public DataFileExporter()
        {
            InitializeMembers();
        }

        public DataFileExporter(SourceQueryParameters source, DataFileBase destination)
        {
            InitializeMembers();

            this.source = source;
            this.destination = destination;
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
        }

        protected override void OnExecute()
        {
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
                                destination.WriteFromDataReader(dr);
                            });

                        UnregisterCancelable(guid);
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
