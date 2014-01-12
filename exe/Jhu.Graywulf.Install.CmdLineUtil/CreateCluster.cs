using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateCluster", Description = "Creates a new cluster entry and an administrator account in the cluster schema. Admin user credentials must be provided.")]
    class CreateCluster : Verb
    {
        protected string clusterName;
        protected string adminUsername;
        protected string adminEmail;
        protected string adminPassword;

        [Parameter(Name = "ClusterName", Description = "Name of the cluster", Required = true)]
        public string ClusterName
        {
            get { return clusterName; }
            set { clusterName = value; }
        }

        [Parameter(Name = "Username", Description = "Name of the new administrator", Required = false)]
        public string AdminUsername
        {
            get { return adminUsername; }
            set { adminUsername = value; }
        }

        [Parameter(Name = "Email", Description = "E-mail of the new administrator", Required = false)]
        public string AdminEmail
        {
            get { return adminEmail; }
            set { adminEmail = value; }
        }

        [Parameter(Name = "Password", Description = "Password of the new administrator", Required = false)]
        public string AdminPassword
        {
            get { return adminPassword; }
            set { adminPassword = value; }
        }

        public CreateCluster()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.clusterName = null;
            this.adminUsername = Constants.ClusterAdminName;
            this.adminEmail = Constants.ClusterAdminEmail;
            this.adminPassword = Constants.ClusterAdminPassword;
        }

        public override void Run()
        {
            base.Run();

            Console.Write("Creating cluster... ");

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit))
            {
                var i = new ClusterInstaller(context);

                i.Install(true, clusterName, adminUsername, adminEmail, adminPassword);

                context.CommitTransaction();
            }

            Console.WriteLine("done.");
        }

    }
}
