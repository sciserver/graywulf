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
    public class CreateLog : Verb
    {
        public override void Run()
        {
            Console.Write("Creating database schema... ");

            var i = new LogInstaller(GetConnectionString());
            i.CreateDatabase();
            i.CreateSchema();

            Console.WriteLine("done.");
        }
    }
}
