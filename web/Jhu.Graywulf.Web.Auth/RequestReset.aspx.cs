using System;
using System.Web.UI.WebControls;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class RequestReset : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/RequestReset.aspx?ReturnUrl={0}", returnUrl);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SingInLink.NavigateUrl = SignIn.GetUrl(ReturnUrl);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SignIn.GetUrl(ReturnUrl), false);
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                RequestResetForm.Visible = false;
                SuccessForm.Visible = true;
            }
        }

        protected void EmailValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);
                var uu = new UserFactory(RegistryContext);
                var user = ip.GetUserByEmail(args.Value);

                user.GenerateActivationCode();

                ip.ModifyUser(user);

                Util.EmailSender.Send(user, File.ReadAllText(MapPath("~/Templates/RequestResetEmail.xml")), BaseUrl);

                args.IsValid = true;
            }
            catch (Exception)
            {
                args.IsValid = false;    
            }
        }
    }
}