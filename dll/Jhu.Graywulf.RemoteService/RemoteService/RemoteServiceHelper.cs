using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security;
using System.Security.Principal;

namespace Jhu.Graywulf.RemoteService
{
    /// <summary>
    /// Implements method to establish channels with remote services to
    /// execute delegated tasks.
    /// </summary>
    public static class RemoteServiceHelper
    {
        public const ImpersonationOption DefaultImpersonation = ImpersonationOption.Allowed;

        #region Remote object creation functions

        /// <summary>
        /// Returns a proxy to the remote service control object.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IRemoteServiceControl GetControlObject(string host)
        {
            return CreateChannel<IRemoteServiceControl>(
                CreateNetTcpBinding(),
                CreateEndpointAddress(GetFullyQualifiedDnsName(host), "Control"));
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
        public static T CreateObject<T>(string host)
            where T : IRemoteService
        {
            // Get the uri to the requested service from the remote server
            var sc = RemoteServiceHelper.GetControlObject(GetFullyQualifiedDnsName(host));
            var uri = sc.GetServiceEndpointUri(typeof(T).AssemblyQualifiedName);

            return CreateChannel<T>(CreateNetTcpBinding(), CreateEndpointAddress(uri));
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
            cf.Credentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;

            return cf.CreateChannel();
        }

        #endregion

        /// <summary>
        /// Returns a NetTcpBinding object initialized to support Kerberos authentication over TCP
        /// </summary>
        /// <returns></returns>
        public static NetTcpBinding CreateNetTcpBinding()
        {
            var tcp = new NetTcpBinding(SecurityMode.Transport);
            tcp.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcp.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
            tcp.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcp.ReceiveTimeout = TimeSpan.FromHours(2);     // **** TODO
            tcp.SendTimeout = TimeSpan.FromHours(2);
            tcp.OpenTimeout = TimeSpan.FromHours(2);

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

            if (StringComparer.InvariantCultureIgnoreCase.Compare(uri.Host, GetFullyQualifiedDnsName()) == 0)
            {
                ea = new EndpointAddress(uri);
            }
            else
            {
                ea = new EndpointAddress(uri, new SpnEndpointIdentity("Graywulf/RemoteService"));
            }

            return ea;
        }

        public static Uri CreateEndpointUri(string host, string service)
        {
            return new Uri(String.Format("net.tcp://{0}:{1}/{2}", host, AppSettings.TcpPort, service));
        }

        /// <summary>
        /// Authorizes the user only if they belong to a specified user group.
        /// </summary>
        public static void EnsureRoleAccess()
        {
            // Access automatically granted for non-remoting scenarios.
            // OperationContext.Current is null if the object is created locally.
            // Otherwise check if the user is authenticated and the identity is equal to the specified
            // user (group) name or member of the given group (role).

            if (OperationContext.Current != null &&
                StringComparer.InvariantCultureIgnoreCase.Compare(Thread.CurrentPrincipal.Identity.Name, AppSettings.UserGroup) != 0 &&
                !Thread.CurrentPrincipal.IsInRole(AppSettings.UserGroup))
            {
                throw new SecurityException("Access denied.");
            }
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

            // TODO: reverse lookup to get FQDN, it fails on current GW config at JHU!
            /*var name = string.Format("{0}.{1}", ipprop.HostName, ipprop.DomainName);
            return name;*/

            return ipprop.HostName;
        }

        /// <summary>
        /// Returns the DNS name of any host identified by its host name.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string GetFullyQualifiedDnsName(string host)
        {
            // TODO: reverse lookup to get FQDN, it fails on current GW config at JHU!
            /*var name = System.Net.Dns.GetHostEntry(host).HostName;
            return name;*/

            return host;
        }
    }
}
