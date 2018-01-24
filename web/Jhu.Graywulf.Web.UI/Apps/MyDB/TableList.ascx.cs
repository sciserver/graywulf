using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class TableList : FederationUserControlBase
    {
        public event EventHandler SelectedTableChanged;

        public string CssClass
        {
            get { return tableList.CssClass; }
            set { tableList.CssClass = value; }
        }

        public bool AutoPostBack
        {
            get { return tableList.AutoPostBack; }
            set { tableList.AutoPostBack = value; }
        }

        public string Style
        {
            get { return tableList.Style.Value; }
            set { tableList.Style.Value = value; }
        }

        public string DefaultRequestField
        {
            get { return (string)ViewState["DefaultRequestField"]; }
            set { ViewState["DefaultRequestField"] = value; }
        }

        public string DatasetName
        {
            get { return (string)ViewState["DatasetName"]; }
            set
            {
                ViewState["DatasetName"] = value;
                RefreshTableList();
            }
        }

        public DatasetBase Dataset
        {
            get { return FederationContext.SchemaManager.Datasets[DatasetName]; }
            set { DatasetName = value.Name; }
        }

        public string TableID
        {
            get { return tableList.SelectedValue; }
            set { tableList.SelectedValue = value; }
        }

        public Jhu.Graywulf.Schema.Table Table
        {
            get { return Dataset.Tables[tableList.SelectedValue]; }
            set { tableList.SelectedValue = value.UniqueKey; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && DatasetName != null && tableList.Items.Count == 0)
            {
                RefreshTableList();
            }
        }

        protected void TableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTableChanged?.Invoke(sender, e);
        }

        public void RefreshTableList()
        {
            var ds = Dataset;
            ds.Tables.LoadAll(true);

            tableList.Items.Clear();

            foreach (var table in ds.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                tableList.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }

            if (!IsPostBack && DefaultRequestField != null)
            {
                var objid = Request[DefaultRequestField];

                if (objid != null)
                {
                    tableList.SelectedValue = objid;
                }
            }
        }
    }
}