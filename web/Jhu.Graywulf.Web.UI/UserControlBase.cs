using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.UI
{
    public class UserUserControlBase : Jhu.Graywulf.Web.UI.UserControlBase
    {
        public new UserPageBase Page
        {
            get { return (UserPageBase)base.Page; }
        }

        public Context RegistryContext
        {
            get { return ((UserPageBase)base.Page).RegistryContext; }
    }

        public FederationContext FederationContext
        {
            get { return ((UserPageBase)base.Page).FederationContext; }
        }
    }
}