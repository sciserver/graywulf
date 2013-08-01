using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO
{
    /// <summary>
    /// Implements functionality to create a table for the output of a DataReader
    /// and executes bulk insert to ingest the data.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the parameters of the destination table
        /// </summary>
        public DestinationTableParameters Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets the number of rows processed.
        /// </summary>
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

        /// <summary>
        /// Creates the destination table
        /// </summary>
        /// <param name="schemaTable"></param>
        protected void CreateDestinationTable(DataTable schemaTable)
        {
            // Execute CREATE TABLE query on destination
            using (var cn = new SqlConnection(destination.Table.Dataset.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(BuildDestinationTableSql(schemaTable), cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Generates the SQL script to create the table
        /// </summary>
        /// <param name="schemaTable"></param>
        /// <returns></returns>
        protected string BuildDestinationTableSql(DataTable schemaTable)
        {
            var sql = "CREATE TABLE [{0}].[{1}] ({2})";
            var columnlist = String.Empty;
            var keylist = String.Empty;
            var nokey = false;

            int cidx = 0;
            int kidx = 0;
            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                var column = new Column();
                column.CopyFromSchemaTableRow(schemaTable.Rows[i]);

                if (!column.IsHidden)
                {
                    if (cidx != 0)
                    {
                        columnlist += ",\r\n";
                    }

                    columnlist += String.Format(
                        "{0} {1} {2} NULL",
                        column.Name,
                        column.DataType.NameWithSize,
                        column.IsNullable ? "" : "NOT");

                    cidx++;
                }

                if (column.IsKey)
                {
                    if (column.IsHidden)
                    {
                        // The key is not returned by the query, so no key can be specified on
                        // the final table
                        nokey = true;
                    }

                    if (kidx != 0)
                    {
                        keylist += ",\r\n";
                    }

                    keylist += String.Format("[{0}] ASC", column.Name);

                    kidx++;
                }
            }

            if (!String.IsNullOrEmpty(keylist) && !nokey)
            {
                columnlist += String.Format(
                    ",\r\nCONSTRAINT [{0}] PRIMARY KEY CLUSTERED ({1})",
                    String.Format("PK_{0}", destination.Table.TableName),
                    keylist);
            }

            sql = String.Format(sql, destination.Table.SchemaName, destination.Table.TableName, columnlist);

            return sql;
        }

        /// <summary>
        /// Executest bulk copy to ingest data from the DataReader
        /// </summary>
        /// <param name="dr"></param>
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
    }
}
