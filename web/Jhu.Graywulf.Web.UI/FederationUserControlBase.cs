using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.UI
{
    public class FederationUserControlBase : Jhu.Graywulf.Web.UI.UserControlBase
    {
        public new FederationPageBase Page
        {
            get { return (FederationPageBase)base.Page; }
        }

        public new RegistryContext RegistryContext
        {
            get { return ((FederationPageBase)base.Page).RegistryContext; }
    }

        public FederationContext FederationContext
        {
            get { return ((FederationPageBase)base.Page).FederationContext; }
        }
    }
}