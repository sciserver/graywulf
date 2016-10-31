using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "AddUser", Description = "Adds a windows user to the database")]
    class AddUser : CreateDb
    {
        public override void Run()
        {
            if (!Quiet)
            {
                Console.Write("Creating database user... ");
            }

            var di = new DBInstaller(GetConnectionString());
            di.AddUser(Username, Role);

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }
    }
}
