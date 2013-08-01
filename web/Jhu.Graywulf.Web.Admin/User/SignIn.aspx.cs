using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.User
{
    public partial class SignIn : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/User/SignIn.aspx?ReturnUrl={0}", returnUrl);
        }

        private Jhu.Graywulf.Registry.User user;

        protected override void EnsureUserIdentified()
        {
            // Override default behavior to do nothing
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SignInForm.Text = String.Format("Welcome to {0}", Application[Jhu.Graywulf.Web.Constants.ApplicationShortTitle]);

                // Load clusters
                EntityFactory cf = new EntityFactory(RegistryContext);
                foreach (Registry.Cluster c in cf.FindAll<Registry.Cluster>())
                {
                    ClusterList.Items.Add(new ListItem(c.Name, c.Guid.ToString()));
                }
                
                ClusterList.SelectedIndex = 0;
            }
        }

        protected void PasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Attempt to log in with supplied credentials
            try
            {
                var c = new Registry.Cluster(RegistryContext);
                c.Guid = new Guid(ClusterList.SelectedValue);
                c.Load();

                var uu = new UserFactory(RegistryContext);

                user = uu.LoginUser(c, Username.Text, Password.Text);

                RegistryContext.UserGuid = user.Guid;
                RegistryContext.UserName = user.Name;

                args.IsValid = true;
            }
            catch (EntityNotFoundException)
            {
                args.IsValid = false;
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                FormsAuthentication.RedirectFromLoginPage(user.Guid.ToString(), Remember.Checked);
            }
        }
    }
}