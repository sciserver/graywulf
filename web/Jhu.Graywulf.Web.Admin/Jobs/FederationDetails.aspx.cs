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

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class FederationDetails : EntityDetailsPageBase<Registry.Federation>
    {
        protected override void InitLists()
        {
            base.InitLists();

            QueueDefinitionList.ParentEntity = Item;
            JobDefinitionList.ParentEntity = Item;
        }

        /*
        protected void QueueDefinitionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/QueueDefinitionDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddQueueDefinition_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.QueueDefinition));
        }

        protected void JobDefinitionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/JobDefinitionDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddJobDefinition_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.JobDefinition));
        }*/
    }
}