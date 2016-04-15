using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;
using Jhu.Graywulf.Entities.AccessControl;

namespace Jhu.Graywulf.Entities
{
    [DbTable]
    public class SecuredEntity : SecurableEntity
    {
        [DbColumn(Binding = DbColumnBinding.Key)]
        public int ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        public SecuredEntity()
        {
        }

        public SecuredEntity(Context context)
            : base(context)
        {
        }
    }
}
