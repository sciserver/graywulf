using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "AddCluster", Description = "Creates a new cluster entry and an administrator account in the cluster schema. Admin user credentials must be provided.")]
    class AddCluster : Verb
    {
        protected string clusterName;
        protected string adminUsername;
        protected string adminEmail;
        protected string adminPassword;
        protected bool createNode;

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

        [Option(Name = "CreateNode", Description = "Generate first server node")]
        public bool CreateNode
        {
            get { return createNode; }
            set { createNode = value; }
        }

        public AddCluster()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.clusterName = null;
            this.adminUsername = Constants.ClusterAdminUserName;
            this.adminEmail = Constants.ClusterAdminUserEmail;
            this.adminPassword = Constants.ClusterAdminUserPassword;
            this.createNode = true;
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
                Console.Write("Creating cluster... ");
            }
            
            using (RegistryContext context = ContextManager.Instance.CreateContext(TransactionMode.ManualCommit))
            {
                var i = new ClusterInstaller(context)
                {
                    ClusterName = clusterName,
                    AdminUserName = adminUsername,
                    AdminEmail = adminEmail,
                    AdminPassword = adminPassword,
                    CreateNode = createNode,
                };
                i.Install();
                context.CommitTransaction();
            }

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }

    }
}
