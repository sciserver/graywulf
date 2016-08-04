using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    class AddUser : Verb
    {
        private string username;

        [Parameter(Name = "Username", Description = "Name of the service account", Required = false)]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

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
                i.AddUser(Username);
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
