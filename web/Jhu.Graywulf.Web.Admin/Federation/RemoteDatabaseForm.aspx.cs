using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
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
    public partial class RemoteDatabaseForm : EntityFormPageBase<RemoteDatabase>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshProviderList();

            ProviderName.SelectedValue = item.ProviderName;
            ConnectionString.Text = item.ConnectionString;
            IntegratedSecurity.Checked = item.IntegratedSecurity;
            Username.Text = item.Username;
            Password.Text = item.Password;
            RequiresSshTunnel.Checked = item.RequiresSshTunnel;
            SshHostName.Text = item.SshHostName;
            SshPortNumber.Text = item.SshPortNumber.ToString();
            SshUsername.Text = item.SshUsername;
            SshPassword.Text = item.SshPassword;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.ProviderName = ProviderName.SelectedValue;
            item.ConnectionString = ConnectionString.Text;
            item.IntegratedSecurity = IntegratedSecurity.Checked;
            item.Username = Username.Text;
            item.Password = Password.Text;
            item.RequiresSshTunnel = RequiresSshTunnel.Checked;
            item.SshHostName = SshHostName.Text;
            item.SshPortNumber = int.Parse(SshPortNumber.Text);
            item.SshUsername = SshUsername.Text;
            item.SshPassword = SshPassword.Text;
        }

        private void RefreshProviderList()
        {
            DataTable dt = DbProviderFactories.GetFactoryClasses();

            ProviderName.Items.Add(new ListItem("(select provider)", ""));
            foreach (DataRow tr in dt.Rows)
            {
                ProviderName.Items.Add(new ListItem(tr[0].ToString(), tr[2].ToString()));
            }
        }
    }
}