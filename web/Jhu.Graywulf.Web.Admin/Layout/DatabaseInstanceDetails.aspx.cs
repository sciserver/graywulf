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

            DatabaseName.Text = item.DatabaseName;
            ServerInstance.EntityReference.Value = item.ServerInstance;
            Slice.EntityReference.Value = item.Slice;
            DatabaseVersion.EntityReference.Value = item.DatabaseVersion;

            Allocate.Visible = item.DeploymentState == DeploymentState.New || item.DeploymentState == DeploymentState.Undeployed;
            Drop.Visible = item.DeploymentState == DeploymentState.Deployed;

            Attach.Visible = item.DeploymentState == DeploymentState.Deployed && item.RunningState == RunningState.Detached;
            Detach.Visible = item.DeploymentState == DeploymentState.Deployed && item.RunningState == RunningState.Attached;
        }

        protected override void InitLists()
        {
            base.InitLists();

            FileGroupList.ParentEntity = item;
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Allocate":
                    item.Deploy();
                    break;
                case "Drop":
                    item.Undeploy();
                    break;
                case "Attach":
                    item.Attach();
                    break;
                case "Detach":
                    item.Detach();
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }

            UpdateForm();
        }

    }
}