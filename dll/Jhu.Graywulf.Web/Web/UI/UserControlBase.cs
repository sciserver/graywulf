using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class UserControlBase : UserControl, Registry.IRegistryContextObject
    {
        private WebLoggingContext loggingContext;

        public new PageBase Page
        {
            get { return ((PageBase)base.Page); }
        }

        public string ReturnUrl
        {
            get { return Page.Request.QueryString[Constants.ReturnUrl] ?? ""; }
        }

        public Registry.RegistryContext RegistryContext
        {
            get { return Page.RegistryContext; }
        }

        Registry.RegistryContext Registry.IRegistryContextObject.RegistryContext
        {
            get { return Page.RegistryContext; }
            set { throw new InvalidOperationException(); }
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
