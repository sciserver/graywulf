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
    public partial class ServerVersionForm : EntityFormPageBase<ServerVersion>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            InstanceName.Text = item.InstanceName;
            IntegratedSecurity.Checked = item.IntegratedSecurity;
            AdminUser.Text = item.AdminUser;
            AdminPassword.Text = item.AdminPassword;

            AdminUser.Enabled = !IntegratedSecurity.Checked;
            AdminPassword.Enabled = !IntegratedSecurity.Checked;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.InstanceName = InstanceName.Text;
            item.IntegratedSecurity = IntegratedSecurity.Checked;
            item.AdminUser = AdminUser.Text;
            item.AdminPassword = AdminPassword.Text;
        }

        protected void IntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            AdminUser.Enabled = !IntegratedSecurity.Checked;
            AdminPassword.Enabled = !IntegratedSecurity.Checked;
        }
    }
}