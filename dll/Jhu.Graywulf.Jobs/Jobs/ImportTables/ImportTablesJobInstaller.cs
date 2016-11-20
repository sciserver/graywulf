using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    public class ImportTablesJobInstaller : InstallerBase
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
                WorkflowTypeName = GetUnversionedTypeName(typeof(ImportTablesJob)),
                Settings = new ImportTablesJobSettings()
            };

            jd.DiscoverWorkflowParameters();
            jd.Save();
        }
    }
}
