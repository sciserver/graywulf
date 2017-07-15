using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class MasterPageBase : System.Web.UI.MasterPage
    {
        private WebLoggingContext loggingContext;

        public new PageBase Page
        {
            get { return (PageBase)base.Page; }
        }

        protected virtual ScriptManager ScriptManager
        {
            get
            {
                return (ScriptManager)FindControlRecursive("theScriptManager");
            }
        }

        public Control FindControlRecursive(string id)
        {
            return Util.PageUtility.FindControlRecursive(this, id);
        }

        protected override void OnInit(EventArgs e)
        {
            loggingContext = Page.LoggingContext;

            loggingContext.Push();
            base.OnInit(e);
            loggingContext.Pop();
        }

        protected override void OnLoad(EventArgs e)
        {
            loggingContext.Push();
            base.OnLoad(e);
            loggingContext.Pop();
        }

        protected override void OnPreRender(EventArgs e)
        {
            loggingContext.Push();
            base.OnPreRender(e);
            loggingContext.Pop();
        }

        protected override void OnUnload(EventArgs e)
        {
            loggingContext.Push();
            base.OnUnload(e);
            loggingContext.Pop();

            this.loggingContext = null;
        }
    }
}
