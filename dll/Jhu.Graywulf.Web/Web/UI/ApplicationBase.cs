using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class ApplicationBase : System.Web.HttpApplication
    {
        #region User managemenet functions

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        internal protected virtual void OnUserArrived(GraywulfPrincipal principal)
        { }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        internal protected virtual void OnUserLeft(GraywulfPrincipal principal)
        { }

        #endregion
    }
}
