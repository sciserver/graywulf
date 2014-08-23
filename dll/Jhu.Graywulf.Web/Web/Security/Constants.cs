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

        public const string ProtocolNameForms = "Forms";
        public const string ProtocolNameWindows = "Windows";

        public const string UserRole = "User";

        public const string SettingsAuthenticators = "Authenticators";

        public const string HttpContextAuthenticationResponse = "Jhu.Graywulf.Web.Security.AuthenticationResponse";
    }
}
