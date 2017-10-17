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
        protected bool createDefaultController;
        protected bool createDefaultNode;

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

        [Option(Name = "CreateDefaultController", Description = "Generate default controller")]
        public bool CreateDefaultController
        {
            get { return createDefaultController; }
            set { createDefaultController = value; }
        }

        [Option(Name = "CreateDefaultNode", Description = "Generate default server node")]
        public bool CreateDefaultNode
        {
            get { return createDefaultNode; }
            set { createDefaultNode = value; }
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
            this.createDefaultController = false;
            this.createDefaultNode = false;
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
            
            using (RegistryContext context = ContextManager.Instance.CreateReadWriteContext())
            {
                var i = new ClusterInstaller(context)
                {
                    ClusterName = clusterName,
                    AdminUserName = adminUsername,
                    AdminEmail = adminEmail,
                    AdminPassword = adminPassword,
                    CreateDefaultController = createDefaultController,
                    CreateDefaultNode = createDefaultNode,
                };
                i.Install();
            }

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }

    }
}
