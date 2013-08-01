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

            FirstName.Text = item.FirstName;
            MiddleName.Text = item.MiddleName;
            LastName.Text = item.LastName;
            Email.Text = item.Email;
            Company.Text = item.Company;
            NtlmUser.Text = item.NtlmUser;
        }

        protected override void InitLists()
        {
            base.InitLists();

            UserDatabaseInstanceList.ParentEntity = item;
        }

        private void RemoveUserGroup(Guid[] guids)
        {
            foreach (var g in guids)
            {
                item.RemoveMemberOf(g);
            }

            item.LoadUserGroups();
        }

    }
}