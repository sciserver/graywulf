using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Api
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return "~/Api/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}