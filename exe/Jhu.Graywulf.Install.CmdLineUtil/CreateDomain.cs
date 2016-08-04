using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateDomain", Description = "Creates a new domain.")]
    class CreateDomain : Verb
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

        public CreateDomain()
        {
        }

        public override void Run()
        {
            base.Run();

            Console.Write("Creating domain... ");

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                try
                {
                    var f = new EntityFactory(context);
                    var c = f.LoadEntity<Cluster>(clusterName);

                    var d = new Domain(c);
                    d.Name = domainName;
                    d.Save();                    
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
