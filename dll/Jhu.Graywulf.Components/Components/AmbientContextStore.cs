using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Components
{
    class AmbientContextStore : Dictionary<Type, AmbientContextBase>
    {
        public Type Find(Type key)
        {
            foreach (var k in this.Keys)
            {
                if (k == key || k.IsSubclassOf(key))
                {
                    return k;
                }
            }

            return null;
        }
    }
}
