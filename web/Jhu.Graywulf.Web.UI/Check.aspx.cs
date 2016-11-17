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
        [Flags]
        enum CheckMode : int
        {
            Iis = 0x01,
            Dlls = 0x02,
            Plugins = 0x04,
            Database = 0x08,
            Email = 0x10,
            Registry = 0x20,
            Services = 0x30,

            Quick = Iis,
            All = 0xFFFF,
            NoEmail = All & ~Email,
        }

        protected void Page_Load()
        {
            CheckMode mode;

            if (!Enum.TryParse(Request.QueryString["mode"], true, out mode))
            {
                mode = CheckMode.All;
            }

            AddChecks(mode);
        }

        private void AddChecks(CheckMode mode)
        {
            if (mode.HasFlag(CheckMode.Iis))
            {
                Checks.Routines.Add(new IisCheck());
                Checks.Routines.Add(new IdentityCheck());
            }

            // Test registry and log databases
            if (mode.HasFlag(CheckMode.Database))
            {
                Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString));
                Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));
            }

            // Test SMTP and target email addresses
            if (mode.HasFlag(CheckMode.Email))
            {
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
            }

            // Test graywulf registry entries
            if (mode.HasFlag(CheckMode.Registry))
            {
                Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName));
                Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.DomainName));
                Checks.Routines.Add(new EntityCheck(RegistryContext, Jhu.Graywulf.Registry.ContextManager.Configuration.FederationName));
            }

            // Test sign-in URL
            if (mode.HasFlag(CheckMode.Services))
            {
                var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
                var authuri = Util.UriConverter.Combine(Request.Url, wam.GetSignInUrl()).ToString();
                Checks.Routines.Add(new UrlCheck(authuri));
            }

            // Test DLLs
            if (mode.HasFlag(CheckMode.Dlls))
            {
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

            // Test plugins
            if (mode.HasFlag(CheckMode.Plugins))
            {
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
}