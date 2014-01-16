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
            IEnumerable<Registry.UserGroup> usergroups;

            var d = user.Domain;
            d.LoadUserGroups(true);
            usergroups = d.UserGroups.Values;

            UserGroup.Items.Add(new ListItem("(select user group)", Guid.Empty.ToString()));
            foreach (var ug in usergroups)
            {
                UserGroup.Items.Add(new ListItem(ug.Name, ug.Guid.ToString()));
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Validate();

            if (IsValid)
            {
                LoadItem();
                user.MakeMemberOf(new Guid(UserGroup.SelectedValue));

                Response.Redirect(user.GetDetailsUrl());
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(user.GetDetailsUrl());
        }
    }
}