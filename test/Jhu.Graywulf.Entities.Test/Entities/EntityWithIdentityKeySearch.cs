using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    class EntityWithIdentityKeySearch : EntitySearch<EntityWithIdentityKey>
    {
        [DbColumn]
        public long? ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn]
        public Range<Int16> Int16 { get; set; }

        public EntityWithIdentityKeySearch()
        {
        }

        public EntityWithIdentityKeySearch(Context context)
            : base(context)
        {
        }
    }
}
