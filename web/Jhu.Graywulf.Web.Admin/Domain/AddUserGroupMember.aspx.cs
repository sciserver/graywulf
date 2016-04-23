using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Domain
{
    public partial class AddUserGroupMember : PageBase
    {
        Registry.User user;

        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Domain/AddUserGroupMember.aspx?guid={0}", guid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadItem();
                UpdateForm();
            }
        }

        private void LoadItem()
        {
            user = new Registry.User(RegistryContext);
            user.Guid = new Guid(Request.QueryString["guid"]);
            user.Load();
        }

        private void UpdateForm()
        {
            var d = user.Domain;
            d.LoadUserGroups(true);
            d.LoadUserRoles(true);
            var usergroups = d.UserGroups.Values;
            var userroles = d.UserRoles.Values;

            UserGroup.Items.Add(new ListItem("(select user group)", Guid.Empty.ToString()));
            foreach (var ug in usergroups)
            {
                UserGroup.Items.Add(new ListItem(ug.Name, ug.Guid.ToString()));
            }

            UserRole.Items.Add(new ListItem("(select user role)", Guid.Empty.ToString()));
            foreach (var ur in userroles)
            {
                UserRole.Items.Add(new ListItem(ur.Name, ur.Guid.ToString()));
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Validate();

            if (IsValid)
            {
                LoadItem();
                user.AddToGroup(new Guid(UserGroup.SelectedValue), new Guid(UserRole.SelectedValue));

                Response.Redirect(user.GetDetailsUrl(), false);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(user.GetDetailsUrl(), false);
        }
    }
}