using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    class AddUser : Verb
    {
        [Parameter(Name = "Username", Description = "Name of the service account", Required = true)]
        public string Username { get; set; }

        [Parameter(Name = "Role", Description = "Role to add user to", Required = true)]
        public string Role { get; set; }

        public override void Run()
        {
            Console.Write("Creating database user... ");

            var di = new DBInstaller(GetConnectionString());
            di.AddUser(Username, Role);

            Console.WriteLine("done.");
        }
    }
}
