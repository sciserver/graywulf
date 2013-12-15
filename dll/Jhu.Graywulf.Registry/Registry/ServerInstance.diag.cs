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

            Util.RunSqlServerDiagnostics(GetConnectionString().ConnectionString, msg);

            return msg;
        }
    }
}
