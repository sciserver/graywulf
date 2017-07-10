using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements extension methods for the <b>ServerInstance</b> class
    /// to provide SMO based deployment of databases.
    /// </summary>
    public partial class ServerInstance
    {
        /// <summary>
        /// Returns an SMO object pointing to the SQL Server instance
        /// </summary>
        /// <param name="serverInstance">The server instance object.</param>
        /// <returns>The SMO object pointing to the SQL Server instance.</returns>
        public Server GetSmoServer()
        {
            // Create connection and add credentials if sql server login method is specified for the server instance
            var csb = new SqlConnectionStringBuilder();

            csb.DataSource = this.GetCompositeName();
            csb.Enlist = false;
            csb.IntegratedSecurity = this.integratedSecurity;
            csb.ConnectTimeout = 5;

            if (!integratedSecurity)
            {
                csb.UserID = this.adminUser;
                csb.Password = this.adminPassword;
            }

            var cn = new SqlConnection(csb.ConnectionString);
            cn.Open();

            var scn = new ServerConnection(cn);
            scn.AutoDisconnectMode = Microsoft.SqlServer.Management.Common.AutoDisconnectMode.DisconnectIfPooled;

            Server srv = new Server(scn);
            srv.ConnectionContext.StatementTimeout = 3600; //1 hour timeout

            return srv;
        }

        #region Linked Server Functions

        /// <summary>
        /// Creates a linked server to a remote SQL Server instance.
        /// </summary>
        /// <param name="serverInstance">The local SQL Server instance.</param>
        /// <param name="remoteInstance">The remote SQL Server instance.</param>
        public void CreateLinkedServer(Jhu.Graywulf.Registry.ServerInstance remoteInstance)
        {
            CreateLinkedServer(remoteInstance, remoteInstance.GetCompositeName(), null, null);
        }

        /// <summary>
        /// Creates a linked server to a remote SQL Server instance with customizable credentials.
        /// </summary>
        /// <param name="serverInstance">The local SQL Server instance.</param>
        /// <param name="remoteInstance">The remote SQL Server instance.</param>
        /// <param name="name">Name of the linked server.</param>
        /// <param name="userName">Username to use for authenticating the connection.</param>
        /// <param name="password">Password to use for authenticating the connection.</param>
        public void CreateLinkedServer(Jhu.Graywulf.Registry.ServerInstance remoteInstance, string name, string userName, string password)
        {
            if (this.Guid == remoteInstance.Guid)
            {
                throw new DeployException(ExceptionMessages.LinkedServerCannotPointToItself);
            }

            Server s = this.GetSmoServer();

            LinkedServer ls = new LinkedServer(s, name);

            // TODO
            ls.Catalog = string.Empty;
            ls.CollationCompatible = false;         // ???
            ls.CollationName = string.Empty;
            ls.ConnectTimeout = 0;                  // ???
            ls.DataAccess = true;
            ls.DataSource = string.Empty;
            ls.DistPublisher = false;
            ls.Distributor = false;
            ls.LazySchemaValidation = true;
            //ls.LinkedServerLogins
            ls.Location = string.Empty; //?
            ls.ProductName = "SQL Server";
            ls.ProviderName = string.Empty;         // OLE DB only
            ls.ProviderString = string.Empty;       // OLE DB only
            ls.Publisher = false;
            ls.QueryTimeout = 0;                    // ???
            ls.Rpc = true;
            ls.RpcOut = true;
            ls.Subscriber = false;
            ls.UseRemoteCollation = true;

            s.LinkedServers.Add(ls);

            ls.Create();

            if (userName != null && password != null)
            {
                // Drop default login (impersonate)
                ls.LinkedServerLogins[0].Drop();

                // Create login for sql authentication
                LinkedServerLogin login = new LinkedServerLogin(ls, string.Empty);
                login.Impersonate = false;
                login.RemoteUser = userName;
                login.SetRemotePassword(password);

                ls.LinkedServerLogins.Add(login);

                login.Create();
            }

            this.RegistryContext.LogEvent(new Event("Jhu.Graywulf.Registry.ServerInstance.CreateLinkedServer", this.Guid));
        }

        /// <summary>
        /// Drops a linked server connection.
        /// </summary>
        /// <param name="serverInstance">The local server instance.</param>
        /// <param name="remoteInstance">The remote server instance.</param>
        public void DropLinkedServer(Jhu.Graywulf.Registry.ServerInstance remoteInstance)
        {
            Server s = this.GetSmoServer();
            s.LinkedServers[remoteInstance.GetCompositeName()].Drop();

            this.RegistryContext.LogEvent(new Event("Jhu.Graywulf.Registry.ServerInstance.DropLinkedServer", this.Guid));
        }

        /// <summary>
        /// Drops a linked server connection.
        /// </summary>
        /// <param name="serverInstance">The local server instance.</param>
        /// <param name="name">Name of the linked server (not the server name but the SQL object name).</param>
        public void DropLinkedServer(string name)
        {
            Server s = this.GetSmoServer();
            s.LinkedServers[name].Drop(true);

            this.RegistryContext.LogEvent(new Event("Jhu.Graywulf.Registry.ServerInstance.CreateLinkedServer", this.Guid));
        }

        /// <summary>
        /// Checks if the liked server connection between the local and a remote server exists.
        /// </summary>
        /// <param name="serverInstance">The local SQL Server instance.</param>
        /// <param name="remoteInstance">The remote SQL Server instance.</param>
        /// <returns></returns>
        public bool CheckLinkedServerExist(Jhu.Graywulf.Registry.ServerInstance remoteInstance)
        {
            if (this.Guid == remoteInstance.Guid)
            {
                throw new DeployException(ExceptionMessages.LinkedServerCannotPointToItself);
            }

            Server s = this.GetSmoServer();
            return s.LinkedServers.Contains(remoteInstance.GetCompositeName());
        }

        #endregion
    }
}
