using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Scheduler
{
    [ServiceContract]
    [ServiceLoggingBehavior]
    public interface ISchedulerControl
    {
        /// <summary>
        /// Returns version information from the server.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        string Hello();

        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        IQueue[] GetQueues();

        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        IJob[] GetJobs(Guid queueGuid);

        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        IJob GetJob(Guid jobGuid);
              
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        void StartJob(Guid guid);

        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        void CancelJob(Guid guid);

        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        void ReloadCluster();
    }
}
