using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ExportTables;

namespace Jhu.Graywulf.Install
{
    public class ExportTablesJobInstaller : InstallerBase
    {
        private Federation federation;

        public ExportTablesJobInstaller(Federation federation)
        {
            this.federation = federation;
        }

        public void Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = typeof(Jobs.ExportTables.ExportTablesJob).Name,
                System = federation.System,
                WorkflowTypeName = GetUnversionedTypeName(typeof(Jobs.ExportTables.ExportTablesJob)),
                Settings = new ExportTablesJobSettings()
                {
                    // TODO: use installer parameter instead
                    OutputDirectory = String.Format(@"\\{0}\ExportOutput\", federation.ControllerMachine.HostName)
                }
            };

            jd.DiscoverWorkflowParameters();
            jd.Save();
        }
    }
}
