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
    public partial class ServerInstanceDetails : EntityDetailsPageBase<ServerInstance>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            ServerVersion.EntityReference.Value = item.ServerVersion;
            InstanceName.Text = item.InstanceName;
            IntegratedSecurity.Text = item.IntegratedSecurity ? Resources.Labels.IntegratedSecurity : Resources.Labels.SqlSecurity;
            AdminUser.Text = item.AdminUser;
        }
    }
}