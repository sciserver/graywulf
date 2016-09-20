using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements extension methods for the <b>DatabaseDefinition</b> class
    /// to provide SMO based deployment of databases.
    /// </summary>
    public partial class DatabaseDefinition
    {
        /// <summary>
        /// Returns the SMO object pointing to the template database belonging to the database definition.
        /// </summary>
        /// <param name="databaseDefinition"></param>
        /// <returns></returns>
        public Database GetSchemaSmoDatabase()
        {
            var di = GetSchemaDatabaseInstance();
            return di.GetSmoDatabase();          
        }
    }
}
