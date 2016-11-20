using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportMaintenanceJobInstaller : InstallerBase
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
                WorkflowTypeName = GetUnversionedTypeName(typeof(Jobs.ExportTables.ExportMaintenanceJob)),
                // TODO: add settings
            };

            jd.DiscoverWorkflowParameters();
            jd.Save();
        }
    }
}
