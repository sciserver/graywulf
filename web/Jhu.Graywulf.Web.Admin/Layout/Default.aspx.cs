using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class Default : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(ClusterDetails.GetUrl((Guid)Session[Web.UI.Constants.SessionClusterGuid]), false);
        }
    }
}