using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    public abstract class AuthenticatorBase
    {
        public abstract string Protocol { get; }

        public abstract string Authority { get; set; }

        public abstract string DisplayName { get; set; }

        public abstract bool IsInteractive { get; }

        public abstract GraywulfPrincipal Authenticate();

        public abstract void RedirectToLoginPage();
    }
}
