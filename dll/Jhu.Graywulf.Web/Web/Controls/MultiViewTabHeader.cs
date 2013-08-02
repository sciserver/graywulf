using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [ParseChildren(false)]
    public class MultiViewTabHeader : TabHeader
    {
        #region Properties

        [DefaultValue(""), Bindable(false), Themeable(false)]
        public string MultiViewID
        {
            get { return (string)(ViewState["MultiViewID"] ?? String.Empty); }
            set { ViewState["MultiViewID"] = value; }
        }

        protected MultiView MultiView
        {
            get { return (MultiView)Parent.FindControl(MultiViewID); }
        }

        #endregion

        protected override void OnTabClick(object sender, EventArgs e)
        {
            base.OnTabClick(sender, e);

            MultiView.ActiveViewIndex = SelectedTabIndex;
        }

        protected override void OnInit(EventArgs e)
        {
            UpdateTabs();

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdateTabs();

            base.OnPreRender(e);
        }

        public void UpdateTabs()
        {
            Tabs.Clear();

            foreach (var v in MultiView.Views.Cast<TabView>())
            {
                var t = new Tab();
                t.Text = v.Text;
                t.Visible = !v.Hidden;

                Tabs.Add(t);
            }

            SelectedTabIndex = MultiView.ActiveViewIndex;
        }

    }
}
