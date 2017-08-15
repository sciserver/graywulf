using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements a per-context cache to avoid reloading entities
    /// from the database every single time.
    /// </summary>
    class EntityCache : IDisposable
    {
        private Dictionary<Guid, Entity> byGuid;
        private Dictionary<string, Entity> byName;

        public EntityCache()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.byGuid = new Dictionary<Guid, Entity>();
            this.byName = new Dictionary<string, Entity>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Dispose()
        {
            byGuid = null;
            byName = null;
        }

        public void Add(Entity entity)
        {
            if (!byGuid.ContainsKey(entity.Guid))
            {
                byGuid.Add(entity.Guid, entity);
            }
            else
            {
            }

            if (entity.IsFullyQualifiedNameLoaded())
            {
                var fqn = entity.GetFullyQualifiedName();

                if (!byName.ContainsKey(fqn))
                {
                    byName.Add(entity.GetFullyQualifiedName(), entity);
                }
            }
        }

        public bool TryGet(Guid guid, out Entity e)
        {
            return byGuid.TryGetValue(guid, out e);
        }

        public bool TryGet(string fullyQualifiedName, out Entity e)
        {
            return byName.TryGetValue(fullyQualifiedName, out e);
        }
    }
}
