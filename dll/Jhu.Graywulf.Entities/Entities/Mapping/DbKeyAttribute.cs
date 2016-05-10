using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Entities.Mapping
{
    public class DbKeyAttribute : DbColumnAttribute
    {
        public DbKeyAttribute()
            : base()
        {
            this.Binding = DbColumnBinding.Key;
        }

        public DbKeyAttribute(string name)
            : base()
        {
            this.Binding = DbColumnBinding.Key;
        }
    }
}
