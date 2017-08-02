using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Scheduler
{
    internal class SchedulerControl : ISchedulerControl
    {
        public string Hello()
        {
            var res = GetType().Assembly.FullName;
            return res;
        }

        public IQueue[] GetQueues()
        {
            return QueueManager.Instance.Cluster.Queues.Values
                .Select(q => (IQueue)q).ToArray();
        }

        public IJob[] GetJobs(Guid queueGuid)
        {
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
        }
    }
}
