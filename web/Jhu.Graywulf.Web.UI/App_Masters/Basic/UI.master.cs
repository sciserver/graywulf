using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Masters
{
    public partial class UI : System.Web.UI.MasterPage
    {
        public Controls.UserStatus UserStatus
        {
            get { return userStatus; }
        }

        public Controls.Menu Menu
        {
            get { return menu; }
        }

        public Controls.Footer Footer
        {
            get { return footer; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}