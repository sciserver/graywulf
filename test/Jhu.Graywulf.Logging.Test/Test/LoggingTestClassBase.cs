using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Test
{
    public abstract class LoggingTestClassBase
    {
        protected static void StartLogger()
        {
            // Here we allow storing the context in global static
            new Logging.LoggingContext(true, Components.AmbientContextStoreLocation.All);
            Logging.LoggingContext.Current.StartLogger(Logging.EventSource.Test, false);
        }

        protected static void StopLogger()
        {
            Logging.LoggingContext.Current.StopLogger();
            Logging.LoggingContext.Current.Dispose();
        }
    }
}
