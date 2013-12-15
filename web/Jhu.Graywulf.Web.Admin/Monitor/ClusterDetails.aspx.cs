using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Monitor
{
    public partial class ClusterDetails : EntityDetailsPageBase<Registry.Cluster>
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Monitor/ClusterDetails.aspx?guid={0}", guid);
        }

        protected override void InitLists()
        {
            base.InitLists();

            List<DiagnosticMessage> messages = new List<DiagnosticMessage>();
            RunDiagnostics(messages, item);
            MessageList.DataSource = messages;
        }

        protected void RunDiagnostics(List<DiagnosticMessage> messages, Entity entity)
        {
            var msgs = entity.RunDiagnostics();

            messages.AddRange(msgs);

            for (int i = 0; i < msgs.Count; i++)
            {
                if (msgs[i].Status == DiagnosticMessageStatus.Error)
                {
                    return;
                }
            }

            entity.LoadAllChildren(false);

            foreach (Entity e in entity.EnumerateAllChildren())
            {
                RunDiagnostics(messages, e);
            }
        }

        protected override void UpdateForm()
        {
            base.UpdateForm();

            ProcessUser.Text = String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
        }
    }
}