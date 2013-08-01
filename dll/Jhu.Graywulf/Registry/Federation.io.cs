using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class Federation : Entity
    {
        #region Database IO Functions

        public void LoadDatabaseDefinitions(bool forceReload)
        {
            LoadChildren<DatabaseDefinition>(forceReload);
        }

        public void LoadRemoteDatabases(bool forceReload)
        {
            LoadChildren<RemoteDatabase>(forceReload);
        }

        public void LoadQueueDefinitions(bool forceReload)
        {
            LoadChildren<QueueDefinition>(forceReload);
        }

        public void LoadJobDefinitions(bool forceReload)
        {
            LoadChildren<JobDefinition>(forceReload);
        }

        #endregion
    }
}
