using System;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Toolbar : FederationUserControlBase
    {
        public event EventHandler<EventArgs> SelectedDatasetChanged;

        public DropDownList DatasetList
        {
            get { return datasetList; }
        }

        protected void DatasetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDatasetChanged?.Invoke(sender, e);
        }
    }
}