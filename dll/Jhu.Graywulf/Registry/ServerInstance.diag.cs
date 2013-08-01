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
            List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

            msg.Add(TestSqlConnection());

            return msg;
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
