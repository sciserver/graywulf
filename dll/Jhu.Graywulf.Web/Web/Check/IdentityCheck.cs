using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.Check
{
    public class IdentityCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Iis; }
        }

        public override void Execute(TextWriter output)
        {
            output.WriteLine("Testing user identity");
            output.WriteLine(
                "Identity is of type {0} and is {1} authenticated.",
                HttpContext.Current.User.Identity.GetType().Name,
                HttpContext.Current.Request.IsAuthenticated ? "" : " not");
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                output.WriteLine("Username: {0}, Identifier: {1}",
                    HttpContext.Current.User.Identity.Name,
                    ((GraywulfIdentity)HttpContext.Current.User.Identity).Identifier);
            }
        }
    }
}
