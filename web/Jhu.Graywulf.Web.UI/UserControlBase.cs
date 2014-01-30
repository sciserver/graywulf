using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI
{
    public class UserControlBase : Jhu.Graywulf.Web.UserControlBase
    {
        public new PageBase Page
        {
            get { return (PageBase)base.Page; }
        }

        public Context RegistryContext
        {
            get { return ((PageBase)base.Page).RegistryContext; }
    }

        public FederationContext FederationContext
        {
            get { return ((PageBase)base.Page).FederationContext; }
        }
    }
}