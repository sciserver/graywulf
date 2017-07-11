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
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.RemoteService.Server
{
    [Serializable]
    public partial class RemoteService : ServiceBase
    {
        private static object syncRoot;
        private static int eventOrder;

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
            AppDomain.CurrentDomain.UnhandledException += Util.ServiceHelper.WriteErrorDump;

            // Initialize logger
            // TODO: add interactive mode, set source
            Logger.Instance.Start(false);

            // Log starting event
            Logger.Instance.LogStatus(
                "Graywulf Remote Service has started.", 
                null,
                new Dictionary<string, object>() { { "UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName) } });

            // In case the server has been just rebooted
            // wait for the Windows Process Activation Service (WAS)
            if (Util.ServiceHelper.IsServiceInstalled("WAS"))
            {
                Util.ServiceHelper.WaitForService("WAS", 1000, 500);
            }

            // Initialize WCF service host to run the control service
            // TODO: the localhost setting needs to be tested!
            var fdqn = RemoteServiceHelper.GetFullyQualifiedDnsName();
            var tcp = RemoteServiceHelper.CreateNetTcpBinding();
            var ep = RemoteServiceHelper.CreateEndpointUri(fdqn, "Control");

            controlServiceHost = new ServiceHost(
                typeof(RemoteServiceControl), ep);

            controlEndpoint = controlServiceHost.AddServiceEndpoint(
                typeof(IRemoteServiceControl), tcp, ep);

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

            Logger.Instance.LogStatus("Graywulf Remote Service has stopped.", 
                null,
                new Dictionary<string, object>() { { "UserAccount", String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName) } });

            Logger.Instance.Stop();            
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

        /// <summary>
        /// Registers a new service host and endpoint for a contract type
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static Uri RegisterService(Type contract)
        {
            ServiceHost host;
            ServiceEndpoint endpoint;

            var uri = RemoteServiceHelper.CreateService(contract, out host, out endpoint);

            // TODO: remove synchronization here
            lock (syncRoot)
            {
                registeredServiceHosts.Add(contract.FullName, host);
                registeredEndpoints.Add(contract.FullName, endpoint);
            }

            return uri;
        }


        // TODO: delete
        // *** TODO: how to log if event order is not available (in context)
        /*
        /// <summary>
        /// Logs scheduler events
        /// </summary>
        /// <param name="e"></param>
        static private void LogEvent(Event e)
        {
            e.UserGuid = Guid.Empty;
            e.Source = EventSource.RemoteService;
            e.ExecutionStatus = ExecutionStatus.Closed;

            e.JobGuid = Guid.Empty;
            e.ContextGuid = Guid.Empty;
            e.Order = ++eventOrder;

            Logger.Instance.LogEvent(e);
        }
        */
    }
}
