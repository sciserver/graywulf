using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity
    {
        public void Discover(List<Entity> update, List<Entity> delete, List<Entity> create, List<Entity> error, bool supressError)
        {
            try
            {
                OnDiscover(update, delete, create);
            }
            catch (Exception ex)
            {
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
