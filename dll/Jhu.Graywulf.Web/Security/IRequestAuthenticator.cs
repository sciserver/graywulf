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
    public interface IRequestAuthenticator
    {
        string Protocol { get; }

        string AuthorityName { get; }

        string AuthorityUri { get; set; }

        string DisplayName { get; set; }

        GraywulfPrincipal Authenticate();
    }
}
