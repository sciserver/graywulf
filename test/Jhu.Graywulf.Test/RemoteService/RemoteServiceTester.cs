using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Principal;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.RemoteService
{
    public class RemoteServiceTester : ServiceTesterBase
    {
        public static RemoteServiceTester Instance
        {
            get { return CrossAppDomainSingleton<RemoteServiceTester>.Instance; }
        }

        public RemoteServiceTester()
        {
        }

        protected override void OnStart(object options)
        {
            RemoteService.Server.Program.StartDebug();
        }

        protected override void OnStop()
        {
            RemoteService.Server.Program.StopDebug();
        }

        public void DrainStop()
        {
            RemoteService.Server.Program.StopDebug();
        }

        public void Kill()
        {
            RemoteService.Server.Program.StopDebug();
        }
    }

}
