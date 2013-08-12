using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class ToolbarSpan : WebControl
    {
        protected override void Render(HtmlTextWriter writer)
        {
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag("td");
            writer.RenderEndTag();
        }
    }
}
