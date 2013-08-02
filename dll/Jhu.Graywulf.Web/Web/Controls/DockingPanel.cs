using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.DockingPanel.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class DockingPanel : WebControl, IScriptControl
    {

        public DockingStyle DockingStyle
        {
            get { return (DockingStyle)(ViewState["DockingStyle"] ?? DockingStyle.None); }
            set { ViewState["DockingStyle"] = value; }
        }

        public DockingPanel()
        {

        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            AddAttributesToRender(writer);
            writer.AddAttribute("class", String.Format("docking-{0}", DockingStyle.ToString().ToLowerInvariant()));
            writer.RenderBeginTag("div");

            base.Render(writer);

            writer.RenderEndTag();
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            yield break;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web");
        }
    }
}
