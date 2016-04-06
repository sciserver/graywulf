using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [DbTable]
    public class TestEntity : Entity
    {
        private long id;

        [DbColumn(Binding = DbColumnBinding.Key, DefaultValue = -1)]
        public long ID
        {
            get { return id; }
            set { id = value; }
        }

        public TestEntity()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.id = -1;
        }
    }
}
