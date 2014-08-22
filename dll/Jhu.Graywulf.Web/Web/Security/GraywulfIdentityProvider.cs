using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
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

        public GraywulfIdentityProvider(Domain domain)
            : base(domain.Context)
        {
        }

        #endregion
        #region User account manipulation

        /// <summary>
        /// Loads user from the Graywulf registry based on the identity. If the
        /// user is marked as authenticated by a master authority and the graywulf
        /// user doesn't exists, this function creates it automatically.
        /// </summary>
        public User LoadOrCreateUser(GraywulfIdentity identity)
        {
            // If the user is authenticated by a generic authority then we should
            // be able to find it by a registered identity in the Graywulf registry.
            // If the authenticator, on the other hand, is a master authority,
            // that case we either load the user by name or by identity. If the user
            // is not found in the registry then we create it.

            // The only special case is when the user is authenticated by an ASP.NET
            // forms authentication ticket. In this case we trust the user name within
            // the ticket (as it was previously issued by us) so we simply load the
            // user by name

            User user = null;

            if (identity.Protocol == Constants.ProtocolNameForms)
            {
                user = GetUser(identity.Identifier);
            }
            else if (identity.IsMasterAuthority)
            {
                // If the user ahs been authenticated by a master authority
                // we try to find it first, then we create a new user if
                // necessary

                user = GetUserByIdentity(identity);

                if (user == null)
                {
                    // No user found by identity, try to load by name
                    user = GetUserByUserName(identity.User.Name);

                    // If the user is still not found, create an entirely new user
                    if (user == null)
                    {
                        CreateUserInternal(identity.User);
                        user = identity.User;
                    }

                    // Now we have a user but no identifier is associated with it
                    // Simply create a new identity in the Graywulf registry
                    var uid = AddUserIdentity(user, identity);
                }
            }
            else
            {
                // If the user was authenticated by a non-master authority
                // we simply look it up by identifier

                user = GetUserByIdentity(identity);
            }

            if (user == null)
            {
                throw new SecurityException(ExceptionMessages.AccessDenied);
            }

            identity.IsAuthenticated = true;
            identity.UserReference.Value = user;

            // Cache name for later
            identity.UserReference.Value.GetFullyQualifiedName();

            return user;
        }

        public User GetUser(string fullyQualifiedName)
        {
            var ef = new EntityFactory(Context);

            try
            {
                return ef.LoadEntity<User>(fullyQualifiedName);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        public override User GetUserByUserName(string username)
        {
            var uf = new UserFactory(Context);

            try
            {
                return uf.FindUserByName(Context.Domain, username);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        public override User GetUserByEmail(string email)
        {
            var uf = new UserFactory(Context);

            try
            {
                return uf.FindUserByEmail(Context.Domain, email);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        private User GetUserByIdentity(GraywulfIdentity identity)
        {
            var uf = new UserFactory(Context);

            try
            {
                return uf.FindUserByIdentity(
                    Context.Domain,
                    identity.Protocol,
                    identity.AuthorityUri,
                    identity.Identifier);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        protected override void OnCreateUser(User user)
        {
            CreateUserInternal(user);
        }

        private void CreateUserInternal(User user)
        {
            user.Context = Context;
            user.ParentReference.Value = Context.Domain;

            user.Save();

            user.AddToGroup(Context.Domain.StandardUserGroup.Guid);
        }

        /// <summary>
        /// Associates a new identity with a user
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public override UserIdentity AddUserIdentity(User user, GraywulfIdentity identity)
        {
            identity.User = user;

            var uid = new UserIdentity(user)
            {
                Name = String.Format("{0}_{1}", identity.AuthorityName, identity.Name),
                Protocol = identity.Protocol,
                Authority = identity.AuthorityUri,
                Identifier = identity.Identifier
            };

            uid.Save();

            user.LoadUserIdentities(true);

            return uid;
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
            user.Activate();
            user.Save();
        }

        public override void DeactivateUser(User user)
        {
            user.Context = Context;
            user.Deactivate();
            user.Save();
        }

        #endregion
        #region Password functions

        public override AuthenticationResponse VerifyPassword(AuthenticationRequest request)
        {
            try
            {
                var uf = new UserFactory(Context);
                var user = uf.LoginUser(Context.Domain, request.Username, request.Password);
                
                return CreateAuthenticationResponse(request, user);
            }
            catch (EntityNotFoundException ex)
            {
                throw new IdentityProviderException(ExceptionMessages.AccessDenied, ex);
            }
        }

        private GraywulfPrincipal CreateAuthenticatedPrincipal(User user)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameForms,
                Identifier = user.GetFullyQualifiedName(),
                IsAuthenticated = true,
                IsMasterAuthority = true,
                User = user,
            };

            return new GraywulfPrincipal(identity);
        }

        private AuthenticationResponse CreateAuthenticationResponse(AuthenticationRequest request, User user)
        {
            var response = new AuthenticationResponse(request);
            var principal = CreateAuthenticatedPrincipal(user);

            response.SetPrincipal(principal);

            return response;
        }

        public override void ChangePassword(User user, string oldPassword, string newPassword)
        {
            var request = new AuthenticationRequest(user.Name, oldPassword);
            VerifyPassword(request);
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
