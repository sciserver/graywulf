using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security.Principal;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.Scheduler
{
    internal class SchedulerControl : ISchedulerControl
    {
        public string Hello()
        {
            var res = GetType().Assembly.FullName;
            QueueManager.Instance.LogDebug("Hello called on {0}", res);
            return res;
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType)
        {
            // Switch to windows principal
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;

            QueueManager.Instance.LogDebug("Client is {0} and {1}authenticated", name, isAuthenticated ? "" : "not ");
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public void WhoAreYou(out string name, out bool isAuthenticated, out string authenticationType)
        {
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;

            QueueManager.Instance.LogDebug("Server is {0} and {1}authenticated", name, isAuthenticated ? "" : "not ");
        }

        public Queue[] GetQueues()
        {
            return QueueManager.Instance.GetQueues();
        }

        public Job[] GetJobs(Guid queueGuid)
        {
            return QueueManager.Instance.GetJobs(queueGuid);
        }

        public Job GetJob(Guid jobGuid)
        {
            return QueueManager.Instance.GetJob(jobGuid);
        }

        public void StartJob(Guid jobGuid)
        {
            QueueManager.Instance.InjectStartJob(jobGuid);
        }

        public void CancelJob(Guid jobGuid)
        {
            QueueManager.Instance.InjectCancelJob(jobGuid);
        }

        public void ReloadCluster()
        {
            QueueManager.Instance.ReloadCluster();
        }

        public void FlushSchema()
        {
            QueueManager.Instance.FlushSchema();
        }
    }
}
