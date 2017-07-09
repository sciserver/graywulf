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
        private Jhu.Graywulf.RemoteService.Server.RemoteService rs;

        public static RemoteServiceTester Instance
        {
            get { return CrossAppDomainSingleton<RemoteServiceTester>.Instance; }
        }

        public RemoteServiceTester()
        {
        }

        protected override void OnStart(object options)
        {
            rs = new Jhu.Graywulf.RemoteService.Server.RemoteService();
            rs.Start(null);
        }

        protected override void OnStop()
        {
            rs.Stop();
        }

        public void DrainStop()
        {
            rs.Stop();
        }

        public void Kill()
        {
            rs.Stop();
        }
    }

}
