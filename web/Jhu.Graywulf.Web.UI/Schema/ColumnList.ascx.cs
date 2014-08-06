using System;
using System.Linq;
using schema = Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.UI.Schema
{
    public partial class ColumnList : UserUserControlBase
    {
        protected schema::IColumns databaseObject;

        public schema::IColumns DatabaseObject
        {
            get { return databaseObject; }
            set
            {
                databaseObject = value;
                RefreshForm();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void RefreshForm()
        {
            if (databaseObject != null)
            {
                listView.DataSource = databaseObject.Columns.Values.OrderBy(i => i.ID);
                listView.DataBind();
            }
        }
    }
}