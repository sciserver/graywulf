using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements an identity for Graywulf authentication schemes.
    /// </summary>
    [Serializable]
    public class GraywulfIdentity : IIdentity
    {
        private string protocol;
        private string authorityName;
        private string authorityUri;
        private bool isMasterAuthority;
        private string identifier;
        private bool isAuthenticated;
        private EntityReference<User> userReference;

        /// <summary>
        /// Gets the name of the authentication type
        /// </summary>
        /// <remarks>
        /// Required by the interface.
        /// </remarks>
        public string AuthenticationType
        {
            get { return Constants.AuthenticationTypeName; }
        }

        /// <summary>
        /// Gets or sets name of the protocol this identity
        /// is authenticated with.
        /// </summary>
        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        /// <summary>
        /// Gets or sets the name of the authority this identity
        /// was authenticated by.
        /// </summary>
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets an URI identifying the authority this
        /// identity was authenticated by.
        /// </summary>
        public string AuthorityUri
        {
            get { return authorityUri; }
            set { authorityUri = value; }
        }

        /// <summary>
        /// Gets whether the user was identified by a master authority.
        /// </summary>
        public bool IsMasterAuthority
        {
            get { return isMasterAuthority; }
            internal set { isMasterAuthority = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of this identity,
        /// as defined by the authority.
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        /// <summary>
        /// Gets or sets whether this identity has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            internal set { isAuthenticated = value; }
        }

        /// <summary>
        /// Gets or sets an object holding a reference to
        /// the registry user object.
        /// </summary>
        public EntityReference<User> UserReference
        {
            get { return userReference; }
        }

        /// <summary>
        /// Gets the name of the registry user.
        /// </summary>
        /// <remarks>
        /// Required by the IIdentity
        /// </remarks>
        public string Name
        {
            get { return userReference.Value.Name; }
        }

        /// <summary>
        /// Gets or sets a reference to the registry user object.
        /// </summary>
        public User User
        {
            get { return userReference.Value; }
            set { userReference.Value = value; }
        }

        internal GraywulfIdentity()
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfIdentity(GraywulfIdentity old)
        {
            CopyMembers(old);
        }

        [OnSerializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.protocol = String.Empty;
            this.authorityName = String.Empty;
            this.authorityUri = String.Empty;
            this.identifier = String.Empty;
            this.isAuthenticated = false;
            this.userReference = new EntityReference<User>(null);
        }

        private void CopyMembers(GraywulfIdentity old)
        {
            this.protocol = old.protocol;
            this.authorityName = old.authorityName;
            this.authorityUri = old.authorityUri;
            this.identifier = old.identifier;
            this.isAuthenticated = old.isAuthenticated;
            this.userReference = new EntityReference<User>(null, old.userReference);
        }

        /// <summary>
        /// Compares two identities by protocol, authority and identifier.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CompareByIdentifier(GraywulfIdentity other)
        {
            return
                StringComparer.InvariantCultureIgnoreCase.Compare(this.protocol, other.protocol) == 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(this.authorityUri, other.authorityUri) == 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(this.identifier, other.identifier) == 0;
        }

        /// <summary>
        /// Loads user from the Graywulf registry based on the identity
        /// </summary>
        public void LoadUser()
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

            using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                User user = null;

                if (this.Protocol == Constants.ProtocolNameForms)
                {
                    user = LoadUserByName(registryContext, this.Identifier);
                }
                else if (this.IsMasterAuthority)
                {
                    // If the user ahs been authenticated by a master authority
                    // we try to find it first, then we create a new user if
                    // necessary

                    user = LoadUserByIdentity(registryContext);

                    if (user == null)
                    {
                        // No user found by identity, try to load by name
                        user = LoadUserByName(registryContext);

                        // If the user is still not found, create an entirely new user
                        if (user == null)
                        {
                            user = CreateUser(registryContext);
                        }

                        // Now we have a user but no identifier is associated with it
                        // Simply create a new identity in the Graywulf registry

                        var uid = CreateUserIdentity(user);
                        uid.Save();
                    }
                }
                else
                {
                    // If the user was authenticated by a non-master authority
                    // we simply look it up by identifier

                    user = LoadUserByIdentity(registryContext);
                }

                if (user == null)
                {
                    throw new SecurityException(ExceptionMessages.AccessDenied);
                }

                this.isAuthenticated = true;
                this.userReference.Value = user;

                // Cache name for later
                this.userReference.Value.GetFullyQualifiedName();
            }
        }

        private User LoadUserByName(Context registryContext)
        {
            // Use name from the temporary user created by the authenticator
            var name = this.User.Name;

            // Prefix name with domain name
            name = EntityFactory.CombineName(EntityType.User, registryContext.Domain.GetFullyQualifiedName(), name);

            // Try to load user
            return LoadUserByName(registryContext, name);
        }

        private User LoadUserByName(Context registryContext, string name)
        {
            var ef = new EntityFactory(registryContext);

            try
            {
                return ef.LoadEntity<User>(name);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        private User LoadUserByIdentity(Context registryContext)
        {
            var uf = new UserFactory(registryContext);

            try
            {
                return uf.FindUserByIdentity(
                    registryContext.Domain,
                    this.Protocol,
                    this.AuthorityUri,
                    this.Identifier);
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
        }

        private User CreateUser(Context registryContext)
        {
            // TODO: this needs testing here
            var user = this.User;

            user.Context = registryContext;
            user.ParentReference.Value = registryContext.Domain;
            user.Save();

            return user;
        }

        /// <summary>
        /// Creates a new registry object based on the identity and the user supplied.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserIdentity CreateUserIdentity(User user)
        {
            var uid = new UserIdentity(user)
            {
                Name = String.Format("{0}_{1}", this.AuthorityName, user.Name),
                Protocol = this.Protocol,
                Authority = this.AuthorityUri,
                Identifier = this.Identifier
            };

            return uid;
        }
    }
}
