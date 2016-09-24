using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class MasterPageBase : System.Web.UI.MasterPage
    {
        protected virtual ScriptManager ScriptManager
        {
            get
            {
                return (ScriptManager)FindControlRecursive("theScriptManager");
            }
        }

        public Control FindControlRecursive(string id)
        {
            return Util.PageUtility.FindControlRecursive(this, id);
        }
    }
}
