using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateDB", Description = "Creates the database schema required for storing the cluster registry.")]
    class CreateDb : Verb
    {
        public override void Run()
        {
            Console.Write("Creating database schema... ");

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var i = new DBInstaller();

                i.CreateSchema();
            }

            Console.WriteLine("done.");
        }
    }
}
