using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;


namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "DropLog", Description = "Drops the database schema required for logging.")]
    public class DropLog : DropDb
    {
        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Logging.SqlLogWriter.Configuration.ConnectionString;
        }

        public override void Run()
        {
            var i = new LogInstaller(GetConnectionString());
            RunInstaller(i);
        }
    }
}
