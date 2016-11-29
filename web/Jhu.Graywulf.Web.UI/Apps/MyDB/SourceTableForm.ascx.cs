using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class SourceTableForm : FederationUserControlBase
    {
        public ListControl DatasetList
        {
            get { return datasetList; }
        }

        public DatasetBase Dataset
        {
            get
            {
                return FederationContext.SchemaManager.Datasets[datasetList.SelectedValue];
            }
        }

        public TableOrView Table
        {
            get
            {
                var userdb = Dataset;
                var usertable = userdb.Tables[tableList.SelectedValue];
                return usertable;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void DatasetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTableList();
        }

        public void RefreshTableList()
        {
            var userdb = Dataset;
            userdb.Tables.LoadAll(true);

            tableList.Items.Clear();

            foreach (var table in userdb.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                tableList.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }

            if (!IsPostBack)
            {
                string objid = Request.QueryString["objid"];
                if (objid != null)
                {
                    tableList.SelectedValue = objid;
                }
            }
        }
    }
}