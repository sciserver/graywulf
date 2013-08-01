using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class TableImporterBase : RemoteServiceBase, ITableImporter
    {
        private bool isBulkCopyCanceled;
        EventWaitHandle bulkCopyFinishedEvent;

        #region Property storage member variabless

        private DestinationTableParameters destination;
        private long rowsAffected;

        #endregion
        #region Properties

        public DestinationTableParameters Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public long RowsAffected
        {
            get { return rowsAffected; }
            protected set { rowsAffected = value; }
        }

        #endregion
        #region Constructors and initializers

        public TableImporterBase()
        {
            InitializeMembers();
        }

        public TableImporterBase(DestinationTableParameters destination)
        {
            InitializeMembers();

            this.destination = destination;
        }

        private void InitializeMembers()
        {
            this.destination = null;
            this.rowsAffected = -1;
        }

        #endregion

        protected void CreateDestinationTable(IDataReader dr)
        {
            var sql = "CREATE TABLE [{0}].[{1}] ({2})";
            var columnlist = String.Empty;

            var dt = dr.GetSchemaTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];

                string typename;
                var name = (string)row["ColumnName"];
                var type = (Type)row["DataType"];
                var size = (int)row["ColumnSize"];
                var isnull = (bool)row["AllowDBNull"];

                typename = Jhu.Graywulf.SqlParser.SqlCodeGen.Constants.SqlTypes[type];

                if (Jhu.Graywulf.SqlParser.SqlCodeGen.Constants.SqlTypeHasSize[type])
                {
                    typename = String.Format("{0} ({1})", typename, size);
                }

                if (i != 0)
                {
                    columnlist += ",\r\n";
                }

                columnlist += String.Format("{0} {1}  {2} NULL", name, typename, isnull ? "" : "NOT");
            }

            // Execute CREATE TABLE query on destination
            using (var cn = new SqlConnection(destination.Table.Dataset.ConnectionString))
            {
                cn.Open();

                sql = String.Format(sql, destination.Table.SchemaName, destination.Table.TableName, columnlist);

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void ExecuteBulkCopy(IDataReader dr)
        {
            isBulkCopyCanceled = false;
            bulkCopyFinishedEvent = new AutoResetEvent(false);

            // Initialize bulk copy
            var sbc = new System.Data.SqlClient.SqlBulkCopy(destination.Table.Dataset.ConnectionString);
            sbc.DestinationTableName = String.Format("[{0}].[{1}]", destination.Table.SchemaName, destination.Table.TableName);
            sbc.BatchSize = destination.BulkInsertBatchSize;
            sbc.BulkCopyTimeout = destination.BulkInsertTimeout;

            // Initialize events
            sbc.NotifyAfter = destination.BulkInsertBatchSize;
            sbc.SqlRowsCopied += delegate(object sender, SqlRowsCopiedEventArgs e)
            {
                RowsAffected = e.RowsCopied;
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

        public override void Cancel()
        {
            if (bulkCopyFinishedEvent != null)
            {
                isBulkCopyCanceled = true;
                bulkCopyFinishedEvent.WaitOne();
            }

            base.Cancel();
        }
    }
}
