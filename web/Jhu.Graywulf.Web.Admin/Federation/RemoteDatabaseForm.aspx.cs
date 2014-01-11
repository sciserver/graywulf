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

            ProviderName.SelectedValue = Item.ProviderName;
            ConnectionString.Text = Item.ConnectionString;
            IntegratedSecurity.Checked = Item.IntegratedSecurity;
            Username.Text = Item.Username;
            Password.Text = Item.Password;
            RequiresSshTunnel.Checked = Item.RequiresSshTunnel;
            SshHostName.Text = Item.SshHostName;
            SshPortNumber.Text = Item.SshPortNumber.ToString();
            SshUsername.Text = Item.SshUsername;
            SshPassword.Text = Item.SshPassword;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.ProviderName = ProviderName.SelectedValue;
            Item.ConnectionString = ConnectionString.Text;
            Item.IntegratedSecurity = IntegratedSecurity.Checked;
            Item.Username = Username.Text;
            Item.Password = Password.Text;
            Item.RequiresSshTunnel = RequiresSshTunnel.Checked;
            Item.SshHostName = SshHostName.Text;
            Item.SshPortNumber = int.Parse(SshPortNumber.Text);
            Item.SshUsername = SshUsername.Text;
            Item.SshPassword = SshPassword.Text;
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