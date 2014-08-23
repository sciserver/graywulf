using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Jhu.Graywulf.Keystone;

namespace Jhu.Graywulf.Web.Security
{
    public class KeystoneSettings
    {
        #region Private member variables

        private string authorityName;
        private Uri authorityUri;
        private string domain;
        private string project;
        private string adminToken;
        private string adminProject;
        private string adminUserName;
        private string adminPassword;
        private string authTokenParameter;
        private string authTokenHeader;
        private string authTokenCookie;

        private DateTime adminTokenExpiresAt;

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
        public Uri AuthorityUri
        {
            get { return authorityUri; }
            set { authorityUri = value; }
        }

        /// <summary>
        /// Gets or sets the Keystone domain associated with the
        /// Graywulf domain
        /// </summary>
        [DataMember]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <summary>
        /// Gets or sets the project (tenant) users are associated with.
        /// </summary>
        public string Project
        {
            get { return project; }
            set { project = value; }
        }

        /// <summary>
        /// Gets or sets the token identifying the administrator
        /// of the Keystone service.
        /// </summary>
        [DataMember]
        public string AdminToken
        {
            get { return adminToken; }
            set { adminToken = value; }
        }

        /// <summary>
        /// Gets or sets the project (tenant) name of
        /// the administrator.
        /// </summary>
        public string AdminProject
        {
            get { return adminProject; }
            set { adminProject = value; }
        }

        /// <summary>
        /// Gets or sets the user name of the identity used to
        /// manage the keystone service.
        /// </summary>
        public string AdminUserName
        {
            get { return adminUserName; }
            set { adminUserName = value; }
        }

        /// <summary>
        /// Gets or sets the password of the identity used to
        /// manage the keystone instance.
        /// </summary>
        public string AdminPassword
        {
            get { return adminPassword; }
            set { adminPassword = value; }
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
            get { return authTokenCookie; }
            set { authTokenCookie = value; }
        }

        [DataMember]
        public string AuthTokenCookie
        {
            get { return authTokenCookie; }
            set { authTokenCookie = value; }
        }

        #endregion
        #region Constructors and initializers

        public KeystoneSettings()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.authorityName = Constants.AuthorityNameKeystone;
            this.authorityUri = new Uri(Constants.KeystoneDefaultUri);
            this.domain = Constants.KeystoneDefaultDomain;
            this.adminToken = null;
            this.adminProject = null;
            this.adminUserName = null;
            this.adminPassword = null;
            this.authTokenParameter = Constants.KeystoneDefaultAuthTokenParameter;
            this.authTokenHeader = Constants.KeystoneDefaultAuthTokenHeader;
            this.authTokenCookie = Constants.KeystoneDefaultAuthTokenCookie;

            this.adminTokenExpiresAt = DateTime.MinValue;
        }

        #endregion

        public KeystoneClient CreateClient()
        {
            var ksclient = new KeystoneClient(AuthorityUri);

            // If using password authentication, make sure we have a valid admin token
            // Leave e 30 second margin to perform all keystone-related operations with an
            // already existing token
            if (!String.IsNullOrWhiteSpace(adminPassword) && (DateTime.Now - adminTokenExpiresAt).TotalSeconds > -30)
            {
                lock (this)
                {
                    var project = new Keystone.Project()
                    {
                        Domain = new Keystone.Domain()
                        {
                            Name = domain
                        },
                        Name = adminProject
                    };

                    var token = ksclient.Authenticate(domain, adminUserName, adminPassword, project);

                    adminToken = token.ID;
                    adminTokenExpiresAt = token.ExpiresAt;
                }
            }

            // Set the valid admin token
            ksclient.AdminAuthToken = adminToken;

            return ksclient;
        }

        public GraywulfPrincipal CreateAuthenticatedPrincipal(Keystone.User user, bool isMasterAuthority)
        {
            // TODO: role logic might be added here

            var identity = new GraywulfIdentity()
            {
                Protocol = Constants.ProtocolNameKeystone,
                AuthorityName = authorityName,
                AuthorityUri = authorityUri.ToString(),
                Identifier = user.ID,
                IsAuthenticated = true,
                IsMasterAuthority = isMasterAuthority,
            };

            // Accept users without the following parameters set but
            // this is not a good practice in general to leave them null 
            // in Keystone
            identity.User = new Registry.User()
            {
                Name = user.Name,
                Comments = user.Description ?? String.Empty,
                Email = user.Email ?? String.Empty,
                DeploymentState = user.Enabled.Value ? Registry.DeploymentState.Deployed : Registry.DeploymentState.Undeployed,
            };

            return new GraywulfPrincipal(identity);
        }

        public void UpdateAuthenticationResponse(AuthenticationResponse response, Token token, bool isMasterAuthority)
        {
            if (response.Principal == null)
            {
                var principal = CreateAuthenticatedPrincipal(token.User, isMasterAuthority);
                response.SetPrincipal(principal);
            }

            // TODO: add keystone cookie, custom parameter, etc...
            response.QueryParameters.Add(authTokenParameter, token.ID);
            response.Headers.Add(authTokenHeader, token.ID);

            // TODO: not sure if it is needed or created automatically
            //response.Cookies.Add(CreateFormsAuthenticationTicketCookie(principal.Identity.User, createPersistentCookie));
        }
    }
}
