using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class DomainInstaller : ContextObject
    {
        private Domain domain;

        public DomainInstaller(Domain domain)
            : base(domain.Context)
        {
            this.domain = domain;
        }

        public void GenerateDefaultSettings()
        {
            domain.AuthenticatorFactory = typeof(Jhu.Graywulf.Web.Security.AuthenticatorFactory).AssemblyQualifiedName;
        }

        public void GenerateDefaultChildren()
        {
            // Create standard user group
            UserGroup ug = new UserGroup(domain);
            ug.Name = Constants.StandardUserGroupName;
            ug.Save();

            domain.StandardUserGroup = ug;
            domain.Save();
        }
    }
}
