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
    public partial class UserDetails : EntityDetailsPageBase<Registry.User>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            FirstName.Text = Item.FirstName;
            MiddleName.Text = Item.MiddleName;
            LastName.Text = Item.LastName;
            Email.Text = Item.Email;
            Company.Text = Item.Company;
            NtlmUser.Text = Item.NtlmUser;
        }

        protected override void InitLists()
        {
            base.InitLists();

            UserDatabaseInstanceList.ParentEntity = Item;
        }

        private void RemoveUserGroup(Guid[] guids)
        {
            foreach (var g in guids)
            {
                Item.RemoveMemberOf(g);
            }

            Item.LoadUserGroups();
        }

    }
}