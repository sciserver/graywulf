using System;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class ChangePassword : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/ChangePassword.aspx?ReturnUrl={0}", returnUrl);
        }

        protected Jhu.Graywulf.Registry.User user;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (RegistryUser != null)
            {
                OldPasswordRow.Visible = true;
                user = RegistryUser;
            }
            else
            {
                OldPasswordRow.Visible = false;

                // Load user
                try
                {
                    var uu = new UserFactory(RegistryContext);
                    user = uu.FindUserByActivationCode(RegistryContext.Domain, Request.QueryString["code"]);
                }
                catch (EntityNotFoundException)
                {
                    throw new Registry.SecurityException("Access denied");
                }
            }
        }

        protected void OldPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RegistryUser != null)
            {
                var uu = new UserFactory(RegistryContext);
                args.IsValid = uu.LoginUser(RegistryContext.Domain, user.Name, OldPassword.Text) != null;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void ConfirmPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (Password.Text == ConfirmPassword.Text);
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                user.SetPassword(Password.Text);
                user.ActivationCode = string.Empty;
                user.Save();

                ChangePasswordForm.Visible = false;
                SuccessForm.Visible = true;
            }
        }
    }
}