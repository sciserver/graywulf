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

        protected string GetConnectionString()
        {
            var csb = new SqlConnectionStringBuilder()
            {
                DataSource = Server,
                InitialCatalog = Database
            };

            return csb.ConnectionString;
        }

        public abstract void Run();
    }
}
