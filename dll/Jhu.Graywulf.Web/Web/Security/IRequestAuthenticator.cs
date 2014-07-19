using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// When implemented in derived classes, performs per request
    /// authentication based on form data and cookies.
    /// </summary>
    public interface IRequestAuthenticator
    {
        /// <summary>
        /// Gets the name of the authentication protocol.
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Gets the name of the authentication authority.
        /// </summary>
        string AuthorityName { get; }

        /// <summary>
        /// Gets or sets the URL of the authority.
        /// </summary>
        string AuthorityUri { get; set; }

        /// <summary>
        /// Gets or sets the display name of the authority.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Authenticates the user based on data in the HTTP context
        /// and if succeeds, returns a principal identifying the user.
        /// </summary>
        /// <returns></returns>
        GraywulfPrincipal Authenticate();
    }
}
