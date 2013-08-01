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

            ShortTitle.Text = item.ShortTitle;
            LongTitle.Text = item.LongTitle;
            Email.Text = item.Email;
            StandardUserGroup.SelectedValue = item.StandardUserGroupReference.Guid.ToString();

            if (!item.IsExisting)
            {
                StandardUserGroupRow.Visible = false;
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.ShortTitle = ShortTitle.Text;
            item.LongTitle = LongTitle.Text;
            item.Email = Email.Text;
            item.StandardUserGroupReference.Guid = new Guid(StandardUserGroup.SelectedValue);
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var conf = new DomainInstaller(item);
                conf.GenerateDefaultChildren();
            }
        }

        protected void RefreshStandardUserGroupList()
        {
            StandardUserGroup.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.LoadUserGroups(false);

            foreach (UserGroup ug in item.UserGroups.Values)
            {
                StandardUserGroup.Items.Add(new ListItem(ug.Name, ug.Guid.ToString()));
            }
        }
    }
}