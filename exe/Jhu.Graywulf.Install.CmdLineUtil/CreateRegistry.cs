using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "CreateRegistry", Description = "Creates the database schema required for storing the cluster registry.")]
    public class CreateRegistry : Verb
    {
        protected void UpdateConnectionString()
        {
            var csb = new SqlConnectionStringBuilder();

            // TODO: add more connection properties (u/n, pass)
            // create variables for properties.

            if (Server != null || Database != null)
            {
                csb.DataSource = Server;
                csb.InitialCatalog = Database;
                csb.IntegratedSecurity = true;
                csb.MultipleActiveResultSets = true;

                ContextManager.Instance.ConnectionString = csb.ConnectionString;
            }
        }

        public override void Run()
        {
            UpdateConnectionString();

            Console.Write("Creating database... ");

            var i = new RegistryInstaller(ContextManager.Instance.ConnectionString);
            i.CreateDatabase();
            i.CreateSchema();

            Console.WriteLine("done.");
        }
    }
}
