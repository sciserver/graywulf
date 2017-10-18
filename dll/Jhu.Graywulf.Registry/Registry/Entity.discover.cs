using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity
    {
        public void Discover()
        {
            // This is used by PS deploy scripts

            var update = new List<Entity>();
            var delete = new List<Entity>();
            var create = new List<Entity>();
            var error = new List<Entity>();

            Discover(update, delete, create, error, false);

            var ef = new EntityFactory(RegistryContext);
            ef.ApplyChanges(update, delete, create);
        }

        public void Discover(List<Entity> update, List<Entity> delete, List<Entity> create, List<Entity> error, bool supressError)
        {
            try
            {
                OnDiscover(update, delete, create);
            }
            catch (Exception ex)
            {
#if BREAKDEBUG
                if (global::System.Diagnostics.Debugger.IsAttached)
                {
                    global::System.Diagnostics.Debugger.Break();
                }
#endif

                if (!supressError)
                {
                    throw;
                }
                else
                {
                    error.Add(this);
                }
            }
        }

        /// <summary>
        /// Updates the entity to reflect actual system configuration
        /// </summary>
        /// <returns></returns>
        protected virtual void OnDiscover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            throw new NotImplementedException();
        }
    }
}
