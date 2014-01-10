using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web
{
    public partial class Basic : MasterPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.theScriptManager.Scripts.Add(new ScriptReference(Jhu.Graywulf.Web.Util.JQuery.JQueryUrl));
            this.theScriptManager.Scripts.Add(new ScriptReference("Jhu.Graywulf.Web.Controls.DockingPanel.js", "Jhu.Graywulf.Web"));

            this.Page.Title = (string)Page.Application[Constants.ApplicationLongTitle];
            this.Caption.Text = (string)Page.Application[Constants.ApplicationShortTitle];
        }
    }
}