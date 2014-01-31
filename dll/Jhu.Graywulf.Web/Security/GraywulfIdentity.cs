using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// Implements an identity for Graywulf authentication schemes.
    /// </summary>
    public class GraywulfIdentity : IIdentity
    {
        private string protocol;
        private string authorityName;
        private string authorityUri;
        private string identifier;
        private bool isAuthenticated;
        private EntityProperty<User> userProperty;

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
        public EntityProperty<User> UserProperty
        {
            get { return userProperty; }
        }

        /// <summary>
        /// Gets the name of the registry user.
        /// </summary>
        public string Name
        {
            get
            {
                if (userProperty.IsEmpty)
                {
                    return null;
                }
                else
                {
                    return EntityFactory.GetName(userProperty.Name);
                }
            }
        }

        /// <summary>
        /// Gets or sets a reference to the registry user object.
        /// </summary>
        public User User
        {
            get { return userProperty.Value; }
            set { userProperty.Value = value; }
        }

        public GraywulfIdentity()
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
            this.userProperty = new EntityProperty<User>();
        }

        private void CopyMembers(GraywulfIdentity old)
        {
            this.protocol = old.protocol;
            this.authorityName = old.authorityName;
            this.authorityUri = old.authorityUri;
            this.identifier = old.identifier;
            this.userProperty = new EntityProperty<User>(old.userProperty);
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
        /// Loads the user from the graywulf registry
        /// </summary>
        /// <param name="identity"></param>
        /// <remarks>
        /// If the identity is not found, it will be marked as non-authenticated
        /// </remarks>
        public void LoadUser(Domain domain)
        {
            using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var uf = new UserFactory(registryContext);

                try
                {
                    switch (protocol)
                    {
                        case Constants.ProtocolNameForms:
                            userProperty.Value = uf.LoadUser(identifier);
                            break;
                        case Constants.ProtocolNameWindows:
                            // TODO: implement NTLM auth
                            throw new NotImplementedException();
                        default:
                            // All other cases use lookup by protocol name
                            userProperty.Value = uf.FindUserByIdentity(domain, protocol, authorityUri, identifier);
                            break;
                    }

                    IsAuthenticated = true;
                }
                catch (EntityNotFoundException)
                {
                    isAuthenticated = false;
                }
            }
        }

        /// <summary>
        /// Creates a new registry object based on the identity and the user supplied.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserIdentity CreateUserIdentity(User user)
        {
            return new UserIdentity(user)
            {
                Name = String.Format("{0}_{1}", AuthorityName, user.Name),
                Protocol = Protocol,
                Authority = AuthorityUri,
                Identifier = Identifier
            };
        }
    }
}
