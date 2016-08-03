using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "CreateDB", Description = "Creates the database schema required for storing the cluster registry.")]
    class CreateDb : Verb
    {
        public override void Run()
        {
            base.Run();

            try
            {
                Console.Write("Creating database... ");

                var i = new DBInstaller()
                {
                    ConnectionString = Jhu.Graywulf.Registry.ContextManager.Instance.ConnectionString
                };
                i.CreateDatabase();
                i.CreateSchema();

                Console.WriteLine("done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed.");

                Console.WriteLine(ex.Message);
            }
        }
    }
}
