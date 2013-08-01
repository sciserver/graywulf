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
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class FederationDetails : EntityDetailsPageBase<Registry.Federation>
    {
        protected override void InitLists()
        {
            base.InitLists();

            DatabaseDefinitionList.ParentEntity = item;
        }

        /*
        protected void DatabaseDefinitionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/layout/DatabaseDefinitionDetails.aspx?Guid=" + e.CommandArgument);
        }*/
    }
}