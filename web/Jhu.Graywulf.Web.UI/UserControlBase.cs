using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jhu.Graywulf.Web.UI
{
    public class UserControlBase : Jhu.Graywulf.Web.UserControlBase
    {
        public new PageBase Page
        {
            get { return (PageBase)base.Page; }
        }
    }
}