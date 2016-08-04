using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;


namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateJobPersistence", Description = "Creates the database schema required for logging.")]
    public class CreateJobPersistence : Verb
    {


        public override void Run()
        {
            var csb = new SqlConnectionStringBuilder()
            {
                DataSource = Server,
                InitialCatalog = Database
            };

            Console.Write("Creating database schema... ");

            var i = new JobPersistenceInstaller(csb.ConnectionString);
            i.CreateDatabase();
            i.CreateSchema();

            Console.WriteLine("done.");
        }
    }
}
