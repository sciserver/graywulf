using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;
using System.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.RemoteService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public abstract class RemoteServiceBase : CancelableTask, IRemoteService
    {
        protected void EnsureRoleAccess()
        {
            RemoteServiceHelper.EnsureRoleAccess();
        }

        public override bool IsCanceled
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return base.IsCanceled; }
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public override void Execute()
        {
            EnsureRoleAccess();
            base.Execute();
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public override void BeginExecute()
        {
            EnsureRoleAccess();
            base.BeginExecute();
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public override void EndExecute()
        {
            EnsureRoleAccess();
            base.EndExecute();
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public override void Cancel()
        {
            EnsureRoleAccess();
            base.Cancel();
        }
    }
}
