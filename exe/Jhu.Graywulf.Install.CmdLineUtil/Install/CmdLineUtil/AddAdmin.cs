using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "AddAdmin", Description = "Creates the admin usergroup and the admin user account for an existing cluster.")]
    class AddAdmin : AddCluster
    {
        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString;
        }

        public override void Run()
        {
            ContextManager.Instance.ConnectionString = GetConnectionString();

            Console.Write("Creating admin user... ");

            using (RegistryContext context = ContextManager.Instance.CreateReadWriteContext())
            {
                var f = new EntityFactory(context);
                var c = f.LoadEntity<Cluster>(clusterName);

                var ci = new ClusterInstaller(c);
                ci.GenerateAdmin(false, adminUsername, adminEmail, adminPassword);
            }

            Console.WriteLine("done.");
        }

    }
}
