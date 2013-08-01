using System;
using System.Web.UI.WebControls;
using System.IO;
using Jhu.Graywulf.Registry;

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
            Response.Redirect(SignIn.GetUrl(ReturnUrl));
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
                var uu = new UserFactory(RegistryContext);
                var user = uu.FindUserByEmail(Domain, args.Value);

                user.GenerateActivationCode();
                user.Save();

                Util.EmailSender.Send(user, File.ReadAllText(MapPath("~/Templates/RequestResetEmail.xml")), BaseUrl);

                args.IsValid = true;
            }
            catch (EntityNotFoundException)
            {
                args.IsValid = false;
            }
        }
    }
}