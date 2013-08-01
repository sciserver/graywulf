using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateAdmin", Description = "Creates the admin usergroup and the admin user account for an existing cluster.")]
    class CreateAdmin : CreateCluster
    {
        public override void Run()
        {
            Console.Write("Creating admin user group and user... ");

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var c = f.LoadEntity<Cluster>(clusterName);

                var u = new User(c)
                {
                    Name = adminUsername,
                    Email = adminEmail,
                    DeploymentState = Registry.DeploymentState.Deployed,
                };
                u.SetPassword(adminPassword);
                u.Save();

                // TODO: create admin group membership
            }

            Console.WriteLine("done.");
        }

    }
}
