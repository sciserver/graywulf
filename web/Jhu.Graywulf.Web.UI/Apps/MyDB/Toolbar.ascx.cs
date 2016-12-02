using System;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Toolbar : FederationUserControlBase
    {
        public event EventHandler<EventArgs> SelectedDatasetChanged;

        public string SelectedTab
        {
            get { return (string)ViewState["SelectedTab"]; }
            set { ViewState["SelectedTab"] = value; }
        }

        public bool DatasetVisible
        {
            get { return datasetListLabel.Visible; }
            set
            {
                datasetListLabel.Visible = value;
                datasetList.Visible = value;
            }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SelectedTab != null)
            {
                var tab = (LinkButton)toolbar.FindControl(SelectedTab);

                if (tab != null)
                {
                    tab.CssClass = "selected";
                }
            }
        }
        
        protected void DatasetList_SelectedDatasetChanged(object sender, EventArgs e)
        {
            SelectedDatasetChanged?.Invoke(sender, e);
        }

        public void RefreshDatasetList()
        {
            if (datasetList.Visible)
            {
                datasetList.RefreshDatasetList();
            }
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "summary":
                    Response.Redirect(Default.GetUrl(datasetList.DatasetName));
                    break;
                case "tables":
                    Response.Redirect(Tables.GetUrl(datasetList.DatasetName));
                    break;
                case "copy":
                    Response.Redirect(Copy.GetUrl());
                    break;
                case "import":
                    Response.Redirect(Import.GetUrl(datasetList.DatasetName));
                    break;
                case "export":
                    Response.Redirect(Export.GetUrl(datasetList.DatasetName));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}