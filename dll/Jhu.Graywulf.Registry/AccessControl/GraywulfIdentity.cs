using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.AccessControl
{
    /// <summary>
    /// Implements an identity for Graywulf authentication schemes that
    /// carries information about the identified used and the method of
    /// authentication.
    /// </summary>
    [Serializable]
    public class GraywulfIdentity : Jhu.Graywulf.AccessControl.Identity, IIdentity
    {
        #region Private member variables

        private string protocol;
        private string authorityName;
        private string authorityUri;
        private bool isMasterAuthority;
        private string identifier;
        private string sessionId;
        private object evidence;
        private EntityReference<User> userReference;

        #endregion
        #region Properties

        /// <summary>
        /// Gets the name of the authentication type
        /// </summary>
        /// <remarks>
        /// Required by the interface.
        /// </remarks>
        public override string AuthenticationType
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
        /// Gets or sets a string (GUID) that can be used to
        /// track the user session.
        /// </summary>
        public string SessionId
        {
            get { return sessionId; }
            set { sessionId = value; }
        }

        /// <summary>
        /// Gets or sets the value of the evidence used to
        /// authenticate the identity
        /// </summary>
        public object Evidence
        {
            get { return evidence; }
            set { evidence = value; }
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
        /// Gets the (non-qualified) name of the registry user.
        /// </summary>
        /// <remarks>
        /// Required by the IIdentity interface
        /// </remarks>
        public override string Name
        {
            get { return userReference.Value.Name; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets a reference to the registry user object.
        /// </summary>
        public User User
        {
            get { return userReference.Value; }
            set { userReference.Value = value; }
        }

        #endregion
        #region Constructors and initializers

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
            this.evidence = null;
            this.userReference = new EntityReference<User>(null);
        }

        private void CopyMembers(GraywulfIdentity old)
        {
            this.protocol = old.protocol;
            this.authorityName = old.authorityName;
            this.authorityUri = old.authorityUri;
            this.identifier = old.identifier;
            this.evidence = old.evidence;
            this.userReference = new EntityReference<User>(null, old.userReference);
        }

        #endregion

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
