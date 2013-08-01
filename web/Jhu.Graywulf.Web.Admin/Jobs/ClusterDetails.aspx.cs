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
    public partial class ClusterDetails : EntityDetailsPageBase<Registry.Cluster>
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/ClusterDetails.aspx?guid={0}", guid);
        }

        protected override void InitLists()
        {
            base.InitLists();

            MachineRoleList.ParentEntity = item;
            DomainList.ParentEntity = item;
            QueueDefinitionList.ParentEntity = item;
            JobDefinitionList.ParentEntity = item;
        }

        /*
        protected void MachineRoleList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/MachineRoleDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void DomainList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/DomainDetails.aspx?Guid=" + e.CommandArgument);
        }

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
            Response.Redirect(EntityExtensions.GetDetailsUrl(
                EntityType.JobDefinition,
                EntityGroup.Jobs,
                Guid.Parse((string)e.CommandArgument)));
        }

        protected void AddJobDefinition_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.JobDefinition));
        }*/
    }
}