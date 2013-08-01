using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity
    {
        /// <summary>
        /// Updates the entity to reflect actual system configuration
        /// </summary>
        /// <returns></returns>
        public virtual void Discover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            throw new NotImplementedException();
        }
    }
}
