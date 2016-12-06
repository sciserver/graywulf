using System;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class FancyByteSizeLabel : Label
    {
        public long? Value
        {
            get { return (long?)ViewState["Value"]; }
            set
            {
                ViewState["Value"] = value;
                Text = Jhu.Graywulf.Util.ByteSizeFormatter.Format(value);
            }
        }
    }
}
