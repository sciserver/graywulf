using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ExportTables;

namespace Jhu.Graywulf.Install
{
    public class ExportMaintenanceJobInstaller : ContextObject
    {
        private Federation federation;

        public ExportMaintenanceJobInstaller(Federation federation)
        {
            this.federation = federation;
        }

        public void Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = typeof(Jobs.ExportTables.ExportMaintenanceJob).Name,
                System = federation.System,
                WorkflowTypeName = typeof(Jobs.ExportTables.ExportMaintenanceJob).AssemblyQualifiedName,
                // TODO: add settings
            };

            jd.DiscoverWorkflowParameters();
            jd.Save();
        }
    }
}
