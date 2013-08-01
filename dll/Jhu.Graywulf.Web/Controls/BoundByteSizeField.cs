using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [Themeable(true)]
    public class BoundByteSizeField : System.Web.UI.WebControls.BoundField
    {
        protected override object GetValue(System.Web.UI.Control controlContainer)
        {
            return Util.ByteSizeFormatter.Format((long)DataBinder.Eval(DataBinder.GetDataItem(controlContainer), DataField));
        }
    }
}
