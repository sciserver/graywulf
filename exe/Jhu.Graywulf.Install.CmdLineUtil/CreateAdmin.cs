using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateAdmin", Description = "Creates the admin usergroup and the admin user account for an existing cluster.")]
    class CreateAdmin : CreateCluster
    {
        public override void Run()
        {
            base.Run();

            Console.Write("Creating admin user... ");

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                try
                {
                    var f = new EntityFactory(context);
                    var c = f.LoadEntity<Cluster>(clusterName);

                    var ci = new ClusterInstaller(c);
                    ci.GenerateAdmin(false, adminUsername, adminEmail, adminPassword);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed.");
                    Console.WriteLine(ex.Message);

                    context.RollbackTransaction();
                }
            }

            Console.WriteLine("done.");
        }

    }
}
