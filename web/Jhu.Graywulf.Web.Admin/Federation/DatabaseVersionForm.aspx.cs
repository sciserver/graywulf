using System;
using System.Collections;
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
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class DatabaseVersionForm : EntityFormPageBase<DatabaseVersion>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshServerVersionList();

            ServerVersion.SelectedValue = Item.ServerVersionReference.Guid.ToString();
            SizeMultiplier.Text = Item.SizeMultiplier.ToString();
        }


        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.ServerVersionReference.Guid = new Guid(ServerVersion.SelectedValue);
            Item.SizeMultiplier = float.Parse(SizeMultiplier.Text);
        }

        protected void RefreshServerVersionList()
        {
            Registry.Cluster cluster = this.Cluster;
            cluster.LoadMachineRoles(false);

            ServerVersion.Items.Add(new ListItem("(select server version)", Guid.Empty.ToString()));
            foreach (MachineRole mr in cluster.MachineRoles.Values)
            {
                mr.LoadServerVersions(false);
                foreach (ServerVersion sv in mr.ServerVersions.Values)
                {
                    ServerVersion.Items.Add(new ListItem(String.Format("{0}\\{1}", mr.Name, sv.Name), sv.Guid.ToString()));
                }
            }
        }
    }
}