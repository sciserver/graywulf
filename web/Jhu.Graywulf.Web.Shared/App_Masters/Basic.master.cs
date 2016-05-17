using System;
using System.Web.UI;
using Jhu.Graywulf.Web.UI;

// Maybe this should be put somewhere else
[assembly: WebResource("Jhu.Footprint.Web.UI.Scripts.Editor.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web
{
    public partial class Basic : MasterPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.theScriptManager.Scripts.Add(new ScriptReference(Util.JQuery.JQueryUrl));
            this.theScriptManager.Scripts.Add(new ScriptReference(Util.Bootstrap.BootstrapUrl));

            this.theScriptManager.Scripts.Add(new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web.Controls"));

            this.Page.Title = (string)Page.Application[Constants.ApplicationLongTitle];
            this.Caption.Text = (string)Page.Application[Constants.ApplicationDomainName] + " :: " + (string)Page.Application[Constants.ApplicationShortTitle];
        }
    }
}