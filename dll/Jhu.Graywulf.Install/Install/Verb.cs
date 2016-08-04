using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Install
{
    public abstract class Verb
    {
        [Parameter(Name = "Server", Description = "Name of server", Required = false)]
        public string Server { get; set; }

        [Parameter(Name = "Database", Description = "Name of database", Required = false)]
        public string Database { get; set; }

        private void UpdateConnectionString()
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

        public virtual void Run()
        {
            UpdateConnectionString();
        }
    }
}
