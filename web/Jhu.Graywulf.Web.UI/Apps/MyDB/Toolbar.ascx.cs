using System;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Toolbar : FederationUserControlBase
    {
        public event EventHandler<EventArgs> SelectedDatasetChanged;

        public string DatasetName
        {
            get { return datasetList.DatasetName; }
            set { datasetList.DatasetName = value; }
        }

        public DatasetBase Dataset
        {
            get { return datasetList.Dataset; }
            set { datasetList.Dataset = value; }
        }
        
        protected void DatasetList_SelectedDatasetChanged(object sender, EventArgs e)
        {
            SelectedDatasetChanged?.Invoke(sender, e);
        }

        public void RefreshDatasetList()
        {
            datasetList.RefreshDatasetList();
        }
    }
}