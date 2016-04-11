using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    class EntityWithIdentityKeySearch : EntitySearch<EntityWithIdentityKey>
    {
        private long? id;
        private string name;

        [DbColumn]
        public long? ID
        {
            get { return id; }
            set { id = value; }
        }

        [DbColumn]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public EntityWithIdentityKeySearch()
        {
            InitializeMembers();
        }

        public EntityWithIdentityKeySearch(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.id = null;
            this.name = null;
        }
    }
}
