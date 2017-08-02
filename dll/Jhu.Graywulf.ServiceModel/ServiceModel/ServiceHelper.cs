using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.ServiceModel
{
    public class ServiceHelper
    {
        public const ImpersonationOption DefaultImpersonation = ImpersonationOption.Allowed;

        private Type contractType;
        private Type serviceType;
        private string hostName;
        private string serviceName;
        private string configSection;
        private ITcpEndpointConfiguration configuration;
        private ServiceHost host;
        private ServiceEndpoint endpoint;

        public Type ContractType
        {
            get { return contractType; }
            set { contractType = value; }
        }

        public Type ServiceType
        {
            get { return serviceType; }
            set { serviceType = value; }
        }

        public string HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }

        public string ConfigSection
        {
            get { return configSection; }
            set { configSection = value; }
        }

        public ServiceHost Host
        {
            get { return host; }
        }

        public ServiceEndpoint Endpoint
        {
            get { return endpoint; }
        }

        public ServiceHelper()
        {
            InitializeMembers();
        }

        public ServiceHelper(Type contractType, Type serviceType, string serviceName, string configSection)
        {
            InitializeMembers();

            this.contractType = contractType;
            this.serviceType = serviceType;
            this.serviceName = serviceName;
            this.configSection = configSection;
        }

        private void InitializeMembers()
        {
            this.contractType = null;
            this.serviceType = null;
            this.hostName = DnsHelper.LocalhostFqdn;    // This determines how other machines will address the server
            this.serviceName = null;
            this.configSection = null;
            this.configuration = null;
            this.host = null;
            this.endpoint = null;
        }

        /// <summary>
        /// Creates a new type of service identified by its contract.
        /// </summary>
        /// <returns></returns>
        public void CreateService()
        {
            configuration = (ITcpEndpointConfiguration)ConfigurationManager.GetSection(configSection);
            
            var ep = CreateEndpointUri(hostName, configuration.Endpoint.TcpPort, serviceName);
            var tcp = CreateNetTcpBinding();

            host = new ServiceHost(serviceType, ep);
            endpoint = host.AddServiceEndpoint(contractType, tcp, ep);

            // Turn on detailed debug info
#if DEBUG
            TurnOnDetailedDebugInfo();
#endif

            // Turn on impersonation
            // TODO: not used, requires setting up SPNs in domain
            // TurnOnInpersonations(host);

            TurnOnUnthrottling();
            TurnOnLogging();
            TurnOnAccessControl();
            
            host.Open();
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
        public static EndpointAddress CreateEndpointAddress(string hostName, int tcpPort, string serviceName, string endpointSpn)
        {
            var uri = CreateEndpointUri(hostName, tcpPort, serviceName);
            return CreateEndpointAddress(uri, endpointSpn);
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
        public static EndpointAddress CreateEndpointAddress(Uri uri, string endpointSpn)
        {
            EndpointAddress ea;

            if (DnsHelper.IsLocalhost(uri.Host))
            {
                // Localhost
                ea = new EndpointAddress(uri);
            }
            else
            {
                // Generic
                ea = new EndpointAddress(uri, new SpnEndpointIdentity(endpointSpn));
            }

            return ea;
        }

        public static Uri CreateEndpointUri(string hostName, int tcpPort, string serviceName)
        {
            return new Uri(String.Format("net.tcp://{0}:{1}/{2}", hostName, tcpPort, serviceName));
        }

        private void TurnOnDetailedDebugInfo()
        {
            var sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(sdb);
            }
            sdb.IncludeExceptionDetailInFaults = true;
        }

        private void TurnOnInpersonation()
        {
            var sab = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            if (sab == null)
            {
                sab = new ServiceAuthorizationBehavior();
                host.Description.Behaviors.Add(sab);
            }
            sab.ImpersonateCallerForAllOperations = true;
        }

        private void TurnOnUnthrottling()
        {
            // Unthrottle service to increase throughput
            // Service is behind a firewall, no DOS attacks will happen
            // TODO: copy these settings to the control endpoint
            var tb = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (tb == null)
            {
                tb = new ServiceThrottlingBehavior();
                host.Description.Behaviors.Add(tb);
            }
            tb.MaxConcurrentCalls = 1024;
            tb.MaxConcurrentInstances = Int32.MaxValue;
            tb.MaxConcurrentSessions = 1024;
        }

        private void TurnOnLogging()
        {
            var logging = host.Description.Behaviors.Find<ServiceLoggingBehavior>();

            if (logging == null)
            {
                logging = new ServiceLoggingBehavior();
                host.Description.Behaviors.Add(logging);
            }
        }

        private void TurnOnAccessControl()
        {
            var access = host.Description.Behaviors.Find<LimitedAccessServiceBehavior>();

            if (access != null)
            {
                host.Description.Behaviors.Remove(access);
            }

            access = new LimitedAccessServiceBehavior();
            access.ConfigSection = configSection;
            host.Description.Behaviors.Add(access);
        }
    }
}
