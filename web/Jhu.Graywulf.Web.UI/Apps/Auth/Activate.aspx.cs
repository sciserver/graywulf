using System;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.UI.Apps.Auth
{
    public partial class Activate : PageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Auth/Activate.aspx";
        }

        public static string GetUrl(string returnUrl)
        {
            return String.Format("{0}?ReturnUrl={1}", GetUrl(), returnUrl);
        }

        protected Jhu.Graywulf.Registry.User user;

        private bool ActivateUser(string code)
        {
            try
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);

                user = ip.GetUserByActivationCode(code);
                ip.ActivateUser(user);

                return true;
            }
            catch (EntityNotFoundException)
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];

            if (!String.IsNullOrEmpty(code))
            {
                if (ActivateUser(code))
                {
                    ActivateUserForm.Visible = false;
                    SuccessForm.Visible = true;
                }
            }

            UserLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.User.GetUrl(ReturnUrl);
            SignInLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.SignIn.GetUrl(ReturnUrl);
            SignInLink2.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Auth.SignIn.GetUrl(ReturnUrl);
        }

        protected void Activate_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                ActivateUserForm.Visible = false;
                SuccessForm.Visible = true;
            }
        }

        protected void ActivationCodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string code = args.Value;

            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    args.IsValid = ActivateUser(code);
                }
            }
            catch (EntityNotFoundException)
            {
                args.IsValid = false;
            }
        }

        protected void SignIn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Auth.SignIn.GetUrl(ReturnUrl), false);
        }
    }
}