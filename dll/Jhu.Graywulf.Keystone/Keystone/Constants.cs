using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Keystone
{
    internal static class Constants
    {
        /// <summary>
        /// Header name for passing token for authentication
        /// </summary>
        public const string KeystoneXAuthTokenHeader = "X-Auth-Token";

        /// <summary>
        /// Header name for passing token subject to manipulation
        /// </summary>
        public const string KeystoneXSubjectTokenHeader = "X-Subject-Token";
    }
}
