using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateSchema", Description = "Creates the database schema required for storing the cluster registry.")]
    class CreateSchema : Verb
    {
        public override void Run()
        {
            base.Run();

            Console.Write("Creating database schema... ");

            var i = new RegistryInstaller();
            i.CreateSchema();

            Console.WriteLine("done.");
        }
    }
}
