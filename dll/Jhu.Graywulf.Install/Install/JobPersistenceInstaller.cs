using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Install
{
    public class JobPersistenceInstaller : DBInstaller
    {
        public JobPersistenceInstaller()
        {
        }

        public JobPersistenceInstaller(string connectionString)
            : base(connectionString)
        {
        }

        public override void CreateSchema()
        {
            var dir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            dir = System.IO.Path.Combine(dir, "SQL\\en");

            ExecuteSqlFile(System.IO.Path.Combine(dir, "SqlWorkflowInstanceStoreSchema.sql"));
            ExecuteSqlFile(System.IO.Path.Combine(dir, "SqlWorkflowInstanceStoreSchemaUpgrade.sql"));
            ExecuteSqlFile(System.IO.Path.Combine(dir, "SqlWorkflowInstanceStoreLogic.sql"));
        }
    }
}
