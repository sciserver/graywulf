using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;


namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "CreateLog", Description = "Creates the database schema required for logging.")]
    public class CreateLog : CreateDb
    {
        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Logging.AppSettings.ConnectionString;
        }

        public override void Run()
        {
            var i = new LogInstaller(GetConnectionString());
            RunInstaller(i);
        }
    }
}
