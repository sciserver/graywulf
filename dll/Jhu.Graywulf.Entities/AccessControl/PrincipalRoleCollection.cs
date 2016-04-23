using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.AccessControl
{
    public class PrincipalRoleCollection : List<PrincipalRole>
    {
        public PrincipalRoleCollection()
        {
        }

        public PrincipalRoleCollection(PrincipalRoleCollection old)
            : base(old.Select(r => new PrincipalRole(r)))
        {
        }

        public void Add(string group, string role)
        {
            Add(new PrincipalRole(group, role));
        }
    }
}
