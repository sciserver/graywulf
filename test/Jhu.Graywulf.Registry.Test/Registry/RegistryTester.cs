using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Registry
{
    public class RegistryTester : ServiceTesterBase
    {
        public static RegistryTester Instance
        {
            get { return CrossAppDomainSingleton<RegistryTester>.Instance; }
        }

        public RegistryTester()
        {
        }

        protected override void OnStart(object state)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
