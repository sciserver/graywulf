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
    public partial class MachineRoleDetails : EntityDetailsPageBase<MachineRole>
    {
        protected override void UpdateForm()
        {
            MachineRoleType.Text = item.MachineRoleType.ToString();
        }

        protected override void InitLists()
        {
            MachineList.ParentEntity = item;
            ServerVersionList.ParentEntity = item;
        }

        /*
        protected void MachineList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/cluster/MachineDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddMachine_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.Machine));
        }

        protected void DeleteMachine_Click(object sender, EventArgs e)
        {
            if (MachineList.SelectedKeys.Count == 1)
            {
                Response.Redirect("../EntityDelete.aspx?guid=" + MachineList.FirstSelectedKey + "&redirect=" + HttpUtility.UrlEncode(Request.RawUrl));
            }
        }

        protected void CopyMachine_Click(object sender, EventArgs e)
        {
            if (MachineList.SelectedKeys.Count == 1)
            {
                Machine m = new Machine(RegistryContext);
                m.Guid = new Guid(MachineList.SelectedKeys.First());
                m.Load();

                Machine nm = m.CreateCopy(item);
                Response.Redirect(nm.GetFormUrl());
            }
        }

        protected void MoveMachine_Click(object sender, EventArgs e)
        {
            if (MachineList.SelectedKeys.Count == 1)
            {
                if (sender == MoveUpMachine)
                {
                    MoveItem<Machine>(MachineList.FirstSelectedKey, EntityMoveDirection.Up);
                }
                else if (sender == MoveDownMachine)
                {
                    MoveItem<Machine>(MachineList.FirstSelectedKey, EntityMoveDirection.Down);
                }
            }

            MachineList.DataBind();
        }

        protected void ServerVersionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect(item.GetChildDetailsUrl(EntityGroup.Cluster, EntityType.ServerVersion, Guid.Parse((string)e.CommandArgument)));
        }

        protected void AddServerVersion_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.ServerVersion));
        }

        protected void DeleteServerVersion_Click(object sender, EventArgs e)
        {
            if (ServerVersionList.SelectedKeys.Count == 1)
            {
                Response.Redirect("../EntityDelete.aspx?guid=" + ServerVersionList.FirstSelectedKey + "&redirect=" + HttpUtility.UrlEncode(Request.RawUrl));
            }
        }*/
    }
}