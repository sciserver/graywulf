using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Jhu.Graywulf.Util
{
    public static class PageUtility
    {
        public static Control FindControlRecursive(Control root, string id)
        {
            Control f = root.FindControl(id);
            if (f == null)
            {
                foreach (Control c in root.Controls)
                {
                    Control x = FindControlRecursive(c, id);
                    if (x != null) return x;
                }
                return null;
            }
            else
            {
                return f;
            }
        }
    }
}
