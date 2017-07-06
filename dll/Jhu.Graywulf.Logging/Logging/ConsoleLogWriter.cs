using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    public class ConsoleLogWriter : StreamLogWriter
    {
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
