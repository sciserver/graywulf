using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    class IdentityKeyEntitySearch : EntitySearch<IdentityKeyEntity>
    {
        [DbColumn]
        public long? ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn]
        public Range<Int16> Int16 { get; set; }

        public IdentityKeyEntitySearch()
        {
        }

        public IdentityKeyEntitySearch(Context context)
            : base(context)
        {
        }
    }
}
