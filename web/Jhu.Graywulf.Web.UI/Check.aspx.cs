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
        protected void Page_Load()
        {
            Checks.Routines.Add(new IisCheck());
            Checks.Routines.Add(new IdentityCheck());

            // Test registry and log databases

            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString));
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            // Test SMTP and target email addresses

            Checks.Routines.Add(new EmailCheck(RegistryContext.Domain.ShortTitle, RegistryContext.Domain.Email, RegistryContext.Domain.Email));
            Checks.Routines.Add(new EmailCheck(RegistryContext.Federation.ShortTitle, RegistryContext.Federation.Email, RegistryContext.Federation.Email));

            // Send an email to a specified address
            if (Request.QueryString["email"] != null)
            {
                Checks.Routines.Add(new EmailCheck(
                    RegistryContext.Domain.ShortTitle,
                    RegistryContext.Domain.Email,
                    Request.QueryString["email"]));
            }
            
            // Test graywulf registry entries

            Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName));
            Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.DomainName));
            Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.FederationName));

            // Test sign-in URL
            var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
            var authuri = Util.UriConverter.Combine(Request.Url, wam.GetSignInUrl()).ToString();
            Checks.Routines.Add(new UrlCheck(authuri));

            // Test download URL TODO: this one needs update, take url from export job?
            var downloaduri = Util.UriConverter.Combine(Request.Url, "Download").ToString();
            Checks.Routines.Add(new UrlCheck(downloaduri, System.Net.HttpStatusCode.Forbidden)); // No directory browsing allowed

            // Test DLLs

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

            // Test plugins

            Checks.Routines.Add(new AuthenticationCheck(RegistryContext));
            Checks.Routines.Add(new IdentityProviderCheck(RegistryContext));
            Checks.Routines.Add(new UserDatabaseCheck(RegistryContext));
            
            Checks.Routines.Add(new TypeCheck(RegistryContext.Federation.SchemaManager));
            Checks.Routines.Add(new TypeCheck(RegistryContext.Federation.QueryFactory));
            Checks.Routines.Add(new TypeCheck(RegistryContext.Federation.FileFormatFactory));
            Checks.Routines.Add(new TypeCheck(RegistryContext.Federation.StreamFactory));
        }
    }
}