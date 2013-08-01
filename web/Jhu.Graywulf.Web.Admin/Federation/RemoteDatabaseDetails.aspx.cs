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

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class RemoteDatabaseDetails : EntityDetailsPageBase<RemoteDatabase>
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void UpdateForm()
        {
            base.UpdateForm();

            ProviderName.Text = item.ProviderName.ToString();
            ConnectionString.Text = item.ConnectionString;
            IntegratedSecurity.Text = item.IntegratedSecurity.ToString();
            Username.Text = item.Username;
            Password.Text = item.Password;
            RequiresSshTunnel.Text = item.RequiresSshTunnel.ToString();
            SshHostName.Text = item.SshHostName;
            SshPortNumber.Text = item.SshPortNumber.ToString();
            SshUsername.Text = item.SshUsername;
            SshPassword.Text = item.SshPassword;

            //Activate.Visible = item.DeploymentState != DeploymentState.Deployed || item.RunningState != RunningState.Running;
            //Deactivate.Visible = !Activate.Visible;
        }

        /*
        protected void Activate_Click(object sender, EventArgs e)
        {
            item.DeploymentState = DeploymentState.Deployed;
            item.RunningState = RunningState.Running;

            item.Save();

            UpdateForm();
        }

        protected void Deactivate_Click(object sender, EventArgs e)
        {
            item.DeploymentState = DeploymentState.Undeployed;
            item.RunningState = RunningState.Stopped;

            item.Save();

            UpdateForm();
        }*/
    }
}