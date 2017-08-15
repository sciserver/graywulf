using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mail;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.UI.Apps.Common
{
    public partial class Status : PageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Common/Status.aspx";
        }

        protected class Role
        {
            public string Name { get; set; }
            public List<Node> Nodes { get; set; } = new List<Node>();
        }

        protected class Node
        {
            public string Name { get; set; }
            public List<CheckRoutineBase> Checks { get; set; } = new List<CheckRoutineBase>();
        }

        protected List<Role> roles;

        protected void Page_Load(object sender, EventArgs e)
        {
            roles = new List<Role>();

            var cluster = RegistryContext.Cluster;
            cluster.LoadMachineRoles(false);

            foreach (var mr in cluster.MachineRoles.Values)
            {
                Role role = GetRole(mr);
                Node node = null;

                mr.LoadMachines(false);

                foreach (var m in mr.Machines.Values)
                {
                    m.LoadServerInstances(false);
                    var sql = m.ServerInstances.Count > 0;

                    if (sql)
                    {
                        foreach (var si in m.ServerInstances.Values)
                        {
                            node = GetNode(si);
                            RunChecks(node);
                            role.Nodes.Add(node);
                        }
                    }
                    else
                    {
                        node = GetNode(m);
                        RunChecks(node);
                        role.Nodes.Add(node);
                    }
                }

                roles.Add(role);
            }
        }

        private Role GetRole(MachineRole mr)
        {
            return new Role()
            {
                Name = mr.DisplayName
            };
        }

        private Node GetNode(Machine m)
        {
            var node = new Node()
            {
                Name = m.Name
            };

            var scheduler = m.MachineRole.Name == Jhu.Graywulf.Registry.Constants.ControllerMachineRoleName;
            var web = m.MachineRole.Name == Jhu.Graywulf.Registry.Constants.WebMachineRoleName;

            if (scheduler)
            {
                node.Checks.Add(new Scheduler.SchedulerCheck(m.HostName.ResolvedValue));
            }

            if (web)
            {
                var host = Request.Url.Host;
                var uri = String.Format("http://{0}{1}", m.HostName.ResolvedValue, VirtualPathUtility.ToAbsolute("~"));

                var check = new Jhu.Graywulf.Web.Check.UrlCheck(uri, host, System.Net.HttpStatusCode.OK);
                node.Checks.Add(check);
            }

            return node;
        }

        private Node GetNode(ServerInstance si)
        {
            var node = GetNode(si.Machine);
            node.Name = si.Machine.Name + "/" + si.Name;
            node.Checks.Add(new RemoteService.RemoteServiceCheck(si.Machine.HostName.ResolvedValue));
            node.Checks.Add(new SqlServerCheck(si.GetConnectionString().ConnectionString));
            return node;
        }

        private void RunChecks(Node node)
        {
            var checks = new CheckRoutineExecutor();
            checks.Filter = CheckCategory.All;
            checks.Routines.AddRange(node.Checks);
            checks.Execute();
        }
    }
}