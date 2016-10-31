using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    public abstract class DropDb : Verb
    {
        protected void RunInstaller(DBInstaller i)
        {
            if (!Quiet)
            {
                Console.Write("Deleting database... ");
            }
            
            i.DropDatabase(false);

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }
    }
}
