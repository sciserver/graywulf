using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportTablesJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(Jobs.ExportTables.ExportTablesJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.ExportTablesJob; }
        }

        protected override bool IsSystem
        {
            get { return false; }
        }

        public ExportTablesJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override void CreateSettings(JobDefinition jobDefinition)
        {
            base.CreateSettings(jobDefinition);

            jobDefinition.Settings = new ExportTablesJobSettings()
            {
                // TODO: use installer parameter instead
                OutputDirectory = ""   // String.Format(@"\\{0}\ExportOutput\", federation.ControllerMachine.HostName)
            };
        }
    }
}
