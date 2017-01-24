using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry.Check;
using Jhu.Graywulf.Web.Check;
using Jhu.Graywulf.RemoteService;

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

            RegisterDllChecks(checks);

            // Test remoting service on hosts running SQL Server
            RegisterRemotingServiceChecks(checks);

            // TODO: scheduler test

            // These take a long time so only add if not filtered
            if ((Filter & CheckCategory.Service) != 0)
            {
                RegisterServerInstanceChecks(checks);
            }

            if ((Filter & CheckCategory.Database) != 0)
            {
                RegisterDatabaseChecks(checks);
            }
        }

        private void RegisterDllChecks(List<CheckRoutineBase> checks)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            dir = Path.Combine(dir, Jhu.Graywulf.Components.AppDomainManager.Configuration.AssemblyPath);

            var webassembly = Assembly.GetAssembly(this.GetType());
            var check = new AssemblyCheck(dir, webassembly);

            checks.Add(check);
        }

        private void RegisterRemotingServiceChecks(List<CheckRoutineBase> checks)
        {
            var hosts = new HashSet<string>();

            foreach (var si in RegistryContext.Cluster.FindServerInstances())
            {
                var host = si.Machine.HostName.ResolvedValue;

                if (!hosts.Contains(host))
                {
                    hosts.Add(host);
                }
            }

            foreach (var host in hosts)
            {
                checks.Add(new RemoteServiceCheck(host));
            }
        }

        private void RegisterServerInstanceChecks(List<CheckRoutineBase> checks)
        {
            foreach (var si in RegistryContext.Cluster.FindServerInstances())
            {
                var csb = si.GetConnectionString();
                csb.Pooling = false;
                csb.ConnectTimeout = 1;
                var dbc = new SqlServerCheck(csb.ConnectionString);
                checks.Add(dbc);
            }
        }

        private void RegisterDatabaseChecks(List<CheckRoutineBase> checks)
        {
            foreach (var di in RegistryContext.Federation.FindDatabaseInstances())
            {
                var csb = di.GetConnectionString();
                csb.Pooling = false;
                csb.ConnectTimeout = 1;
                var dbc = new DatabaseCheck(csb.ConnectionString);
                checks.Add(dbc);
            }
        }
    }
}