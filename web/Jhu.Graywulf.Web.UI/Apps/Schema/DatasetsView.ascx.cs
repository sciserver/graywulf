using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DatasetsView : FederationUserControlBase
    {
        public static string GetIconUrl(DatasetBase dataset)
        {
            return String.Format("~/Assets/Datasets/Icons/{0}", dataset.Metadata.Icon);
        }

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
                RefreshForm();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void RefreshForm()
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

                if (!String.IsNullOrWhiteSpace(ds.Metadata.Icon))
                {
                    var img = (Image)e.Item.FindControl("icon");
                    img.ImageUrl = GetIconUrl(ds);
                }
            }
        }

        protected void ListView_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var di = (ListViewDataItem)e.Item;
                var ds = (DatasetBase)di.DataItem;

                switch (e.CommandName)
                {
                    case "dataset_click":

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
    }
}