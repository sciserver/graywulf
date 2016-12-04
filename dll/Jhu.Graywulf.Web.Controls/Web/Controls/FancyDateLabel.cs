using System;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class FancyDateLabel : Label
    {
        public DateTime? Value
        {
            get { return (DateTime?)ViewState["Value"]; }
            set
            {
                ViewState["Value"] = value;
                Text = Jhu.Graywulf.Util.DateFormatter.FancyFormat(value);
            }
        }
    }
}
