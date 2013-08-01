using System;
using System.Linq;
using schema = Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Schema
{
    public partial class ParameterList : System.Web.UI.UserControl
    {
        protected schema::IParameters databaseObject;

        public schema::IParameters DatabaseObject
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
                listView.DataSource = databaseObject.Parameters.Values.OrderBy(i => i.ID);
                listView.DataBind();
            }
        }
    }
}