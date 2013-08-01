using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.Admin
{
    public partial class EntityForm : MasterPage
    {
        public Controls.EntityForm EntityFormControl
        {
            get { return entityForm; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            ((IEntityForm)Page).OnButtonCommand(sender, e);
        }
    }
}