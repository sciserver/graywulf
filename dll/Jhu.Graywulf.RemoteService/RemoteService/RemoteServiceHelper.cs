using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security;
using System.Security.Principal;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements method to establish channels with remote services to
    /// execute delegated tasks.
    /// </summary>
    public class RemoteServiceHelper : ServiceHelper
    {
        #region Remote object creation functions

        /// <summary>
        /// Returns a proxy to the remote service control object.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IRemoteServiceControl GetControlObject(string host)
        {
            return CreateChannel<IRemoteServiceControl>(host, "Control", RemoteServiceBase.Configuration.Endpoint);
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
        public static T CreateObject<T>(string host, bool allowInProcess)
            where T : IRemoteService
        {
            var fdqn = DnsHelper.GetFullyQualifiedDnsName(host);

            if (allowInProcess && DnsHelper.IsLocalhost(fdqn))
            {
                // Create the object in-process
                var st = GetServiceType(typeof(T));
                return (T)Activator.CreateInstance(st);
            }
            else
            {
                // Get the uri to the requested service from the remote server
                var sc = GetControlObject(fdqn);
                var uri = sc.GetServiceEndpointUri(typeof(T).AssemblyQualifiedName);
                var ep = CreateEndpointAddress(uri, RemoteServiceBase.Configuration.Endpoint.ServicePrincipalName);
                return CreateChannel<T>(ep);
            }
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
