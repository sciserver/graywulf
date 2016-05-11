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


            // temporarily commented out scripts here, no need to build project every time something changes in them
            // It was also needed to comment out one line in ToolbarButton.cs (Jhu.Graywulf.Web.Controls/Web/Controls)

            //this.theScriptManager.Scripts.Add(new ScriptReference(Util.JQuery.JQueryUrl));
            //this.theScriptManager.Scripts.Add(new ScriptReference(Util.Bootstrap.BootstrapUrl));

            //this.theScriptManager.Scripts.Add(new ScriptReference("Jhu.Footprint.Web.UI.Scripts.Editor.js", "Jhu.Footprint.Web.UI"));
            //this.theScriptManager.Scripts.Add(new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web.Controls"));

            this.Page.Title = (string)Page.Application[Constants.ApplicationLongTitle];
            this.Caption.Text = (string)Page.Application[Constants.ApplicationDomainName] + " :: " + (string)Page.Application[Constants.ApplicationShortTitle];
        }
    }
}