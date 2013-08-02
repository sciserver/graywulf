using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [ParseChildren(true)]
    public class TabHeader : WebControl, INamingContainer
    {
        #region Static declarations
        private static readonly object EventTabClick;

        static TabHeader()
        {
            EventTabClick = new object();
        }

        #endregion
        #region Private member variables

        private List<Tab> tabs;

        #endregion
        #region Properties

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

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassSelected
        {
            get { return (string)(ViewState["CssClassSelected"] ?? String.Empty); }
            set { ViewState["CssClassSelected"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassLeftSpan
        {
            get { return (string)(ViewState["CssClassLeftSpan"] ?? String.Empty); }
            set { ViewState["CssClassLeftSpan"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassRightSpan
        {
            get { return (string)(ViewState["CssClassRightSpan"] ?? String.Empty); }
            set { ViewState["CssClassRightSpan"] = value; }
        }

        public List<Tab> Tabs
        {
            get { return tabs; }
        }

        public int SelectedTabIndex
        {
            get
            {
                return (int)(ViewState["SelectedIndex"] ?? 0);
            }
            set
            {
                ViewState["SelectedIndex"] = value;
            }
        }

        public Tab SelectedTab
        {
            get
            {
                return tabs[SelectedTabIndex];
            }
            set
            {
                SelectedTabIndex = tabs.IndexOf(value);
            }
        }

        #endregion
        #region Events

        public event EventHandler Click
        {
            add { base.Events.AddHandler(EventTabClick, value); }
            remove { base.Events.RemoveHandler(EventTabClick, value); }
        }

        #endregion

        public TabHeader()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.tabs = new List<Tab>();
        }

        private void InitializeTabs()
        {
            Controls.Clear();

            for (int i = 0; i < tabs.Count; i ++)
            {
                tabs[i].Enabled = i != SelectedTabIndex;
                tabs[i].Click += new EventHandler(OnTabClick);
                Controls.Add(tabs[i]);
            }
        }

        protected virtual void OnTabClick(object sender, EventArgs e)
        {
            SelectedTabIndex = tabs.IndexOf((Tab)sender);

            var handler = (EventHandler)base.Events[EventTabClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeTabs();

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            InitializeTabs();

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            ControlStyle.AddAttributesToRender(writer);

            writer.AddAttribute("id", ClientID);
            writer.AddAttribute("class", CssClass);
            writer.RenderBeginTag("table");

            writer.RenderBeginTag("tr");

            writer.AddAttribute("class", CssClassLeftSpan);
            writer.RenderBeginTag("td");
            writer.RenderEndTag();

            foreach (var c in Controls.Cast<Control>())
            {
                c.RenderControl(writer);
            }

            writer.AddAttribute("class", CssClassRightSpan);
            writer.RenderBeginTag("td");
            writer.RenderEndTag();

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}