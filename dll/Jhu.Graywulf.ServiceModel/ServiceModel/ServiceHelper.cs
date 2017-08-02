using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Jhu.Graywulf.ServiceModel
{
    public static class ServiceHelper
    {
        public static void TurnOnDetailedDebugInfo(ServiceHost host)
        {
            var sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (sdb == null)
            {
                sdb = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(sdb);
            }
            sdb.IncludeExceptionDetailInFaults = true;
        }

        public static void TurnOnInpersonation(ServiceHost host)
        {
            var sab = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            if (sab == null)
            {
                sab = new ServiceAuthorizationBehavior();
                host.Description.Behaviors.Add(sab);
            }
            sab.ImpersonateCallerForAllOperations = true;
        }

        public static void TurnOnUnthrottling(ServiceHost host)
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

        public static void TurnOnLogging(ServiceHost host)
        {
            var logging = host.Description.Behaviors.Find<ServiceLoggingBehavior>();

            if (logging == null)
            {
                logging = new ServiceLoggingBehavior();
                host.Description.Behaviors.Add(logging);
            }
        }

        public static void TurnOnAccessControl(ServiceHost host, string configSection)
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
