using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateRegistry", Description = "Creates the database schema required for storing the cluster registry.")]
    class CreateRegistry : AddUser
    {
        public override void Run()
        {
            base.Run();

            Console.Write("Creating database... ");

            var i = new RegistryInstaller(ContextManager.Instance.ConnectionString);
            i.CreateDatabase();
            i.CreateSchema();
            i.AddUser(Username);

            Console.WriteLine("done.");
        }
    }
}
