using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    public abstract class CreateDb : Verb
    {
        [Option(Name = "DbOnly", Description = "Database only, without schema")]
        public bool DbOnly { get; set; }

        [Option(Name = "SchemaOnly", Description = "Schema only, use existing database")]
        public bool SchemaOnly { get; set; }

        [Parameter(Name = "Username", Description = "Name of the service account", Required = false)]
        public string Username { get; set; }

        [Parameter(Name = "Role", Description = "Role to add user to", Required = false)]
        public string Role { get; set; }

        protected void RunInstaller(DBInstaller i)
        {
            if (!Quiet)
            {
                Console.Write("Creating database... ");
            }

            if (!SchemaOnly)
            {
                i.CreateDatabase();
            }

            if (!DbOnly)
            {
                i.CreateSchema();
            }

            if (!String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Role))
            {
                i.AddUser(Username, Role);
            }

            if (!Quiet)
            {
                Console.WriteLine("done.");
            }
        }
    }
}
