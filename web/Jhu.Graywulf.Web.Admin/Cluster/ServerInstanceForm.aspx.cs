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
    public partial class ServerInstanceForm : EntityFormPageBase<ServerInstance>
    {

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshServerVersionList();

            ServerVersion.SelectedValue = Item.ServerVersionReference.Guid.ToString();
            InstanceName.Text = Item.InstanceName;
            IntegratedSecurity.Checked = Item.IntegratedSecurity;
            AdminUser.Text = Item.AdminUser;
            AdminPassword.Text = Item.AdminPassword;

            AdminUser.Enabled = !IntegratedSecurity.Checked;
            AdminPassword.Enabled = !IntegratedSecurity.Checked;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.ServerVersionReference.Guid = new Guid(ServerVersion.SelectedValue);
            Item.InstanceName = InstanceName.Text;
            Item.IntegratedSecurity = IntegratedSecurity.Checked;
            Item.AdminUser = AdminUser.Text;
            Item.AdminPassword = AdminPassword.Text;
        }

        private void RefreshServerVersionList()
        {
            Item.Machine.MachineRole.LoadServerVersions(false);

            ServerVersion.Items.Add(new ListItem("(select version)", Guid.Empty.ToString()));
            foreach (ServerVersion sv in Item.Machine.MachineRole.ServerVersions.Values)
            {
                ServerVersion.Items.Add(new ListItem(sv.Name, sv.Guid.ToString()));
            }
        }

        protected void IntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            AdminUser.Enabled = !IntegratedSecurity.Checked;
            AdminPassword.Enabled = !IntegratedSecurity.Checked;
        }

        protected void ServerVersion_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}