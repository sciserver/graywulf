using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Security;
using System.Security.Principal;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.RemoteService.Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true)]
    class RemoteServiceControl : IRemoteServiceControl
    {
        private void EnsureRoleAccess()
        {
            RemoteServiceHelper.EnsureRoleAccess();
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public string Hello()
        {
            EnsureRoleAccess();

            return GetType().Assembly.FullName;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType)
        {
            EnsureRoleAccess();

            // Switch to windows principal
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.NotAllowed)]
        public void WhoAreYou(out string name, out bool isAuthenticated, out string authenticationType)
        {
            var id = WindowsIdentity.GetCurrent();

            name = id.Name;
            isAuthenticated = id.IsAuthenticated;
            authenticationType = id.AuthenticationType;
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public Uri GetServiceEndpointUri(string contractType)
        {
            EnsureRoleAccess();

            var contract = Type.GetType(contractType);

            lock (RemoteService.SyncRoot)
            {
                if (RemoteService.RegisteredEndpoints.ContainsKey(contract.FullName))
                {
                    return RemoteService.RegisteredEndpoints[contract.FullName].Address.Uri;
                }
                else
                {
                    return RemoteService.RegisterService(contract);
                }
            }
        }

        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        public string[] QueryRegisteredServices()
        {
            EnsureRoleAccess();

            lock (RemoteService.SyncRoot)
            {
                return RemoteService.RegisteredServiceHosts.Keys.ToArray();
            }
        }
    }
}
