using System;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class FancyTimeSpanLabel : Label
    {
        public TimeSpan? Value
        {
            get { return (TimeSpan?)ViewState["Value"]; }
            set
            {
                ViewState["Value"] = value;
                Text = Jhu.Graywulf.Util.TimeSpanFormatter.FancyFormat(value);
            }
        }
    }
}
