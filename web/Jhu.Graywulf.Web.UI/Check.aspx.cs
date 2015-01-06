using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web.Check;

namespace Jhu.Graywulf.Web.UI
{
    public partial class Check : CheckPageBase
    {
        protected void Page_Load()
        {
            Checks.Routines.Add(new IdentityCheck());

            // Test registry and log databases

            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString));
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            // Test SMTP and target email addresses

            Checks.Routines.Add(new EmailCheck(RegistryContext.Domain.Email));
            Checks.Routines.Add(new EmailCheck(RegistryContext.Federation.Email));

            if (Request.QueryString["email"] != null)
            {
                Checks.Routines.Add(new EmailCheck((string)Request.QueryString["email"]));
            }
            
            // Test sign-in URL
            var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
            Checks.Routines.Add(new UrlCheck(wam.GetSignInUrl()));
            
            
            Checks.Routines.Add(new UrlCheck("Download", System.Net.HttpStatusCode.Forbidden)); // No directory browsing allowed

            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName));
            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.DomainName));
            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.FederationName));

            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Jhu.Graywulf.Components.AppDomainManager.Configuration.AssemblyPath);

            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Activities.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Components.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Install.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.IO.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Jobs.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Logging.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Registry.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Schema.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Sql.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Web.dll")));
        }
    }
}