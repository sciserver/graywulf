using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Check
{
    public class IdentityCheck : CheckRoutineBase
    {
        public override void Execute(PageBase page)
        {
            page.Response.Output.WriteLine("Testing user identity");
            page.Response.Output.WriteLine(
                "Identity is of type {0} and is {1} authenticated.",
                page.User.Identity.GetType().Name,
                page.Request.IsAuthenticated ? "" : " not");
            if (page.Request.IsAuthenticated)
            {
                page.Response.Output.WriteLine("Username: {0}, Identifier: {1}",
                    page.User.Identity.Name,
                    ((GraywulfIdentity)page.User.Identity).Identifier);
            }
        }
    }
}
