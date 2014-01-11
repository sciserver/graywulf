using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class ServerInstance
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            if (Machine.RunningState == Registry.RunningState.Running)
            {
                List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

                msg.Add(TestSqlConnection());

                return msg;
            }
            else
            {
                return base.RunDiagnostics();
            }
        }

        private DiagnosticMessage TestSqlConnection()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = GetFullyQualifiedName(),
                NetworkName = GetCompositeName(),
                ServiceName = "SQL Connection"
            };

            RunDiagnostics(GetConnectionString().ConnectionString, msg);

            return msg;
        }

        internal static void RunDiagnostics(string connectionString, DiagnosticMessage message)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            csb.ConnectTimeout = 5;

            try
            {
                using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
                {
                    cn.Open();
                    cn.Close();
                }

                message.Status = DiagnosticMessageStatus.OK;
            }
            catch (System.Exception ex)
            {
                message.Status = DiagnosticMessageStatus.Error;
                message.ErrorMessage = ex.Message;
            }
        }
    }
}
