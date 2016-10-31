using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;


namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "CreateJobPersistence", Description = "Creates the database schema required for job persistence.")]
    public class CreateJobPersistence : CreateDb
    {
        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Scheduler.Scheduler.Configuration.PersistenceConnectionString;
        }

        public override void Run()
        {
            var i = new JobPersistenceInstaller(GetConnectionString());
            RunInstaller(i);
        }
    }
}
