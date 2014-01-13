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
    public partial class UserDatabaseInstanceForm : EntityFormPageBase<UserDatabaseInstance>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshUserList();
            if (Item.IsExisting)
            {
                UserList.SelectedValue = Item.User.Guid.ToString();
            }

            RefreshDatabaseInstanceList();
            if (Item.IsExisting)
            {
                DatabaseInstanceList.SelectedValue = Item.DatabaseInstance.Guid.ToString();
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.UserReference.Guid = new Guid(UserList.SelectedValue);
            Item.DatabaseInstanceReference.Guid = new Guid(DatabaseInstanceList.SelectedValue);
        }

        private void RefreshUserList()
        {
            UserList.Items.Clear();

            var domain = Item.DatabaseVersion.DatabaseDefinition.Federation.Domain;
            domain.LoadUsers(false);

            foreach (var user in domain.Users.Values.OrderBy(i => i.Name))
            {
                UserList.Items.Add(new ListItem(user.Name, user.Guid.ToString()));
            }
        }

        private void RefreshDatabaseInstanceList()
        {
            var dv = Item.DatabaseVersion;
            var dd = dv.DatabaseDefinition;
            dd.LoadDatabaseInstances(false);

            foreach (var di in dd.DatabaseInstances.Values.Where(dii => dii.DatabaseVersionReference.Guid == dv.Guid))  // TODO
            {
                DatabaseInstanceList.Items.Add(new ListItem(di.Name, di.Guid.ToString()));
            }
        }
    }
}