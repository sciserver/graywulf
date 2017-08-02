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
using System.Management;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements method to establish channels with remote services to
    /// execute delegated tasks.
    /// </summary>
    public static class RemoteServiceHelper
    {
        public const ImpersonationOption DefaultImpersonation = ImpersonationOption.Allowed;

        private static readonly string localhost;
        private static readonly string localhostName;
        private static readonly string localhostFqdn;
        private static readonly HashSet<string> localhostIPs;

        static RemoteServiceHelper()
        {
            localhost = "localhost";
            localhostName = GetHostName();
            localhostFqdn = GetFullyQualifiedDnsName();
            localhostIPs = GetIPs();
        }

        #region Remote object creation functions

        /// <summary>
        /// Returns a proxy to the remote service control object.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IRemoteServiceControl GetControlObject(string host)
        {
            var fdqn = GetFullyQualifiedDnsName(host);
            var tcp = CreateNetTcpBinding();
            var ep = CreateEndpointAddress(fdqn, "Control");

            return CreateChannel<IRemoteServiceControl>(tcp, ep);
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
            var fdqn = GetFullyQualifiedDnsName(host);

            if (allowInProcess && IsLocalhost(fdqn))
            {
                // Create the object in-process
                var st = GetServiceType(typeof(T));
                return (T)Activator.CreateInstance(st);
            }
            else
            {
                // Get the uri to the requested service from the remote server
                var sc = RemoteServiceHelper.GetControlObject(fdqn);
                var uri = sc.GetServiceEndpointUri(typeof(T).AssemblyQualifiedName);

                return CreateChannel<T>(CreateNetTcpBinding(), CreateEndpointAddress(uri));
            }
        }

        /// <summary>
        /// Creates a channel of type T to the given endpoint via the given binding.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tcp"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private static T CreateChannel<T>(NetTcpBinding tcp, EndpointAddress endpoint)
        {
            var cf = new ChannelFactory<T>(tcp, endpoint);

            // Ensure delegation
            cf.Credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            cf.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

            return cf.CreateChannel();
        }

        #endregion

        /// <summary>
        /// Returns the type implementing a service contract. The contract
        /// needs to be decorated with a RemoteServiceAttribute
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private static Type GetServiceType(Type contract)
        {
            // See if contractType is decorated with the RemoteServiceClassAttribute
            var attr = contract.GetCustomAttributes(typeof(RemoteServiceAttribute), false);

            if (attr == null || attr.Length != 1)
            {
                // TODO
                throw new InvalidOperationException("Contracts must be decorated with the RemoteServiceClassAttribute for automatic service registration.");
            }

            var serviceType = ((RemoteServiceAttribute)attr[0]).Type.AssemblyQualifiedName;

            // Attempt to load type
            var service = Type.GetType(serviceType);

            // Validate service implementation
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

        /// <summary>
        /// Returns a NetTcpBinding object initialized to support Kerberos authentication over TCP
        /// </summary>
        /// <returns></returns>
        public static NetTcpBinding CreateNetTcpBinding()
        {
            var tcp = new NetTcpBinding();
            tcp.ReceiveTimeout = TimeSpan.FromHours(2);     // **** TODO
            tcp.SendTimeout = TimeSpan.FromHours(2);
            tcp.OpenTimeout = TimeSpan.FromHours(2);

            // Configure security to use windows credentials and signed messages
            tcp.Security.Mode = SecurityMode.Transport;
            tcp.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcp.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
            tcp.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

            // Large XML messages require these quotas set to large values
            tcp.ReaderQuotas.MaxArrayLength = 0x7FFFFFFF;
            tcp.ReaderQuotas.MaxDepth = 0x7FFFFFFF;
            tcp.ReaderQuotas.MaxStringContentLength = 0x7FFFFFFF;

            // It might be necessary to increase buffer size on large messages
            //tcp.MaxBufferPoolSize = 0x1000000;  // 16M

            return tcp;
        }

        /// <summary>
        /// Creates an EndpointAddress from a hostname and service name.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static EndpointAddress CreateEndpointAddress(string host, string service)
        {
            return CreateEndpointAddress(CreateEndpointUri(host, service));
        }

        /// <summary>
        /// Creates and initializes an EndpointAddress object based on an URI
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <remarks>
        /// The EndpointAddress is initialized to support delegation via Kerberos.
        /// Different initialization for localhost is handled.
        /// </remarks>
        public static EndpointAddress CreateEndpointAddress(Uri uri)
        {
            EndpointAddress ea;

            if (IsLocalhost(uri.Host))
            {
                // Localhost
                ea = new EndpointAddress(uri);
            }
            else
            {
                // Generic
                ea = new EndpointAddress(uri, new SpnEndpointIdentity("Graywulf/RemoteService"));
            }

            return ea;
        }

        public static Uri CreateEndpointUri(string host, string service)
        {
            return new Uri(String.Format("net.tcp://{0}:{1}/{2}", host, RemoteServiceBase.Configuration.TcpPort, service));
        }

        /// <summary>
        /// Creates a new type of service identified by its contract.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static Uri CreateService(Type contract, out ServiceHost host, out ServiceEndpoint endpoint)
        {
            var service = GetServiceType(contract);

            var hostname = localhostFqdn;       // This determines how other machines will address the server
            var ep = RemoteServiceHelper.CreateEndpointUri(hostname, service.FullName);
            var tcp = RemoteServiceHelper.CreateNetTcpBinding();

            host = new ServiceHost(service, ep);

            // Create endpoint
            endpoint = host.AddServiceEndpoint(contract, tcp, ep);

            // Turn on detailed debug info
#if DEBUG
            ServiceHelper.TurnOnDetailedDebugInfo(host);
#endif

            // Turn on impersonation
            // TODO: not used, requires setting up SPNs in domain
            // TurnOnInpersonations(host);

            ServiceHelper.TurnOnUnthrottling(host);
            ServiceHelper.TurnOnLogging(host);
            TurnOnAccessControl(host);

            host.Open();

            return endpoint.Address.Uri;
        }

        public static void TurnOnAccessControl(ServiceHost host)
        {
            var access = new LimitedAccessServiceExtension();
            access.AddUser(Constants.Default, RemoteServiceBase.Configuration.UserGroup);
            access.AddRole(Constants.Default, RemoteServiceBase.Configuration.UserGroup);
            host.Extensions.Add(access);
        }

        /// <summary>
        /// Returns the host name of the machine.
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            var ipprop = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            return ipprop.HostName;
        }

        private static HashSet<string> GetIPs()
        {
            var res = new HashSet<string>();
            res.Add("127.0.0.1");

            var query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'";
            var moSearch = new ManagementObjectSearcher(query);
            var moCollection = moSearch.Get();

            // Every record in this collection is a network interface
            foreach (ManagementObject mo in moCollection)
            {
                // IPAddresses, probably have more than one value
                string[] addresses = (string[])mo["IPAddress"];

                for (int i = 0; i < addresses.Length; i++)
                {
                    if (!res.Contains(addresses[i]))
                    {
                        res.Add(addresses[i]);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the DNS name of the current machine.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function should return a fully and well qualified domain name,
        /// otherwise authentication wont work. To set up DNS names correctly on
        /// machines outside a windows domain, the primary DNS suffix must be set
        /// in the computer name configurations. ipconfig /all should give the
        /// currently set suffix.
        /// </remarks>
        public static string GetFullyQualifiedDnsName()
        {
            var ipprop = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            string fqdn;

            try
            {
                fqdn = String.Format("{0}.{1}", ipprop.HostName, ipprop.DomainName);
            }
            catch (Exception)
            {
                fqdn = ipprop.HostName;
            }

            return fqdn;
        }

        /// <summary>
        /// Returns the DNS name of any host identified by its host name.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string GetFullyQualifiedDnsName(string host)
        {
            // If host is localhost, simply replace it with the known host name,
            // otherwise windows authentication will not work within the local
            // machine

            // TODO: reverse lookup to get FQDN, it fails on current GW config at JHU!

            string name;

            if (StringComparer.InvariantCultureIgnoreCase.Compare(host, "localhost") == 0)
            {
                name = localhostFqdn;
            }
            else if (host.IndexOf('.') > -1)
            {
                // Assume it's FQDN
                name = host;
            }
            else
            {
                name = System.Net.Dns.GetHostEntry(host).HostName;
            }

            return name;
        }

        /// <summary>
        /// Returns true if the host is the local host
        /// </summary>
        /// <param name="fdqn"></param>
        /// <returns></returns>
        private static bool IsLocalhost(string fqdn)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;

            if (comparer.Compare(localhost, fqdn) == 0)
            {
                return true;
            }
            else if (comparer.Compare(localhostFqdn, fqdn) == 0)
            {
                return true;
            }
            else if (fqdn.IndexOf('.') > -1 && comparer.Compare(localhostName, fqdn) == 0)
            {
                // localhostName might not be a fully qualified domain name, hence we check
                // for the dot above
                return true;
            }
            else if (localhostIPs.Contains(fqdn))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
