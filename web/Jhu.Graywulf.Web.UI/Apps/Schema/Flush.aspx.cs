using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Flush : FederationPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FederationContext.SchemaManager.Flush();

            Response.Write("Schema cache flushed.");
            Response.End();
        }
    }
}