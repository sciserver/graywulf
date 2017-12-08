using System;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements method to establish channels with remote services to
    /// execute delegated tasks.
    /// </summary>
    public class RemoteServiceHelper : ServiceHelper
    {
        #region Remote object creation functions

        public static ServiceProxy<IRemoteServiceControl> GetControlObject(string host)
        {
            return GetControlObject(host, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Returns a proxy to the remote service control object.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ServiceProxy<IRemoteServiceControl> GetControlObject(string host, TimeSpan timeout)
        {
            return CreateChannel<IRemoteServiceControl>(host, "Control", RemoteServiceBase.Configuration.Endpoint, timeout);
        }

        public static ServiceProxy<T> CreateObject<T>(CancellationContext cancellationContext, string host, bool allowInProcess)
            where T : IRemoteService
        {
            return CreateObject<T>(cancellationContext, host, allowInProcess, TimeSpan.FromSeconds(Constants.DefaultChannelTimeout));
        }

        /// <summary>
        /// Returns a proxy to a custom service object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <remarks>
        /// If the remote service does not serve the requested type of service yet, the
        /// function registers it before creating a proxy.
        /// </remarks>
        public static ServiceProxy<T> CreateObject<T>(CancellationContext cancellationContext, string host, bool allowInProcess, TimeSpan timeout)
            where T : IRemoteService
        {
            // TODO: make this async

            var fdqn = DnsHelper.GetFullyQualifiedDnsName(host);
            ServiceProxy<T> res;

            if (allowInProcess && DnsHelper.IsLocalhost(fdqn))
            {
                // Create the object in-process
                var st = GetServiceType(typeof(T));
                res = new ServiceProxy<T>((T)Activator.CreateInstance(st));  // This calls the parameterless constructor!
                res.Value.CancellationContext = cancellationContext;
            }
            else
            {
                // Get the uri to the requested service from the remote server
                var sc = GetControlObject(fdqn, timeout);
                var uri = sc.Value.GetServiceEndpointUri(typeof(T).AssemblyQualifiedName);
                var ep = CreateEndpointAddress(uri, RemoteServiceBase.Configuration.Endpoint.ServicePrincipalName);
                res = CreateChannel<T>(ep, timeout);
                cancellationContext.Register(res.Value);
            }
            
            return res;
        }

        #endregion

        /// <summary>
        /// Returns the type implementing a service contract. The contract
        /// needs to be decorated with a RemoteServiceAttribute
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static Type GetServiceType(Type contract)
        {
            // See if contractType is decorated with the RemoteServiceClassAttribute
            var attr = contract.GetCustomAttributes(typeof(RemoteServiceAttribute), false);

            if (attr == null || attr.Length != 1)
            {
                // TODO
                throw new InvalidOperationException("Contracts must be decorated with the RemoteServiceClassAttribute for automatic service registration.");
            }

            var serviceType = ((RemoteServiceAttribute)attr[0]).Type.AssemblyQualifiedName;
            var service = Type.GetType(serviceType);

            if (!service.IsSubclassOf(typeof(RemoteServiceBase)))
            {
                // TODO
                throw new InvalidOperationException("Service class must derive from Jhu.Graywulf.RemoteService.RemoteServiceBase");
            }

            if (service == null || contract == null)
            {
                throw new Exception("Type not found.");    // TODO
            }

            return service;
        }
    }
}
