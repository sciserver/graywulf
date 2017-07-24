using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace Jhu.Graywulf.Components
{
    public abstract class ServiceBase : System.ServiceProcess.ServiceBase
    {
        #region Command line entrypoints

        internal void Start(string[] args)
        {
            OnStart(args);
        }

        internal new void Stop()
        {
            OnStop();
        }

        #endregion
    }
}
