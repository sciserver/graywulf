using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class EntityButtons : System.Web.UI.UserControl
    {

        public bool BasicButtonsVisible
        {
            get { return (bool)(ViewState["BasicButtonsVisible"] ?? true); }
            set { ViewState["BasicButtonsVisible"] = value; }
        }

        public bool DeploymentStateButtonsVisible
        {
            get { return (bool)(ViewState["DeploymentStateButtonsVisible"] ?? false); }
            set { ViewState["DeploymentStateButtonsVisible"] = value; }
        }

        public bool RunningStateButtonsVisible
        {
            get { return (bool)(ViewState["DeploymentStateButtonsVisible"] ?? false); }
            set { ViewState["DeploymentStateButtonsVisible"] = value; }
        }

        public bool DiscoverButtonVisible
        {
            get { return (bool)(ViewState["DiscoverButtonVisible"] ?? false); }
            set { ViewState["DiscoverButtonVisible"] = value; }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var item = ((IEntityForm)Page).Item;

            BasicButtons.Visible = BasicButtonsVisible;

            Edit.Enabled = !item.CanModify();
            Delete.Enabled = item.CanDelete(false, false);

            // Show / Hide
            ToggleShowHide.Visible = BasicButtonsVisible;
            ToggleShowHide.Enabled = !item.CanModify();
            if (item.Hidden)
            {
                ToggleShowHide.Text = "Show";
            }
            else
            {
                ToggleShowHide.Text = "Hide";
            }
            
            // Deployment state
            ToggleDeploymentState.Visible = DeploymentStateButtonsVisible;
            if (item.DeploymentState == DeploymentState.Deployed)
            {
                ToggleDeploymentState.Enabled = item.CanUndeploy(false);
                ToggleDeploymentState.Text = "Undeploy";
            }
            else
            {
                ToggleDeploymentState.Enabled = item.CanDeploy(false);
                ToggleDeploymentState.Text = "Deploy";
            }

            // Running state
            ToggleRunningState.Visible = RunningStateButtonsVisible;
            if (item.RunningState == RunningState.Running)
            {
                ToggleRunningState.Text = "Stop";
            }
            else
            {
                ToggleRunningState.Text = "Start";
            }

            // 
            DiscoverButton.Visible = DiscoverButtonVisible;
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            ((IEntityForm)Page).OnButtonCommand(sender, e);
        }
    }
}