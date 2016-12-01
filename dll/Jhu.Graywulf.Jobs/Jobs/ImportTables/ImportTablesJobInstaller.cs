using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    public class ImportTablesJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(ImportTablesJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.ImportTablesJob; }
        }

        protected override bool IsSystem
        {
            get { return false; }
        }

        public ImportTablesJobInstaller(Federation federation)
            : base(federation)
        {
        }
        
        protected override void CreateSettings(JobDefinition jobDefinition)
        {
            base.CreateSettings(jobDefinition);

            jobDefinition.Settings = new ImportTablesJobSettings();
        }
    }
}
