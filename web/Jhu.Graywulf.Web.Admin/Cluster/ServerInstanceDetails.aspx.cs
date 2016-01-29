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

            ServerVersion.EntityReference.Value = Item.ServerVersion;
            InstanceName.Text = Item.InstanceName;
            IntegratedSecurity.Text = Item.IntegratedSecurity ? Resources.Labels.IntegratedSecurity : Resources.Labels.SqlSecurity;
            AdminUser.Text = Item.AdminUser;
        }

        protected override void InitLists()
        {
            base.InitLists();

            DiskGroupList.ParentEntity = Item;
        }
    }
}