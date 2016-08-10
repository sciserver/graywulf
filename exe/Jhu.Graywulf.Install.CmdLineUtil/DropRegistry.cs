using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "DropRegistry", Description = "Drops the database schema required for storing the cluster registry.")]
    public class DropRegistry : DropDb
    {
        protected override string OnGetConnectionString()
        {
            var csb = new SqlConnectionStringBuilder(ContextManager.Instance.ConnectionString);
            return csb.ConnectionString;
        }

        public override void Run()
        {
            var i = new RegistryInstaller(GetConnectionString());
            RunInstaller(i);
        }
    }
}
