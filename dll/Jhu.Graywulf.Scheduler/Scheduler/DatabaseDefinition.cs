using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    internal class DatabaseDefinition : RegistryObject
    {
        private Dictionary<string, Dictionary<Guid, DatabaseInstance>> databaseInstances;

        public Dictionary<string, Dictionary<Guid, DatabaseInstance>> DatabaseInstances
        {
            get { return databaseInstances; }
        }

        public DatabaseDefinition(Jhu.Graywulf.Registry.DatabaseDefinition e)
            : base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.databaseInstances = new Dictionary<string, Dictionary<Guid, DatabaseInstance>>();
        }
    }
}
