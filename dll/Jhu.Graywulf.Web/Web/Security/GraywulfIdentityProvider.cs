using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public class GraywulfIdentityProvider : IdentityProvider
    {
        #region Constructors and initializers

        public GraywulfIdentityProvider(Context context)
            : base(context)
        {

        }

        #endregion
        #region User account manipulation

        public override User GetUser(string username)
        {
            var uf = new UserFactory(Context);
            var user = uf.FindUserByName(Context.Domain, username);

            return user;
        }

        public override User GetUserByEmail(string email)
        {
            var uf = new UserFactory(Context);
            var user = uf.FindUserByEmail(Context.Domain, email);

            return user;
        }

        public override void CreateUser(User user)
        {
            user.Context = Context;
            user.Save();

            user.MakeMemberOf(Context.Domain.StandardUserGroup.Guid);
        }

        public override void ModifyUser(User user)
        {
            user.Save();
        }

        public override void DeleteUser(User user)
        {
            user.Context = Context;
            user.Delete();
        }

        public override bool IsNameExisting(string username)
        {
            var ef = new EntityFactory(Context);
            return ef.CheckEntityDuplicate(EntityType.User, Guid.Empty, Context.Domain.Guid, username);
        }

        public override bool IsEmailExisting(string email)
        {
            var uf = new UserFactory(Context);
            return uf.CheckEmailDuplicate(Context.Domain, email);
        }

        #endregion
        #region User activation

        public override bool IsUserActive(Jhu.Graywulf.Registry.User user)
        {
            return user.DeploymentState == DeploymentState.Deployed;
        }

        public override User GetUserByActivationCode(string activationCode)
        {
            var uf = new UserFactory(Context);
            var user = uf.FindUserByActivationCode(Context.Domain, activationCode);

            if (user == null)
            {
                throw new IdentityProviderException(ExceptionMessages.AccessDenied);
            }

            return user;
        }

        public override void ActivateUser(User user)
        {
            user.Context = Context;
            user.ActivationCode = string.Empty;
            user.DeploymentState = DeploymentState.Deployed;
            user.Save();
        }

        public override void DeactivateUser(User user)
        {
            user.Context = Context;
            user.GenerateActivationCode();
            user.DeploymentState = DeploymentState.Undeployed;
            user.Save();
        }

        #endregion
        #region Password functions

        public override AuthenticationResponse VerifyPassword(string username, string password, bool createPersistentCookie)
        {
            try
            {
                var uf = new UserFactory(Context);
                var user = uf.LoginUser(Context.Domain, username, password);

                // Create a response with no headers set because
                // forms authentication will set the cookie

                return CreateAuthenticationResponse(user);
            }
            catch (Exception ex)
            {
                throw new IdentityProviderException(ExceptionMessages.AccessDenied, ex);
            }
        }

        public override void ChangePassword(User user, string oldPassword, string newPassword)
        {
            VerifyPassword(user.Name, oldPassword, false);
            ResetPassword(user, newPassword);
        }

        public override void ResetPassword(User user, string newPassword)
        {
            user.SetPassword(newPassword);

            user.ActivationCode = String.Empty;
            user.Save();
        }

        #endregion
        #region User group membership

        public override void MakeMemberOf(User user, UserGroup group)
        {
            // TODO: implement this if necessary
            throw new NotImplementedException();
        }

        public override void RevokeMemberOf(User user, UserGroup group)
        {
            // TODO: implement this if necessary
            throw new NotImplementedException();
        }

        #endregion
    }
}
