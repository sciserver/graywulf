using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class SourceTableForm : CustomUserControlBase
    {
        public string SelectedValue
        {
            get { return null; }
            set { }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshTableList();
            }
        }

        private void RefreshTableList()
        {
            FederationContext.MyDBDataset.Tables.LoadAll();

            foreach (var table in FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                TableName.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }
        }

        public TableOrView GetTable()
        {
            return null;
        }
    }
}