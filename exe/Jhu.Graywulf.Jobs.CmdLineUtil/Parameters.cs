using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Activities;
using System.Threading;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.CmdLineUtil
{
    abstract class Parameters
    {
        private AutoResetEvent workflowCompletedEvent;

        protected string userName;
        protected string password;
        protected bool integratedSecurity;

        [Parameter(Name = "UserName", Description = "User name or e-mail")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        [Parameter(Name = "Password", Description = "Password")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Option(Name = "EnableIntegratedSecurity", Description = "Enable integrated security")]
        public bool IntegratedSecurity
        {
            get { return integratedSecurity; }
            set { integratedSecurity = value; }
        }

        public Parameters()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.userName = null;
            this.password = null;
            this.integratedSecurity = true;
        }

        public abstract void Run();

        protected void RunWorkflow(Type wftype, IDictionary<string, object> par)
        {
            // Create workflow
            var wf = (Activity)Activator.CreateInstance(wftype);
            var wfapp = par == null ? new WorkflowApplication(wf) : new WorkflowApplication(wf, par);

            // Turn on logging to console window
            Jhu.Graywulf.Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.StreamLogWriter(Console.Out));
            wfapp.Extensions.Add(new Jhu.Graywulf.Activities.GraywulfTrackingParticipant());

            // Wire-up workflow runtime events
            wfapp.OnUnhandledException = wfapp_OnUnhandledException;
            wfapp.Completed = wfapp_WorkflowCompleted;
            wfapp.Unloaded = wfapp_WorkflowUnloaded;
            wfapp.Aborted = wfapp_WorkflowAborted;

            workflowCompletedEvent = new AutoResetEvent(false);
            wfapp.Run();

            workflowCompletedEvent.WaitOne();
        }

        private UnhandledExceptionAction wfapp_OnUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs e)
        {
            return UnhandledExceptionAction.Abort;
        }

        private void wfapp_WorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
        }

        private void wfapp_WorkflowUnloaded(WorkflowApplicationEventArgs e)
        {
            workflowCompletedEvent.Set();
        }

        private void wfapp_WorkflowAborted(WorkflowApplicationEventArgs e)
        {
            workflowCompletedEvent.Set();
        }

        protected void SignIn(Cluster cluster)
        {
            cluster.LoadDomains(false);
            var domain = cluster.Domains[Registry.Constants.SharedDomainName];

            var uu = new UserFactory(cluster.Context);
            uu.LoginUser(domain, userName, password);
        }
    }
}
