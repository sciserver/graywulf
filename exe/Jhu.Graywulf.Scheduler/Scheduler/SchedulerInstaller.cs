using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Jhu.Graywulf.BulkOp.Server
{
    [RunInstaller(true)]
    public partial class SchedulerInstaller : Installer
    {
        public SchedulerInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);

            OverrideServiceName();
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
            OverrideServiceName();
        }

        private void OverrideServiceName()
        {
            if (Context.Parameters.ContainsKey("svcname"))
            {
                Console.WriteLine("Service name is overriden.");
                this.serviceInstaller.ServiceName = Context.Parameters["svcname"];
                this.serviceInstaller.DisplayName += " (" + Context.Parameters["svcname"] + ")";
            }
        }
    }
}
