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

        private string authorityName;
        private Uri keystoneBaseUri;
        private string keystoneAdminToken;
        private string keystoneDomainID;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the name of the authority providing
        /// the Keystone service
        /// </summary>
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the base URL of the Keystone service
        /// </summary>
        public Uri KeystoneBaseUri
        {
            get { return keystoneBaseUri; }
            set { keystoneBaseUri = value; }
        }

        /// <summary>
        /// Gets or sets the token identifying the administrator
        /// of the Keystone service.
        /// </summary>
        public string KeystoneAdminToken
        {
            get { return keystoneAdminToken; }
            set { keystoneAdminToken = value; }
        }

        /// <summary>
        /// Gets or sets the Keystone domain associated with the
        /// Graywulf domain
        /// </summary>
        public string KeystoneDomainID
        {
            get { return keystoneDomainID; }
            set { keystoneDomainID = value; }
        }

        /// <summary>
        /// Gets a configures Keystone client class.
        /// </summary>
        internal Jhu.Graywulf.Keystone.KeystoneClient KeystoneClient
        {
            get
            {
                return new Keystone.KeystoneClient(keystoneBaseUri)
                {
                    AdminAuthToken = this.keystoneAdminToken,
                };
            }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneIdentityProvider(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityName = Constants.KeystoneDefaultAuthorityName;
            this.keystoneBaseUri = new Uri(Constants.KeystoneDefaultUri);
            this.keystoneAdminToken = null;
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
                DomainID = keystoneDomainID,
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
            return String.Format("{0}_{1}", authorityName, user.Name);
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
            var users = KeystoneClient.FindUsers(keystoneDomainID, username, false, false);

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
                Authority = keystoneBaseUri.ToString(),
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
            var users = KeystoneClient.FindUsers(keystoneDomainID, username, false, false);

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

        public override User VerifyPassword(string username, string password)
        {
            // Verify user password in Keystone, we don't use
            // Graywulf password in this case

            var token = KeystoneClient.Authenticate(keystoneDomainID, username, password);

            return base.GetUser(token.User.Name);
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
