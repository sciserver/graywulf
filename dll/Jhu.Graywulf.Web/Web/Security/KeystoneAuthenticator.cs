using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Security
{
#if false
    public class KeystoneAuthenticator : IInteractiveAuthenticator
    {
        private string authorityName;
        private string authorityUri;
        private string displayName;

        /// <summary>
        /// Gets or sets the name of the authority
        /// </summary>
        [XmlElement]
        public string AuthorityName
        {
            get { return authorityName; }
            set { authorityName = value; }
        }

        /// <summary>
        /// Gets or sets the URI uniquely identifying the authority
        /// </summary>
        [XmlElement]
        public string AuthorityUri
        {
            get { return authorityUri; }
            set { authorityUri = value; }
        }

        /// <summary>
        /// Gets or sets the display name of the authority
        /// </summary>
        [XmlElement]
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        /// <summary>
        /// Gets the name of the authentication protocol.
        /// </summary>
        [XmlIgnore]
        public string Protocol
        {
            get { return Constants.ProtocolNameKeystone; }
        }

        public KeystoneAuthenticator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.authorityName = null;
            this.authorityUri = null;
            this.displayName = null;
        }

        public GraywulfPrincipal Authenticate()
        {
            /*
            var identityProvider = new CloudIdentityProvider(KeystoneSettings.Uri);
            var identity = new CloudIdentity()
            {
                //TenantName = ConfigurationManager.AppSettings["Keystone.AdminTenant"],
                Username = KeystoneSettings.AdminUser,
                Password = KeystoneSettings.AdminPassword,
            };

            UserAccess userAccess = identityProvider.ValidateToken(token, null, identity);
            
            bool isUser = userAccess.User.Roles.Any(item => item.Name == "user");
            
            if (!isUser)
            {
                throw new NotAuthorizedException("You do not have the permission to access CasJobs service.");
            }

            return userAccess;
            */

            throw new NotImplementedException();
        }
    }
#endif
}
