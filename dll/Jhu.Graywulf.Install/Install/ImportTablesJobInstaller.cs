using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ImportTables;

namespace Jhu.Graywulf.Install
{
    public class ImportTablesJobInstaller : ContextObject
    {
        private Federation federation;

        public ImportTablesJobInstaller(Federation federation)
        {
            this.federation = federation;
        }

        public void Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = typeof(ImportTablesJob).Name,
                System = federation.System,
                WorkflowTypeName = typeof(ImportTablesJob).AssemblyQualifiedName,
                Settings = new ImportTablesJobSettings()
            };

            jd.DiscoverWorkflowParameters();
            jd.Save();
        }
    }
}
