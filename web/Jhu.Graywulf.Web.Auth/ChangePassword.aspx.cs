using System;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Security;

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

                var ip = IdentityProvider.Create(RegistryContext.Domain);
                user = ip.GetUserByActivationCode(Request.QueryString["code"]);
            }
        }

        protected void OldPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RegistryUser != null)
            {
                try
                {
                    var ip = IdentityProvider.Create(RegistryContext.Domain);
                    var response = ip.VerifyPassword(user.Name, OldPassword.Text);

                    user = response.Principal.Identity.User;

                    args.IsValid = true;
                    return;
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
                var ip = IdentityProvider.Create(RegistryContext.Domain);

                if (RegistryUser != null)
                {
                    ip.ChangePassword(user, OldPassword.Text, Password.Text);
                }
                else
                {
                    ip.ResetPassword(user, Password.Text);
                }

                ChangePasswordForm.Visible = false;
                SuccessForm.Visible = true;
            }
        }
    }
}