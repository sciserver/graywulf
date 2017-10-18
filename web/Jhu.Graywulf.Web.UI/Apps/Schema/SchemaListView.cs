using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public abstract class SchemaListView<T> : SchemaView
        where T : IMetadata
    {
        protected MultiSelectListView listView;
        private IEnumerable<T> items;

        public IEnumerable<T> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                UpdateView();
            }
        }

        public override void UpdateView()
        {
            if (items != null)
            {
                listView.DataSource = items;
                listView.DataBind();
            }
        }

        protected void ListView_ItemCreated(object sender, System.Web.UI.WebControls.ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var di = (ListViewDataItem)e.Item;
                var item = (T)di.DataItem;
                var img = (Image)e.Item.FindControl("icon");

                if (!String.IsNullOrWhiteSpace(item?.Metadata?.Icon))
                {
                    img.ImageUrl = GetIconUrl(item.Metadata);
                }
                else
                {
                    img.Height = System.Web.UI.WebControls.Unit.Parse("1px");
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