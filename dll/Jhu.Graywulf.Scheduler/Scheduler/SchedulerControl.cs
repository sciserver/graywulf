using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security.Principal;

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

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType)
        {
            // Switch to windows principal
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;

            QueueManager.Instance.LogDebug("Client is {0} and {1}authenticated", name, isAuthenticated ? "" : "not ");
        }

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public void WhoAreYou(out string name, out bool isAuthenticated, out string authenticationType)
        {
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;

            QueueManager.Instance.LogDebug("Server is {0} and {1}authenticated", name, isAuthenticated ? "" : "not ");
        }

        public IQueue[] GetQueues()
        {
            // TODO synchronize!!!
            return QueueManager.Instance.Cluster.Queues.Values
                .Select(q => (IQueue)q).ToArray();
        }

        public IJob[] GetJobs(Guid queueGuid)
        {
            // TODO synchronize!!!
            return QueueManager.Instance.Cluster.Queues[queueGuid].Jobs.Values
            .Select(j => (IJob)j).ToArray();
        }

        public IJob GetJob(Guid jobGuid)
        {
            return (IJob)QueueManager.Instance.RunningJobs[jobGuid];
        }

        public void StartJob(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void CancelJob(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void ReloadCluster()
        {
            throw new NotImplementedException();
        }
    }
}
