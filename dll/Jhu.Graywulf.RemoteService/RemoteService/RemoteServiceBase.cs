using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;
using System.ServiceModel;
using System.Configuration;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements methods to execute and cancel delegated long-running tasks.
    /// </summary>
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

        public override bool IsCanceled
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return base.IsCanceled; }
        }
        
        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation(Constants.Default)]
        public override void Execute()
        {
            // Modify remote service to do check access when called from service only
            // add wcf handler to check roles instead of doing it from here
            base.Execute();
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation(Constants.Default)]
        public override void BeginExecute()
        {
            base.BeginExecute();
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public override void EndExecute()
        {
            base.EndExecute();
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
