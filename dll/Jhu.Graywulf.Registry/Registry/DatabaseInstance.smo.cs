using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Security.AccessControl;
using smo = Microsoft.SqlServer.Management.Smo;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Registry
{

    public partial class DatabaseInstance
    {
        /// <summary>
        /// Returns an SMO database object referencing the database instance.
        /// </summary>
        /// <param name="databaseInstance">The database instance object.</param>
        /// <returns>An SMO database object connected to the database instance.</returns>
        public smo::Database GetSmoDatabase()
        {
            smo::Server s = this.ServerInstance.GetSmoServer();
            return s.Databases[databaseName];
        }

        public void LoadFromSmo(smo::Database smodb)
        {
            this.DatabaseName = smodb.Name;
        }
    }
}
