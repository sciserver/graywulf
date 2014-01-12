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
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class DomainForm : EntityFormPageBase<Registry.Domain>
    {
        protected Registry.Cluster cluster;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshStandardUserGroupList();

            ShortTitle.Text = Item.ShortTitle;
            LongTitle.Text = Item.LongTitle;
            Email.Text = Item.Email;
            Copyright.Text = Item.Copyright;
            Disclaimer.Text = Item.Disclaimer;
            StandardUserGroup.SelectedValue = Item.StandardUserGroupReference.Guid.ToString();

            if (!Item.IsExisting)
            {
                StandardUserGroupRow.Visible = false;
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.ShortTitle = ShortTitle.Text;
            Item.LongTitle = LongTitle.Text;
            Item.Email = Email.Text;
            Item.Copyright = Copyright.Text;
            Item.Disclaimer = Disclaimer.Text;
            Item.StandardUserGroupReference.Guid = new Guid(StandardUserGroup.SelectedValue);
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var conf = new DomainInstaller(Item);
                conf.GenerateDefaultChildren();
            }
        }

        protected void RefreshStandardUserGroupList()
        {
            StandardUserGroup.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Item.LoadUserGroups(false);

            foreach (UserGroup ug in Item.UserGroups.Values)
            {
                StandardUserGroup.Items.Add(new ListItem(ug.Name, ug.Guid.ToString()));
            }
        }
    }
}