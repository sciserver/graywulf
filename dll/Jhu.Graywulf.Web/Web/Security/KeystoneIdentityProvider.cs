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

        private KeystoneSettings settings;

        #endregion
        #region Properties

        public KeystoneSettings Settings
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
                return new Keystone.KeystoneClient(settings.AuthorityUri)
                {
                    AdminAuthToken = settings.AdminToken,
                };
            }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneIdentityProvider(Domain domain)
            : base(domain)
        {
            InitializeMembers();

            settings = (KeystoneSettings)domain.Settings[Constants.SettingsKeystone].Value;
        }

        private void InitializeMembers()
        {
            this.settings = new KeystoneSettings();
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
                DomainID = settings.Domain,
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
                    // TODO: we might add logic here, to create user in keystone
                    throw new IdentityProviderException("No matching identity found."); // TODO
                }
            }

            return keystoneUser;
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

        private Keystone.User GetKeystoneUser(string username)
        {
            // Try to get user from Keystone
            var users = KeystoneClient.FindUsers(settings.Domain, username, false, false);

            if (users == null)
            {
                return null;
            }
            else if (users.Length != 1)
            {
                // This shouldn't happen but let's throw an exception to
                // make sure nothing goes wrong
                throw new InvalidOperationException();
            }
            else
            {
                return users[0];
            }
        }

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
        public override User GetUserByUserName(string username)
        {
            var user = GetKeystoneUser(username);

            if (user == null)
            {
                return null;
            }
            else
            {
                // Try to get user from the registry. If not found, return null       
                return base.GetUserByUserName(username);
            }
        }

        protected override void OnCreateUser(User user)
        {
            // Create user in keystone
            var keystoneUser = KeystoneClient.Create(ConvertUser(user));

            // Create user locally
            base.OnCreateUser(user);

            // Grant user a role on the default project. This is necessary to
            // gain access to services like swift.
            // Project has no equivalent in graywulf (because users are not associated
            // with federations but only with domains). Hence, project name is simply
            // taken from settings.
            // Roles are taken from graywulf user groups.
            var project = KeystoneClient.FindProjects(settings.Domain, settings.Project, true, false)[0];

            foreach (var userGroup in user.UserGroupMemberships.Values)
            {
                var role = KeystoneClient.FindRoles(settings.Domain, userGroup.Name, true, false)[0];
                KeystoneClient.GrantRole(project, keystoneUser, role);
            }

            // Add identity to local user
            var principal = settings.CreateAuthenticatedPrincipal(keystoneUser, true);
            AddUserIdentity(user, principal.Identity);
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
            var users = KeystoneClient.FindUsers(settings.Domain, username, false, false);

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

        public override AuthenticationResponse VerifyPassword(AuthenticationRequest request)
        {
            // User needs to be authenticated in the scope of a project
            var project = new Keystone.Project()
            {
                Domain = new Keystone.Domain()
                {
                    Name = settings.Domain
                },
                Name = settings.Project,
            };

            // Verify user password in Keystone, we don't use
            // Graywulf password in this case
            var token = KeystoneClient.Authenticate(settings.Domain, request.Username, request.Password, project);

            // Find user details in keystone
            token.User = GetKeystoneUser(request.Username);

            // Create a response, this sets necessary response headers
            var response = new AuthenticationResponse(request);
            settings.UpdateAuthenticationResponse(response, token, true);
            
            // Load user from the graywulf registry. This call will create the user
            // if necessary because authority is set to master
            LoadOrCreateUser(response.Principal.Identity);
            
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
