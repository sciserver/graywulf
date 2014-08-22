using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Domain
{
    public partial class AddUserRoleMember : PageBase
    {
        Registry.User user;

        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Domain/AddUserRoleMember.aspx?guid={0}", guid);
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
            IEnumerable<Registry.UserRole> userroles;

            var d = user.Domain;
            d.LoadUserRoles(true);
            userroles = d.UserRoles.Values;

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
                user.AddToRole(new Guid(UserRole.SelectedValue));

                Response.Redirect(user.GetDetailsUrl());
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(user.GetDetailsUrl());
        }
    }
}