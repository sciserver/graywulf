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

            HostName.Text = item.HostName.Value;
            AdminUrl.Text = item.AdminUrl.Value;
            DeployUncPath.Text = item.DeployUncPath.Value;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.HostName.Value = HostName.Text;
            item.AdminUrl.Value = AdminUrl.Text;
            item.DeployUncPath.Value = DeployUncPath.Text;
        }
    }
}