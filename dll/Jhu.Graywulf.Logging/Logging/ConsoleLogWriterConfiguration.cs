using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class ConsoleLogWriterConfiguration : LogWriterConfigurationBase
    {
        protected override LogWriterBase OnCreateLogWriter()
        {
            return new ConsoleLogWriter();
        }
    }
}
