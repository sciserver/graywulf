using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.ToolbarButton.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class ToolbarButton : WebControl, IButtonControl, IPostBackEventHandler, IScriptControl
    {
        private static readonly object EventClick;
        private static readonly object EventCommand;

        static ToolbarButton()
        {
            EventClick = new object();
            EventCommand = new object();
        }

        #region Properties

        [Themeable(false), DefaultValue(true)]
        public bool CausesValidation
        {
            get { return (bool)(ViewState["CausesValidation"] ?? true); }
            set { ViewState["CausesValidation"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(false)]
        public string CommandArgument
        {
            get { return (string)(ViewState["CommandArgument"] ?? String.Empty); }
            set { ViewState["CommandArgument"] = value; }
        }

        [DefaultValue(""), Themeable(false)]
        public string CommandName
        {
            get { return (string)(ViewState["CommandName"] ?? String.Empty); }
            set { ViewState["CommandName"] = value; }
        }

        public string ClientOnClick
        {
            get { return (string)(ViewState["ClientOnClick"] ?? String.Empty); }
            set { ViewState["ClientOnClick"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassDisabled
        {
            get { return (string)(ViewState["CssClassDisabled"] ?? String.Empty); }
            set { ViewState["CssClassDisabled"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassHover
        {
            get { return (string)(ViewState["CssClassHover"] ?? String.Empty); }
            set { ViewState["CssClassHover"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(false)]
        public string NavigateUrl
        {
            get { return (string)(ViewState["NavigateUrl"] ?? String.Empty); }
            set { ViewState["NavigateUrl"] = value; }
        }

        public string NavigateUrl_ForAjax
        {
            get
            {
                if (!String.IsNullOrEmpty(NavigateUrl) && VirtualPathUtility.IsAppRelative(NavigateUrl))
                {
                    return VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, NavigateUrl);
                }
                else
                {
                    return NavigateUrl;
                }
            }
        }

        [Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=4.0.0.0, Culture=neutral", typeof(UITypeEditor)), DefaultValue(""), Themeable(false), UrlProperty("*.aspx")]
        public string PostBackUrl
        {
            get { return (string)(ViewState["PostBackUrl"] ?? String.Empty); }
            set { ViewState["PostBackUrl"] = value; }
        }

        [Bindable(true), DefaultValue(""), Localizable(true)]
        public string Text
        {
            get { return (string)(ViewState["Text"] ?? String.Empty); }
            set { ViewState["Text"] = value; }
        }

        public string ValidationGroup
        {
            get { return (string)(ViewState["ValidationGroup"] ?? String.Empty); }
            set { ViewState["ValidationGroup"] = value; }
        }

        #endregion
        #region Events

        public event EventHandler Click
        {
            add { base.Events.AddHandler(EventClick, value); }
            remove { base.Events.RemoveHandler(EventClick, value); }
        }

        public event CommandEventHandler Command
        {
            add { base.Events.AddHandler(EventCommand, value); }
            remove { base.Events.RemoveHandler(EventCommand, value); }
        }

        #endregion

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                var scriptManager = ScriptManager.GetCurrent(this.Page);
                if (scriptManager != null)
                {
                    Scripts.Script.Register(scriptManager, new Scripts.JQuery());
                    scriptManager.RegisterScriptControl(this);
                }
                else
                {
                    throw new ApplicationException("You must have a ScriptManager on the Page.");
                }
            }

            base.OnPreRender(e);
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("Graywulf.ToolbarButton", this.UniqueID);
            descriptor.AddProperty("ClientOnClick", this.ClientOnClick);
            descriptor.AddProperty("CssClass", this.CssClass);
            descriptor.AddProperty("CssClassDisabled", this.CssClassDisabled);
            descriptor.AddProperty("CssClassHover", this.CssClassHover);
            descriptor.AddProperty("NavigateUrl", this.NavigateUrl_ForAjax);
            descriptor.AddProperty("Enabled", this.Enabled);

            yield return descriptor;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.ToolbarButton.js", "Jhu.Graywulf.Web.Controls");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }

            writer.AddAttribute("id", UniqueID);

            // render style attribute
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag("td");

            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag("div");

            writer.WriteEncodedText(Text);

            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        protected virtual void OnClick(EventArgs e)
        {
            var handler = (EventHandler)base.Events[EventClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnCommand(CommandEventArgs e)
        {
            var handler = (CommandEventHandler)base.Events[EventCommand];
            if (handler != null)
            {
                handler(this, e);
            }
            base.RaiseBubbleEvent(this, e);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            OnClick(new EventArgs());

            if (!String.IsNullOrEmpty(CommandName))
            {
                OnCommand(new CommandEventArgs(CommandName, CommandArgument));
            }
        }
    }
}
