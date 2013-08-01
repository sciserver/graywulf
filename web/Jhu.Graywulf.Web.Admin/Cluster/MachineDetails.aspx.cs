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

namespace Jhu.Graywulf.Web.Admin.Cluster
{
    public partial class MachineDetails : EntityDetailsPageBase<Machine>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            HostName.Text = String.Format("{0} ({1})", item.HostName.Value, item.HostName.ResolvedValue);
            AdminUrl.Text = String.Format("{0} ({1})", item.AdminUrl.Value, item.AdminUrl.ResolvedValue);
            AdminUrl.NavigateUrl = item.AdminUrl.ResolvedValue;
            DeployUncPath.Text = String.Format("{0} ({1})", item.DeployUncPath.Value, item.DeployUncPath.ResolvedValue);
            DeployUncPath.NavigateUrl = item.DeployUncPath.ResolvedValue;
        }

        protected override void InitLists()
        {
            base.InitLists();

            ServerInstanceList.ParentEntity = item;
            DiskVolumeList.ParentEntity = item;
        }

        /*
        protected void ServerInstanceList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/cluster/ServerInstanceDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddServerInstance_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.ServerInstance));
        }

        protected void DiskVolumeList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/cluster/DiskVolumeDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddDiskVolume_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.DiskVolume));
        }

        protected void MoveServerInstance_Click(object sender, EventArgs e)
        {
            if (sender == MoveDownServerInstance)
                MoveItem<ServerInstance>(ServerInstanceList.FirstSelectedKey, EntityMoveDirection.Down);
            else if (sender == MoveUpServerInstance)
                MoveItem<ServerInstance>(ServerInstanceList.FirstSelectedKey, EntityMoveDirection.Up);
        }

        protected void DeleteServerInstance_Click(object sender, EventArgs e)
        {

        }
        protected void MoveDiskVolume_Click(object sender, EventArgs e)
        {
            if (sender == MoveDownDiskVolume)
                MoveItem<DiskVolume>(DiskVolumeList.FirstSelectedKey, EntityMoveDirection.Down);
            else if (sender == MoveUpDiskVolume)
                MoveItem<DiskVolume>(DiskVolumeList.FirstSelectedKey, EntityMoveDirection.Up);
        }
        protected void DeleteDiskVolume_Click(object sender, EventArgs e)
        {

        }*/
    }
}