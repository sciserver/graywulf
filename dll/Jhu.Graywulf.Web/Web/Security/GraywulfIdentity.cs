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
            set { isMasterAuthority = value; }
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
            set { isAuthenticated = value; }
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
    }
}
