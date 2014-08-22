using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Check
{
    public class IdentityProviderCheck : CheckRoutineBase
    {
        public override void Execute(PageBase page)
        {
            page.Response.Output.WriteLine("Testing identity provider");
            page.Response.Output.WriteLine(
                "Indentity provider is {0}",
                IdentityProvider.Create(page.RegistryContext.Domain).GetType().Name);
        }
    }
}
