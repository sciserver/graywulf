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

namespace Jhu.Graywulf.Web.Admin.Cluster
{
    public partial class MachineForm : EntityFormPageBase<Machine>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            HostName.Text = Item.HostName.Value;
            AdminUrl.Text = Item.AdminUrl.Value;
            DeployUncPath.Text = Item.DeployUncPath.Value;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.HostName.Value = HostName.Text;
            Item.AdminUrl.Value = AdminUrl.Text;
            Item.DeployUncPath.Value = DeployUncPath.Text;
        }
    }
}