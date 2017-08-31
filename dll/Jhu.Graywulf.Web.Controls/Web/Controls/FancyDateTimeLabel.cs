using System;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class FancyDateTimeLabel : Label
    {
        public DateTime? Value
        {
            get { return (DateTime?)ViewState["Value"]; }
            set
            {
                ViewState["Value"] = value;
                Text = Jhu.Graywulf.Util.DateTimeFormatter.FancyFormat(value);
            }
        }
    }
}
