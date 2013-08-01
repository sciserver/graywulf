using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Metadata.CmdLineUtil
{
    abstract class DatabaseVerbBase : Verb
    {
        private string server;
        private string database;
        private string userId;
        private string password;
        private bool integratedSecurity;

        [Parameter(Name = "Server", Description = "Database server", Required = false)]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        [Parameter(Name = "Database", Description = "Database", Required = true)]
        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        [Parameter(Name = "UserId", Description = "User ID", Required = false)]
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [Parameter(Name = "Password", Description = "Password", Required = false)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Option(Name = "EnableIntegratedSecurity", Description="Enable integrated security")]
        public bool IntegratedSecurity
        {
            get { return integratedSecurity; }
            set { integratedSecurity = value; }
        }

        public DatabaseVerbBase()
        {
            IntializeMembers();
        }

        private void IntializeMembers()
        {
            this.server = "localhost";
            this.database = null;
            this.userId = null;
            this.password = null;
            this.integratedSecurity = true;
        }

        protected SqlConnectionStringBuilder GetConnectionString()
        {
            var csb = new SqlConnectionStringBuilder();
            
            csb.DataSource = server;
            csb.InitialCatalog = database;

            if (!integratedSecurity)
            {
                csb.UserID = userId;
                csb.Password = password;
            }
            else
            {
                csb.IntegratedSecurity = true;
            }

            return csb;
        }
    }
}
