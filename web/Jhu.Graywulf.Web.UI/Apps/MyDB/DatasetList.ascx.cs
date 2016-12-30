using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class DatasetList : FederationUserControlBase
    {
        public event EventHandler SelectedDatasetChanged;

        public string CssClass
        {
            get { return datasetList.CssClass; }
            set { datasetList.CssClass = value; }
        }

        public bool AutoPostBack
        {
            get { return datasetList.AutoPostBack; }
            set { datasetList.AutoPostBack = value; }
        }

        public string Style
        {
            get { return datasetList.Style.Value; }
            set { datasetList.Style.Value = value; }
        }

        public string DefaultRequestField
        {
            get { return (string)ViewState["DefaultRequestField"]; }
            set { ViewState["DefaultRequestField"] = value; }
        }

        public string TableListControl
        {
            get { return (string)ViewState["TableListControl"]; }
            set { ViewState["TableListControl"] = value; }
        }

        public string DatasetName
        {
            get { return datasetList.SelectedValue; }
            set { datasetList.SelectedValue = value; }
        }

        public DatasetBase Dataset
        {
            get { return FederationContext.SchemaManager.Datasets[datasetList.SelectedValue]; }
            set { datasetList.SelectedValue = value.Name; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && datasetList.Items.Count == 0)
            {
                RefreshDatasetList();
            }
        }

        protected void DatasetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDatasetChanged?.Invoke(sender, e);
            RefreshTableList();
        }

        public void RefreshDatasetList()
        {
            var uf = UserDatabaseFactory.Create(RegistryContext.Federation);
            var mydbds = uf.GetUserDatabases(FederationContext.RegistryUser);

            datasetList.Items.Clear();

            foreach (var key in mydbds.Keys)
            {
                var mydbli = new ListItem(key, key);

                if (mydbds[key].IsInError)
                {
                    mydbli.Text += " (not available)";
                }

                datasetList.Items.Add(mydbli);

            }

            if (!IsPostBack && DefaultRequestField != null)
            {
                var reqds = Request[DefaultRequestField];

                if (reqds != null)
                {
                    if (!reqds.Contains("|"))
                    {
                        datasetList.SelectedValue = reqds;
                    }
                    else
                    {
                        DatabaseObjectType type;
                        string datasetName, databaseName, schemaName, objectName;

                        FederationContext.SchemaManager.GetNamePartsFromKey(
                            reqds,
                            out type,
                            out datasetName,
                            out databaseName,
                            out schemaName,
                            out objectName);

                        datasetList.SelectedValue = datasetName;
                    }
                }
            }

            if (!IsPostBack)
            {
                RefreshTableList();
            }
        }

        private void RefreshTableList()
        {
            if (TableListControl != null)
            {
                var tableList = (TableList)Page.FindControlRecursive(TableListControl);
                tableList.Dataset = this.Dataset;
            }
        }
    }
}