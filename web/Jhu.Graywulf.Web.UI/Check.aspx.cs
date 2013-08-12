using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.IO;
using Jhu.Graywulf.Web.Check;

namespace Jhu.Graywulf.Web.UI
{
    public partial class Check : CheckPageBase
    {
        protected void Page_Load()
        {
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Registry.AppSettings.ConnectionString));
            Checks.Routines.Add(new DatabaseCheck(Jhu.Graywulf.Logging.AppSettings.ConnectionString));

            Checks.Routines.Add(new EmailCheck(Domain.Email));
            Checks.Routines.Add(new EmailCheck(Federation.Email));

            Checks.Routines.Add(new UrlCheck(FormsAuthentication.LoginUrl));
            Checks.Routines.Add(new UrlCheck("Download", System.Net.HttpStatusCode.Forbidden)); // No directory browsing allowed

            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.Cluster.AppSettings.ClusterName));
            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.Domain.AppSettings.DomainName));
            Checks.Routines.Add(new EntityCheck(Jhu.Graywulf.Registry.Federation.AppSettings.FederationName));

            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Activities.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Components.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Format.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.IO.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Jobs.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Logging.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Activities.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Registry.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.Schema.dll")));
            Checks.Routines.Add(new AssemblyCheck(Path.Combine(Jhu.Graywulf.Activities.AppSettings.WorkflowAssemblyPath, "Jhu.Graywulf.SqlParser.dll")));
        }
    }
}