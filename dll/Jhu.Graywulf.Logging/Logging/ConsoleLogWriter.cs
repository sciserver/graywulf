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

        public override void Start()
        {
            Open(Console.Out);
        }

        public override void Stop()
        {
            Close();
        }
    }
}
