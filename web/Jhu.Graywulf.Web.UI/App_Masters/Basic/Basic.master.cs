using System;
using System.Web.UI;
using Jhu.Graywulf.Web.UI;


namespace Jhu.Graywulf.Web.UI.Masters
{
    public partial class Basic : MasterPageBase
    {
        protected override ScriptManager ScriptManager
        {
            get
            {
                return scriptManager;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Scripts.ScriptLibrary.Register(ScriptManager, new Scripts.JQuery());
            Scripts.ScriptLibrary.Register(ScriptManager, new Scripts.JQueryValidation());
            Scripts.ScriptLibrary.Register(ScriptManager, new Scripts.Bootstrap());
            Scripts.ScriptLibrary.Register(ScriptManager, new Scripts.BootstrapLighbox());
            Scripts.ScriptLibrary.Register(ScriptManager, new Scripts.DockingPanel());

            Page.Title = (string)Page.Application[Constants.ApplicationLongTitle];
            Caption.Text = (string)Page.Application[Constants.ApplicationDomainName] + " :: " + (string)Page.Application[Constants.ApplicationShortTitle];
        }

        protected void ScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            ScriptManager.AsyncPostBackErrorMessage = e.Exception.Message;
        }
    }
}