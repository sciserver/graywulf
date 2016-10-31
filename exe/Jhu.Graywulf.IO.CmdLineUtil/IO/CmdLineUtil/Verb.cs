using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.IO.CmdLineUtil
{
    [Verb]
    abstract class Verb
    {
        public abstract void Run();
    }
}
