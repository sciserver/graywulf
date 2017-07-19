using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class ConsoleLogWriter : StreamLogWriter
    {
        public static ConsoleLogWriterConfiguration Configuration
        {
            get
            {
                return (ConsoleLogWriterConfiguration)ConfigurationManager.GetSection("jhu.graywulf/logging/console");
            }
        }

        protected override void OnStart()
        {
            Open(Console.Out);
        }

        protected override void OnStop()
        {
            Close();
        }
    }
}
