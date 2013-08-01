using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class BulkInsertImporter : TableImporterBase, IBulkInsertImporter
    {
        private BulkInsertParameters parameters;
        

        public BulkInsertParameters Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        #region Constructors and initializers

        public BulkInsertImporter()
        {
            InitializeMembers();
        }

        public BulkInsertImporter(BulkInsertParameters parameters)
        {
            InitializeMembers();

            this.parameters = parameters;
        }

        private void InitializeMembers()
        {
            this.parameters = null;
        }

        #endregion

        protected override void OnExecute()
        {
            var sql = String.Format("BULK INSERT [{0}].[{1}] FROM '{2}'", Destination.Table.SchemaName, Destination.Table.TableName, parameters.Filename);

            var sb = new StringBuilder();

            if (Destination.BulkInsertBatchSize > 0) sb.Append(String.Format(",BATCHSIZE={0}", Destination.BulkInsertBatchSize));
            if (parameters.CheckConstraints) sb.Append(",CHECK_CONSTRAINTS");
            if (parameters.CodePage != null && parameters.CodePage != string.Empty) sb.Append(String.Format(",CODEPAGE='{0}'", parameters.CodePage));
            if (parameters.DataFileType != null && parameters.DataFileType != string.Empty) sb.Append(String.Format(",DATAFILETYPE='{0}'", parameters.DataFileType));
            if (parameters.FieldTerminator != null && parameters.FieldTerminator != string.Empty) sb.Append(String.Format(",FIELDTERMINATOR='{0}'", parameters.FieldTerminator));
            if (parameters.FirstRow != 0) sb.Append(String.Format(",FIRSTROW={0}", parameters.FirstRow));
            if (parameters.FireTriggers) sb.Append(",FIRE_TRIGGERS");
            if (parameters.FormatFile != null && parameters.FormatFile != string.Empty) sb.Append(String.Format(",FORMATFILE='{0}'", parameters.FormatFile));
            if (parameters.KeepIdentity) sb.Append(",KEEPIDENTITY");
            if (parameters.KeepNulls) sb.Append(",KEEPNULLS");
            if (parameters.KilobytesPerBatch != 0) sb.Append(String.Format(",KILOBYTES_PER_BATCH={0}", parameters.KilobytesPerBatch));
            if (parameters.LastRow != 0) sb.Append(String.Format(",LASTROW={0}", parameters.LastRow));
            if (parameters.MaxErrors >= 0) sb.Append(String.Format(",MAXERRORS={0}", parameters.MaxErrors));
            if (parameters.Order != null && parameters.Order != string.Empty) sb.Append(String.Format(",ORDER='{0}'", parameters.Order));
            if (parameters.RowsPerBatch != 0) sb.Append(String.Format(",ROWS_PER_BATCH={0}", parameters.RowsPerBatch));
            if (parameters.RowTerminator != null && parameters.RowTerminator != string.Empty) sb.Append(String.Format(",ROWTERMINATOR='{0}'", parameters.RowTerminator));
            if (parameters.TabLock) sb.Append(",TABLOCK");
            if (parameters.ErrorFile != null && parameters.ErrorFile != string.Empty) sb.Append(String.Format(",ERRORFILE='{0}'", parameters.ErrorFile));
            if (sb.Length != 0)
                sql += " WITH (" + sb.ToString().Substring(1) + ")";

            // Open connection to the database instance and execute command
            using (var cn = new SqlConnection(Destination.Table.Dataset.ConnectionString))
            {
                cn.Open();

                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.CommandType = CommandType.Text;
                    if (Destination.BulkInsertTimeout != 0)
                    {
                        cmd.CommandTimeout = Destination.BulkInsertTimeout;
                    }

                    var guid = Guid.NewGuid();

                    var ccmd = new CancelableDbCommand(cmd);
                    RegisterCancelable(guid, ccmd);

                    RowsAffected = ccmd.ExecuteNonQuery();

                    UnregisterCancelable(guid);
                }
            }
        }
    }
}
