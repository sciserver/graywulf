using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseDefinition
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

            // Get schema source server
            // Only for federations, cluster level DBs don't have schemas (TEMP)

            if (DeploymentState == DeploymentState.Deployed && Parent is Federation)
            {
                msg.Add(TestSqlConnection());
            }

            return msg;
        }

        private DiagnosticMessage TestSqlConnection()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = GetFullyQualifiedName(),
                NetworkName = GetSchemaDatabaseInstance().ServerInstance.GetCompositeName(),
                ServiceName = "SQL Connection to Schema Source Server"
            };

            ServerInstance.RunDiagnostics(GetSchemaConnectionString().ConnectionString, msg);

            return msg;
        }
    }
}
