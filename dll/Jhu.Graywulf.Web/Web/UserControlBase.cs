using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web
{
    public abstract class UserControlBase : UserControl
    {
        public string ReturnUrl
        {
            get { return Page.Request.QueryString[Constants.ReturnUrl] ?? ""; }
        }

        public Registry.Context RegistryContext
        {
            get { return ((PageBase)Page).RegistryContext; }
        }

        
    }
}
