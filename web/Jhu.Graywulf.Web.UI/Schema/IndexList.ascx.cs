using System;
using schema = Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Schema
{
    public partial class IndexList : System.Web.UI.UserControl
    {
        protected schema::IIndexes databaseObject;

        public schema::IIndexes DatabaseObject
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
                listView.DataSource = databaseObject.Indexes.Values;
                listView.DataBind();
            }
        }
    }
}