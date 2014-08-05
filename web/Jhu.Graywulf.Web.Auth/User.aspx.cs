using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Auth
{
    public partial class User : PageBase
    {
        public static string GetUrl(string returnUrl)
        {
            return String.Format("~/User.aspx?ReturnUrl={0}", returnUrl);
        }

        protected Jhu.Graywulf.Registry.User user;

        private void UpdateForm()
        {
            if (RegistryUser == null)
            {
                UserForm.Text = "Register new user";
                ChangePasswordPanel.Visible = false;
            }
            else
            {
                UserForm.Text = "Modify user account";
                Username.ReadOnly = true;

                PasswordTable.Visible = false;
                CaptchaTable.Visible = false;
                ChangePasswordPanel.Visible = true;
            }

            Username.Text = user.Name;
            FirstName.Text = user.FirstName;
            MiddleName.Text = user.MiddleName;
            LastName.Text = user.LastName;
            Email.Text = user.Email;
            Company.Text = user.Company;
            Address.Text = user.Address;
            WorkPhone.Text = user.WorkPhone;
        }

        private void SaveForm()
        {
            user.FirstName = FirstName.Text;
            user.MiddleName = MiddleName.Text;
            user.LastName = LastName.Text;
            user.Email = Email.Text;
            user.Company = Company.Text;
            user.Address = Address.Text;
            user.WorkPhone = WorkPhone.Text;
        }

        private void CreateUser()
        {
            var ip = IdentityProvider.Create(RegistryContext.Domain);

            user.Name = Username.Text.Trim();
            ip.CreateUser(user);
            ip.ResetPassword(user, Password.Text);

            // Set user activity status
            if (ip.IsActivationRequired)
            {
                ip.DeactivateUser(user);
            }
            else
            {
                ip.ActivateUser(user);
            }

            // If user signed in with a temporary identity
            if (TemporaryPrincipal != null)
            {
                var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                var uid = identity.CreateUserIdentity(user);
                uid.Save();
            }

            Util.EmailSender.Send(user, File.ReadAllText(MapPath("~/templates/ActivationEmail.xml")), BaseUrl);
        }

        private void ModifyUser()
        {
            var ip = IdentityProvider.Create(RegistryContext.Domain);
            ip.ModifyUser(user);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Create user object and load data if required

            if (RegistryUser != null)
            {
                user = RegistryUser;
            }
            else if (TemporaryPrincipal != null)
            {
                // Create a user object based on the temporary user generated
                // based on the OpenID etc. used at sign in.
                var identity = (GraywulfIdentity)TemporaryPrincipal.Identity;
                user = new Registry.User(identity.User);
                user.ParentReference.Value = RegistryContext.Domain;
                user.Context = RegistryContext;
            }
            else
            {
                // Must be under the current domain!
                user = new Registry.User(RegistryContext.Domain);
            }

            // Update form
            if (!IsPostBack)
            {
                UpdateForm();
            }
        }

        protected void ConfirmPasswordValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Make sure password and confirmation match
            args.IsValid = (Password.Text == ConfirmPassword.Text);
        }

        protected void DuplicateUsernameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RegistryUser == null)
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);
                args.IsValid = !ip.IsNameExisting(args.Value);
            }
        }

        protected void DuplicateEmailValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RegistryUser == null)
            {
                var ip = IdentityProvider.Create(RegistryContext.Domain);
                args.IsValid = !ip.IsEmailExisting(args.Value);
            }
        }

        protected void OK_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                SaveForm();

                if (user.Guid == Guid.Empty)
                {
                    CreateUser();

                    Response.Redirect(Activate.GetUrl(ReturnUrl));
                }
                else
                {
                    ModifyUser();

                    UserForm.Visible = false;
                    SuccessForm.Visible = true;
                    SuccessOK.Attributes.Add("onClick", Util.UrlFormatter.GetClientRedirect(ReturnUrl));
                }
            }
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.Auth.ChangePassword.GetUrl(ReturnUrl));
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ReturnUrl);
        }
    }
}