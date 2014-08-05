using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Security
{
    public class KeystoneIdentityProviderSettings
    {
        #region Private member variables

        private string authorityName;
        private Uri keystoneBaseUri;
        private string keystoneAdminToken;
        private string keystoneDomainID;
        private string authTokenParameter;
        private string authTokenHeader;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the name of the authority providing
        /// the Keystone service
        /// </summary>
        [DataMember]
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the base URL of the Keystone service
        /// </summary>
        [DataMember]
        public Uri KeystoneBaseUri
        {
            get { return keystoneBaseUri; }
            set { keystoneBaseUri = value; }
        }

        /// <summary>
        /// Gets or sets the token identifying the administrator
        /// of the Keystone service.
        /// </summary>
        [DataMember]
        public string KeystoneAdminToken
        {
            get { return keystoneAdminToken; }
            set { keystoneAdminToken = value; }
        }

        /// <summary>
        /// Gets or sets the Keystone domain associated with the
        /// Graywulf domain
        /// </summary>
        [DataMember]
        public string KeystoneDomainID
        {
            get { return keystoneDomainID; }
            set { keystoneDomainID = value; }
        }

        [DataMember]
        public string AuthTokenParameter
        {
            get { return authTokenParameter; }
            set { authTokenParameter = value; }
        }

        [DataMember]
        public string AuthTokenHeader
        {
            get { return authTokenHeader; }
            set { authTokenHeader = value; }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneIdentityProviderSettings()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityName = Constants.AuthorityNameKeystone;
            this.keystoneBaseUri = new Uri(Constants.KeystoneDefaultUri);
            this.keystoneAdminToken = null;
            this.authTokenParameter = Constants.KeystoneDefaultAuthTokenParameter;
            this.authTokenHeader = Constants.KeystoneDefaultAuthTokenHeader;
        }

        #endregion
    }
}
