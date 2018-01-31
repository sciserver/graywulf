using System;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements methods to execute and cancel delegated long-running tasks.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class RemoteServiceBase : CancelableTask, IRemoteService
    {
        #region Static members
        public static RemoteServiceConfiguration Configuration
        {
            get
            {
                return (RemoteServiceConfiguration)ConfigurationManager.GetSection("jhu.graywulf/remoteService");
            }
        }

        #endregion

        protected RemoteServiceBase()
        {
        }

        protected RemoteServiceBase(CancellationContext cancellationContext)
            :base (cancellationContext)
        {
        }
    }
}
