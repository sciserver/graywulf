using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    public class SecuredEntitySearch : SecurableEntitySearch<SecuredEntity>
    {
        [DbColumn]
        public string Name { get; set; }

        public SecuredEntitySearch()
        {
        }

        public SecuredEntitySearch(Context context)
            : base(context)
        {
        }
    }
}
