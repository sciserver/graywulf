using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry.Check;
using Jhu.Graywulf.Web.Check;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI
{
    public partial class Check : CheckPageBase
    {
        protected override void RegisterChecks(List<CheckRoutineBase> checks)
        {
            base.RegisterChecks(checks);

            checks.Add(new IisCheck());
            checks.Add(new IdentityCheck());

            // Test registry and log databases

            checks.Add(new DatabaseCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString));
            checks.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            // Test SMTP and target email addresses

            checks.Add(new EmailCheck(RegistryContext.Domain.ShortTitle, RegistryContext.Domain.Email, RegistryContext.Domain.Email));
            checks.Add(new EmailCheck(RegistryContext.Federation.ShortTitle, RegistryContext.Federation.Email, RegistryContext.Federation.Email));

            // Send an email to a specified address
            if (Request.QueryString["email"] != null)
            {
                checks.Add(new EmailCheck(
                    RegistryContext.Domain.ShortTitle,
                    RegistryContext.Domain.Email,
                    Request.QueryString["email"]));
            }

            // Test graywulf registry entries
            checks.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName));
            checks.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.DomainName));
            checks.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.FederationName));

            // Test sign-in URL and authentication plug-ins
            var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
            var authuri = Util.UriConverter.Combine(Request.Url, wam.GetSignInUrl()).ToString();
            checks.Add(new UrlCheck(authuri));
            checks.Add(new AuthenticationCheck(RegistryContext));

            // Test DLLs
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Jhu.Graywulf.Components.AppDomainManager.Configuration.AssemblyPath);

            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Activities.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Components.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Install.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.IO.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Jobs.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Logging.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Registry.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Schema.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Sql.dll")));
            checks.Add(new AssemblyCheck(Path.Combine(dir, "Jhu.Graywulf.Web.dll")));
        }
    }
}