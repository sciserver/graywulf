using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;
using System.Threading;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ITableImportBase : IRemoteService
    {
        Table[] Destinations
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        TableInitializationOptions Options
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        int BatchSize
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

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
    public abstract class TableImportBase : RemoteServiceBase, ITableImportBase, ICloneable
    {
        private Table[] destinations;
        private TableInitializationOptions options;
        private int batchSize;
        private int timeout;

        [NonSerialized]
        private bool isBulkCopyCanceled;
        [NonSerialized]
        EventWaitHandle bulkCopyFinishedEvent;

        public Table[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        public TableInitializationOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        protected TableImportBase()
        {
            InitializeMembers();
        }

        protected TableImportBase(TableImportBase old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.destinations = null;
            this.options = TableInitializationOptions.Append;
            this.batchSize = 10000;
            this.timeout = 1000;    // *** TODO: use constant or setting
        }

        private void CopyMembers(TableImportBase old)
        {
            this.destinations = Util.DeepCopy.CopyArray(old.destinations);
            this.options = old.options;
            this.batchSize = old.batchSize;
            this.timeout = old.timeout;
        }

        public abstract object Clone();

        protected void ImportTable(IDbCommand cmd, Table destination)
        {
            var guid = Guid.NewGuid();
            var ccmd = new CancelableDbCommand(cmd);
            RegisterCancelable(guid, ccmd);

            ccmd.ExecuteReader(dr =>
            {
                destination.Initialize(dr.GetSchemaTable(), options);
                ExecuteBulkCopy(dr, destination);
            });

            UnregisterCancelable(guid);
        }

        /// <summary>
        /// Executest bulk copy to ingest data from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        protected void ExecuteBulkCopy(IDataReader dr, Table destination)
        {
            // TODO: it can only import the first resultset from dr

            isBulkCopyCanceled = false;
            bulkCopyFinishedEvent = new AutoResetEvent(false);

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Dataset.ConnectionString);
            sbc.DestinationTableName = destination.GetFullyResolvedName();
            sbc.BatchSize = batchSize;
            sbc.BulkCopyTimeout = timeout;

            // Initialize events
            sbc.NotifyAfter = batchSize;
            sbc.SqlRowsCopied += delegate(object sender, SqlRowsCopiedEventArgs e)
            {
                //RowsAffected = e.RowsCopied;  // TODO: delete if not used
                e.Abort = isBulkCopyCanceled;
            };

            try
            {
                sbc.WriteToServer(dr);
            }
            finally
            {
                bulkCopyFinishedEvent.Set();
            }
        }

        /// <summary>
        /// Cancels the bulk insert operation
        /// </summary>
        public override void Cancel()
        {
            if (bulkCopyFinishedEvent != null)
            {
                isBulkCopyCanceled = true;
                bulkCopyFinishedEvent.WaitOne();
            }

            base.Cancel();
        }

        /*
        /// <summary>
        /// Creates the destination table
        /// </summary>
        /// <param name="schemaTable"></param>
        public static void CreateDestinationTable(DataTable schemaTable, Table destination)
        {
            // Generate create table SQL
            var cg = new SqlServerCodeGenerator();
            var sql = cg.GenerateCreateDestinationTableQuery(schemaTable, destination);

            // Execute CREATE TABLE query on destination
            using (var cn = new SqlConnection(destination.Dataset.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void TruncateTable(Table table)
        {
            // TODO: move this to schema eventually
            string sql = String.Format("TRUNCATE TABLE [{0}].[{1}]", table.SchemaName, table.TableName);

            ExecuteCommandNonQuery(sql, table.Dataset.ConnectionString);
        }*/
    }
}
