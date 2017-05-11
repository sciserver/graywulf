using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DatasetsView : SchemaView
    {
        private IEnumerable<DatasetBase> datasets;

        public IEnumerable<DatasetBase> Datasets
        {
            get
            {
                return datasets;
            }
            set
            {
                datasets = value;
                UpdateView();
            }
        }

        public override void UpdateView()
        {
            if (datasets != null)
            {
                listView.DataSource = datasets;
                listView.DataBind();
            }
        }
        
        protected void ListView_ItemCreated(object sender, System.Web.UI.WebControls.ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var di = (ListViewDataItem)e.Item;
                var ds = (DatasetBase)di.DataItem;

                if (ds != null && !String.IsNullOrWhiteSpace(ds.Metadata.Icon))
                {
                    var img = (Image)e.Item.FindControl("icon");
                    img.ImageUrl = GetIconUrl(ds);

                    var link1 = (HyperLink)e.Item.FindControl("link1");
                    var link2 = (HyperLink)e.Item.FindControl("link2");
                    link1.NavigateUrl = link2.NavigateUrl = Page.GetUrl(ds);
                }
            }
        }

        protected void ListView_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                OnCommand(e);
            }
        }
    }
}