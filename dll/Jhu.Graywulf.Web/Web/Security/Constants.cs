using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Security
{
    static class Constants
    {
        public const string AuthenticationTypeName = "Graywulf";

        public const string AuthorityNameGraywulf = "Graywulf";
        public const string AuthorityNameKeystone = "Keystone";

        public const string ProtocolNameForms = "Forms";
        public const string ProtocolNameWindows = "Windows";
        public const string ProtocolNameOpenID = "OpenID";
        public const string ProtocolNameOAuth = "OAuth";
        public const string ProtocolNameOAuth2 = "OAuth2";
        public const string ProtocolNameKeystone = "Keystone";

        public const string UserRole = "User";

        public const string SettingsAuthenticators = "Authenticators";
        public const string SettingsKeystone = "KeystoneIdentityProviderSettings";

        public const string KeystoneDefaultUri = "http://localhost:5000/";
        public const string KeystoneDefaultDomain= "default";
        public const string KeystoneDefaultAuthTokenParameter = "token";        // TODO
        public const string KeystoneDefaultAuthTokenHeader = "X-Subject-Token"; // TODO

        public const string HttpContextAuthenticationResponse = "Jhu.Graywulf.Web.Security.AuthenticationResponse";
    }
}
