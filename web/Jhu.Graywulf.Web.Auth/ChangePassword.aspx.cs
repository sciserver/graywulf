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
                // Change password mode
                OldPasswordRow.Visible = true;
                user = RegistryUser;
            }
            else
            {
                // Reset password mode
                OldPasswordRow.Visible = false;
                user = IdentityProvider.GetUserByActivationCode(Request.QueryString["code"]);
            }
        }

        protected void OldPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RegistryUser != null)
            {
                try
                {
                    user = IdentityProvider.VerifyPassword(user.Name, OldPassword.Text);
                    args.IsValid = true;
                }
                catch (Exception)
                {
                }
            }

            args.IsValid = false;
        }

        protected void ConfirmPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (Password.Text == ConfirmPassword.Text);
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (RegistryUser != null)
                {
                    IdentityProvider.ChangePassword(user, OldPassword.Text, Password.Text);
                }
                else
                {
                    IdentityProvider.ResetPassword(user, Password.Text);
                }

                ChangePasswordForm.Visible = false;
                SuccessForm.Visible = true;
            }
        }
    }
}