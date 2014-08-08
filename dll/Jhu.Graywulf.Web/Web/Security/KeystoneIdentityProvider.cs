using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements function to act as identity provider based on
    /// user data stored in a keystone instance. It created shadow
    /// users in the Graywulf registry
    /// </summary>
    public class KeystoneIdentityProvider : GraywulfIdentityProvider
    {
        #region Private member variables

        private KeystoneIdentityProviderSettings settings;

        #endregion
        #region Properties

        public KeystoneIdentityProviderSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Gets a configures Keystone client class.
        /// </summary>
        internal Jhu.Graywulf.Keystone.KeystoneClient KeystoneClient
        {
            get
            {
                return new Keystone.KeystoneClient(settings.KeystoneBaseUri)
                {
                    AdminAuthToken = settings.KeystoneAdminToken,
                };
            }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneIdentityProvider(Domain domain)
            : base(domain)
        {
            InitializeMembers();

            settings = (KeystoneIdentityProviderSettings)domain.Settings[Constants.SettingsKeystone].Value;
        }

        private void InitializeMembers()
        {
            this.settings = new KeystoneIdentityProviderSettings();
        }

        #endregion

        /// <summary>
        /// Converts a Graywulf user to a matching Keystone user
        /// </summary>
        /// <param name="graywulfUser"></param>
        /// <returns></returns>
        private Jhu.Graywulf.Keystone.User ConvertUser(User graywulfUser)
        {
            var keystoneUser = new Keystone.User()
            {
                Name = graywulfUser.Name,
                DomainID = settings.KeystoneDomainID,
                Description = graywulfUser.Comments,
                Email = graywulfUser.Email,
                Enabled = graywulfUser.DeploymentState == DeploymentState.Deployed,
            };

            if (graywulfUser.IsExisting)
            {
                graywulfUser.LoadUserIdentities(false);

                var idname = GetIdentityName(graywulfUser);
                if (graywulfUser.UserIdentities.ContainsKey(idname))
                {
                    var uid = graywulfUser.UserIdentities[idname];
                    var idx = uid.Identifier.LastIndexOf("/");
                    keystoneUser.ID = uid.Identifier.Substring(idx + 1);
                }
                else
                {
                    throw new IdentityProviderException("No matching identity found."); // TODO
                }
            }

            return keystoneUser;
        }

        /// <summary>
        /// Converts a Keystone user to a matching Graywulf user
        /// </summary>
        /// <param name="keystoneUser"></param>
        /// <returns></returns>
        private User ConvertUser(Jhu.Graywulf.Keystone.User keystoneUser)
        {
            return new User(Context)
            {
                Name = keystoneUser.Name,
                Comments = keystoneUser.Description,
                Email = keystoneUser.Email,
                DeploymentState = keystoneUser.Enabled.Value ? DeploymentState.Deployed : DeploymentState.Undeployed
            };
        }

        /// <summary>
        /// Returns a generated name for the UserIdentity entry
        /// in the Graywulf registry.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GetIdentityName(User user)
        {
            return String.Format("{0}_{1}", settings.AuthorityName, user.Name);
        }

        #region User account manipulation

        /// <summary>
        /// Returns a user looked up by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <remarks>
        /// If a user exists in the Keystone registry but not in Graywulf,
        /// a Graywulf user is returned initialized to values from Keystone
        /// but not saved automatically to the Graywulf registry.
        /// </remarks>
        public override User GetUser(string username)
        {
            // Try to get user from Keystone
            var users = KeystoneClient.FindUsers(settings.KeystoneDomainID, username, false, false);

            if (users == null || users.Length != 1)
            {
                return null;
            }
            else
            {
                try
                {
                    // Try to get the matching user from the Graywulf repository
                    return base.GetUser(username);
                }
                catch (EntityNotFoundException)
                {
                    // Create a non-saved user from keystone user
                    return ConvertUser(users[0]);
                }
            }
        }

        public override void CreateUser(User user)
        {
            // Create user in keystone
            var keystoneUser = KeystoneClient.Create(ConvertUser(user));

            // Create local shadow
            base.CreateUser(user);

            // TODO: add logic to create project/tenant in keystone here if necessary

            // Create identity refering to the keystone id
            var id = new UserIdentity(user)
            {
                Name = GetIdentityName(user),
                Protocol = Constants.ProtocolNameKeystone,
                Authority = settings.KeystoneBaseUri.ToString(),
                Identifier = keystoneUser.Links.Self
            };

            id.Save();
        }

        public override void ModifyUser(User user)
        {
            // Modify local shadow
            base.ModifyUser(user);

            // Update user in keystone
            KeystoneClient.Update(ConvertUser(user));
        }

        public override void DeleteUser(User user)
        {
            // Delete local shadow
            base.DeleteUser(user);

            // Delete user from keystone
            KeystoneClient.Delete(ConvertUser(user));
        }

        public override bool IsNameExisting(string username)
        {
            // Try to get user from Keystone
            var users = KeystoneClient.FindUsers(settings.KeystoneDomainID, username, false, false);

            return users != null && users.Length > 0;
        }

        #endregion
        #region User activation

        public override bool IsUserActive(User user)
        {
            var keystoneUser = ConvertUser(user);
            keystoneUser = KeystoneClient.GetUser(keystoneUser.ID);

            return keystoneUser.Enabled.Value;
        }

        public override void ActivateUser(User user)
        {
            base.ActivateUser(user);

            var keystoneUser = ConvertUser(user);
            keystoneUser.Enabled = true;
            KeystoneClient.Update(keystoneUser);
        }

        public override void DeactivateUser(User user)
        {
            base.DeactivateUser(user);

            var keystoneUser = ConvertUser(user);
            keystoneUser.Enabled = false;
            KeystoneClient.Update(keystoneUser);
        }

        #endregion
        #region Password functions

        public override AuthenticationResponse VerifyPassword(string username, string password, bool createPersistentCookie)
        {
            // Verify user password in Keystone, we don't use
            // Graywulf password in this case

            var token = KeystoneClient.Authenticate(settings.KeystoneDomainID, username, password);
            var user = GetUser(token.User.Name);

            // Create a response and set necessary headers

            var response = CreateAuthenticationResponse(user);

            response.QueryParameters.Add(settings.AuthTokenParameter, token.ID);
            response.Headers.Add(settings.AuthTokenHeader, token.ID);
            response.Cookies.Add(CreateFormsAuthenticationTicketCookie(user, createPersistentCookie));

            return response;
        }

        public override void ChangePassword(User user, string oldPassword, string newPassword)
        {
            var keystoneUser = ConvertUser(user);

            KeystoneClient.ChangePassword(keystoneUser.ID, oldPassword, newPassword);
        }

        public override void ResetPassword(User user, string newPassword)
        {
            var keystoneUser = ConvertUser(user);

            KeystoneClient.ResetPassword(keystoneUser.ID, newPassword);
        }

        #endregion

    }
}
