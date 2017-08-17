using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace Jhu.Graywulf.Registry
{
    class EntityCommandEnumerator<T> : ContextObject, IEnumerable<T>, IDisposable
        where T : Entity, new()
    {
        private bool writeCache;
        private Dictionary<Guid, T> entities;
        private SqlCommand command;
        private SqlDataReader reader;

        public EntityCommandEnumerator(RegistryContext context, SqlCommand command, bool writeCache)
            : base(context)
        {
            this.writeCache = writeCache;
            this.entities = new Dictionary<Guid, T>();
            this.command = command;
            this.reader = command.ExecuteReader();

            while (reader.Read())
            {
                var guid = reader.GetGuid(0);
                Entity entity;

                if (!(RegistryContext.EntityCache.TryGet(guid, out entity) && entity is T))
                {
                    entity = new T();
                    entity.RegistryContext = RegistryContext;
                    entity.LoadFromDataReader(reader);

                    if (writeCache)
                    {
                        RegistryContext.EntityCache.Add(entity);
                    }
                }

                entity.InitializeEntityReferences();
                entities.Add(entity.Guid, (T)entity);
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var guid = reader.GetGuid(0);
                    var type = reader.GetByte(1);
                    var referencedguid = reader.GetGuid(2);

                    var entity = entities[guid];

                    if (entity.entityReferences.ContainsKey(type))
                    {
                        entity.entityReferences[type].Guid = referencedguid;
                    }
                }

                foreach (var entity in entities.Values)
                {
                    entity.isEntityReferencesLoaded = true;
                }
            }

            Dispose();
        }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Dispose();
            }

            if (command != null)
            {
                command.Dispose();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            //return this;
            return entities.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // return this;
            return entities.Values.GetEnumerator();
        }
    }
}
