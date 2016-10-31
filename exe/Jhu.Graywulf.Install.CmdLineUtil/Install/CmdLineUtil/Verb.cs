using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    public abstract class Verb
    {
        [Parameter(Name = "Server", Description = "Name of server", Required = false)]
        public string Server { get; set; }

        [Parameter(Name = "Database", Description = "Name of database", Required = false)]
        public string Database { get; set; }

        [Option(Name = "Quiet", Description = "Quiet")]
        public bool Quiet { get; set; }

        protected string GetConnectionString()
        {
            var csb = new SqlConnectionStringBuilder(OnGetConnectionString());

            if (!String.IsNullOrWhiteSpace(Server))
            {
                csb.DataSource = Server;
            }

            if (!String.IsNullOrWhiteSpace(Database))
            {
                csb.InitialCatalog = Database;
            }

            if (String.IsNullOrWhiteSpace(csb.UserID))
            {
                csb.IntegratedSecurity = true;
            }

            // TODO: add more connection properties (u/n, pass)
            // create variables for properties.
            csb.MultipleActiveResultSets = true;

            return csb.ConnectionString;
        }

        protected virtual string OnGetConnectionString()
        {
            return "";
        }
        
        public abstract void Run();
    }
}
