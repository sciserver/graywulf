using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    public abstract class ServiceInstallerBase : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;
        protected System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        protected System.ServiceProcess.ServiceInstaller serviceInstaller;

        public ServiceInstallerBase()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this.serviceProcessInstaller,
                this.serviceInstaller});
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
