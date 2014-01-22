using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    /// <summary>
    /// When implemented in derived classes, performs per request
    /// authentication based on form data and cookies.
    /// </summary>
    public abstract class RequestAuthenticatorBase : IAuthenticator
    {
        public abstract string Protocol { get; }

        public abstract string Authority { get; set; }

        public abstract string DisplayName { get; set; }

        public abstract GraywulfPrincipal Authenticate();
    }
}
