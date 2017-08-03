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

        /// <summary>
        /// Returns info about the user under which the server operations run.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAuthenticated"></param>
        /// <param name="authenticationType"></param>
        [OperationContract]
        [LimitedAccessOperation(Constants.DefaultRole)]
        void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType);

        [OperationContract]
        void WhoAreYou(out string name, out bool isAuthenticated, out string authenticationType);

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
