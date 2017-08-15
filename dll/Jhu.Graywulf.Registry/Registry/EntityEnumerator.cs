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
    class EntityCommandEnumerator<T> : ContextObject, IEnumerable<T>, IEnumerator<T>, IDisposable
        where T : Entity, new()
    {
        protected bool writeCache;
        protected SqlCommand command;
        protected SqlDataReader reader;
        private T current;

        public T Current
        {
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return current; }
        }

        public EntityCommandEnumerator(RegistryContext context, SqlCommand command, bool writeCache)
            :base(context)
        {
            this.writeCache = writeCache;
            this.command = command;
            this.reader = command.ExecuteReader();
            this.current = null;
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
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            var res = reader.Read();

            if (res)
            {
                var guid = reader.GetGuid(0);
                Entity entity;

                if (RegistryContext.EntityCache.TryGet(guid, out entity) &&
                    entity is T)
                {
                    current = (T)entity;
                }
                else
                {
                    entity = new T();
                    entity.RegistryContext = RegistryContext;
                    entity.LoadFromDataReader(reader);

                    if (writeCache)
                    {
                        RegistryContext.EntityCache.Add(entity);
                    }

                    current = (T)entity;
                }
            }
            else
            {
                current = null;
                Dispose();
            }

            return res;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
