using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "AddDomain", Description = "Creates a new domain.")]
    class AddDomain : Verb
    {
        protected string clusterName;
        protected string domainName;

        [Parameter(Name = "ClusterName", Description = "Name of the cluster", Required = true)]
        public string ClusterName
        {
            get { return clusterName; }
            set { clusterName = value; }
        }

        [Parameter(Name = "DomainName", Description = "Name of the domain", Required = true)]
        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        public AddDomain()
        {
        }

        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString;
        }

        public override void Run()
        {
            ContextManager.Instance.ConnectionString = GetConnectionString();

            if (!Quiet)
            {
                Console.Write("Creating domain... ");
            }

            using (RegistryContext context = ContextManager.Instance.CreateReadWriteContext())
            {
                var f = new EntityFactory(context);
                var c = f.LoadEntity<Cluster>(clusterName);

                var d = new Domain(c);
                d.Name = domainName;
                d.Save();
            }

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }

    }
}
