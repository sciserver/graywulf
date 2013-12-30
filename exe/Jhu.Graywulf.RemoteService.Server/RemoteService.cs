using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.RemoteService.Server
{
    [Serializable]
    public partial class RemoteService : ServiceBase
    {
        private static object syncRoot;

        private static ServiceHost controlServiceHost;
        private static ServiceEndpoint controlEndpoint;

        private static Dictionary<string, ServiceHost> registeredServiceHosts;
        private static Dictionary<string, ServiceEndpoint> registeredEndpoints;

        public static object SyncRoot
        {
            get { return syncRoot; }
        }

        public static Dictionary<string, ServiceHost> RegisteredServiceHosts
        {
            get { return registeredServiceHosts; }
        }

        public static Dictionary<string, ServiceEndpoint> RegisteredEndpoints
        {
            get { return registeredEndpoints; }
        }

        static RemoteService()
        {
            syncRoot = new object();
            registeredServiceHosts = new Dictionary<string, ServiceHost>();
            registeredEndpoints = new Dictionary<string, ServiceEndpoint>();
        }

        public RemoteService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Initialize WCF service
            controlServiceHost = new ServiceHost(
                typeof(RemoteServiceControl),
                RemoteServiceHelper.CreateEndpointUri(RemoteServiceHelper.GetFullyQualifiedDnsName(), ""));

            controlEndpoint = controlServiceHost.AddServiceEndpoint(
                typeof(IRemoteServiceControl),
                RemoteServiceHelper.CreateNetTcpBinding(),
                RemoteServiceHelper.CreateEndpointUri(RemoteServiceHelper.GetFullyQualifiedDnsName(), "Control"));

            controlServiceHost.Open();
        }

        protected override void OnStop()
        {
            controlServiceHost.Close();
            controlServiceHost = null;

            foreach (var host in registeredServiceHosts.Values)
            {
                host.Close();
            }

            registeredServiceHosts.Clear();
            registeredEndpoints.Clear();
        }

        #region Functions to support command-line execution

        public void Start(string[] args)
        {
            OnStart(args);
        }

        public new void Stop()
        {
            OnStop();
        }

        #endregion

        public static Uri RegisterService(Type contract)
        {
            // See if contractType is decorated with the RemoteServiceClassAttribute
            var attr = contract.GetCustomAttributes(typeof(RemoteServiceClassAttribute), false);

            if (attr == null || attr.Length != 1)
            {
                // TODO
                throw new InvalidOperationException("Contracts must be decorated with the RemoteServiceClassAttribute for automatic service registration.");
            }

            var serviceType = ((RemoteServiceClassAttribute)attr[0]).Type.AssemblyQualifiedName;

            // Attempt to load type
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

            // Everything is OK, initialize service

            lock (syncRoot)
            {
                var host = new ServiceHost(
                    service,
                    RemoteServiceHelper.CreateEndpointUri(RemoteServiceHelper.GetFullyQualifiedDnsName(), ""));

                // Turn on detailed debug info
                var sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (sdb == null)
                {
                    sdb = new ServiceDebugBehavior();
                    host.Description.Behaviors.Add(sdb);
                }
                sdb.IncludeExceptionDetailInFaults = true;

                // Turn on impersonation
                /*
                var sab = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
                if (sab == null)
                {
                    sab = new ServiceAuthorizationBehavior();
                    host.Description.Behaviors.Add(sab);
                }
                sab.ImpersonateCallerForAllOperations = true;
                */

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

                var endpoint = host.AddServiceEndpoint(
                    contract,
                    RemoteServiceHelper.CreateNetTcpBinding(),
                    RemoteServiceHelper.CreateEndpointUri(RemoteServiceHelper.GetFullyQualifiedDnsName(), service.FullName));
                
                host.Open();

                registeredServiceHosts.Add(contract.FullName, host);
                registeredEndpoints.Add(contract.FullName, endpoint);

                return endpoint.Address.Uri;
            }
        }
    }
}
