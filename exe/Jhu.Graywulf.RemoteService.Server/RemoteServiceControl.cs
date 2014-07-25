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
    /// <summary>
    /// Implements the remote service control interface to dynamically
    /// invoke remote functions.
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true)]
    class RemoteServiceControl : IRemoteServiceControl
    {
        [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation]
        public string Hello()
        {
            return GetType().Assembly.FullName;
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        [LimitedAccessOperation]
        public void WhoAmI(out string name, out bool isAuthenticated, out string authenticationType)
        {
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
        [LimitedAccessOperation]
        public Uri GetServiceEndpointUri(string contractType)
        {
            var contract = Type.GetType(contractType);

            // TODO: need to remove synchronization here
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
        [LimitedAccessOperation]
        public string[] QueryRegisteredServices()
        {
            // TODO: remove synchronization if possible
            lock (RemoteService.SyncRoot)
            {
                return RemoteService.RegisteredServiceHosts.Keys.ToArray();
            }
        }
    }
}
