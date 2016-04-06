using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public class IdentityRoleCollection : List<IdentityRole>
    {
        public IdentityRoleCollection()
        {
        }

        public IdentityRoleCollection(IdentityRoleCollection old)
            : base(old.Select(r => new IdentityRole(r)))
        {
        }

        public void Add(string group, string role)
        {
            Add(new IdentityRole(group, role));
        }
    }
}
