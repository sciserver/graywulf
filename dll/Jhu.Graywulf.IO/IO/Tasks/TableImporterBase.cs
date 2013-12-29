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
using Jhu.Graywulf.SqlParser.SqlCodeGen;

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

        protected void DropDestinationTable()
        {
            string sql = @"
IF (OBJECT_ID('[{0}].[{1}].[{2}]') IS NOT NULL)
BEGIN
    DROP {3} [{0}].[{1}].[{2}]
END";

            sql = String.Format(
                sql,
                !String.IsNullOrWhiteSpace(Destination.Table.DatabaseName) ? Destination.Table.DatabaseName : Destination.Table.Dataset.DatabaseName,
                Destination.Table.SchemaName,
                Destination.Table.ObjectName,
                Destination.Table.GetType().Name);

            // TODO: move this to schema eventually
            using (var cn = new SqlConnection(destination.Table.Dataset.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Creates the destination table
        /// </summary>
        /// <param name="schemaTable"></param>
        protected void CreateDestinationTable(DataTable schemaTable)
        {
            // Generate create table SQL
            var cg = new SqlServerCodeGenerator();
            var sql = cg.GenerateCreateDestinationTableQuery(schemaTable, destination.Table);

            // Execute CREATE TABLE query on destination
            using (var cn = new SqlConnection(destination.Table.Dataset.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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
