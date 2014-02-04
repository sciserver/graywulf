using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// When implemented in derived classes, perfoms authentication
    /// that requires user interaction
    /// </summary>
    public interface IInteractiveAuthenticator
    {
        string Protocol { get; }
        string AuthorityName { get; set;  }
        string AuthorityUri { get; set; }
        string DisplayName { get; set; }
        GraywulfPrincipal Authenticate();
        void RedirectToLoginPage();
    }
}
