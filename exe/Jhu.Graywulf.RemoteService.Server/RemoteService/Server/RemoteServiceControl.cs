using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Security;
using System.Security.Principal;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.ServiceModel;

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
        public Task<string> HelloAsync()
        {
            var res = GetType().Assembly.FullName;
            RemoteService.LogDebug("Hello called on {0}", res);
            return Task.FromResult(res);
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public Task<AuthenticationDetails> WhoAmIAsync()
        {
            // Switch to windows principal
            var id = WindowsIdentity.GetCurrent();
            var details = new AuthenticationDetails()
            {
                Name = id.Name,
                IsAuthenticated = id.IsAuthenticated,
                AuthenticationType = id.AuthenticationType,
            };

            RemoteService.LogDebug("Client is {0} and {1}authenticated", details.Name, details.IsAuthenticated ? "" : "not ");

            return Task.FromResult(details);
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public Task<AuthenticationDetails> WhoAreYouAsync()
        {
            var id = WindowsIdentity.GetCurrent();
            var details = new AuthenticationDetails()
            {
                Name = id.Name,
                IsAuthenticated = id.IsAuthenticated,
                AuthenticationType = id.AuthenticationType,
            };

            RemoteService.LogDebug("Server is {0} and {1}authenticated", details.Name, details.IsAuthenticated ? "" : "not ");

            return Task.FromResult(details);
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public Task<Uri> GetServiceEndpointUriAsync(string contractType)
        {
            var contract = Type.GetType(contractType);

            // TODO: need to remove synchronization here
            lock (RemoteService.SyncRoot)
            {
                if (RemoteService.RegisteredEndpoints.ContainsKey(contract.FullName))
                {
                    return Task.FromResult(RemoteService.RegisteredEndpoints[contract.FullName].Address.Uri);
                }
                else
                {
                    return Task.FromResult(RemoteService.RegisterService(contract));
                }
            }
        }

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        public Task<string[]> QueryRegisteredServicesAsync()
        {
            // TODO: remove synchronization if possible
            lock (RemoteService.SyncRoot)
            {
                return Task.FromResult(RemoteService.RegisteredServiceHosts.Keys.ToArray());
            }
        }
    }
}
