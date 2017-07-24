using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Jhu.Graywulf.Scheduler
{
    [RunInstaller(true)]
    public partial class SchedulerInstaller : Jhu.Graywulf.Components.ServiceInstallerBase
    {
        public SchedulerInstaller()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            this.serviceInstaller.DisplayName = "Graywulf Scheduler Service";
            this.serviceInstaller.ServiceName = "gwscheduler";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
        }
    }
}
