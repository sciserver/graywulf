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
    public partial class DatabaseInstanceDetails : EntityDetailsPageBase<DatabaseInstance>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            DatabaseName.Text = Item.DatabaseName;
            ServerInstance.EntityReference.Value = Item.ServerInstance;
            Slice.EntityReference.Value = Item.Slice;
            DatabaseVersion.EntityReference.Value = Item.DatabaseVersion;

            Allocate.Visible = Item.DeploymentState == DeploymentState.New || Item.DeploymentState == DeploymentState.Undeployed;
            Drop.Visible = Item.DeploymentState == DeploymentState.Deployed;

            Attach.Visible = Item.DeploymentState == DeploymentState.Deployed && Item.RunningState == RunningState.Detached;
            Detach.Visible = Item.DeploymentState == DeploymentState.Deployed && Item.RunningState == RunningState.Attached;
        }

        protected override void InitLists()
        {
            base.InitLists();

            FileGroupList.ParentEntity = Item;
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Allocate":
                    Item.Deploy();
                    break;
                case "Drop":
                    Item.Undeploy();
                    break;
                case "Attach":
                    Item.Attach();
                    break;
                case "Detach":
                    Item.Detach();
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }

            UpdateForm();
        }

    }
}