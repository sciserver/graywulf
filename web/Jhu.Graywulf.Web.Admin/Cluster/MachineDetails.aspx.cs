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
    public partial class MachineDetails : EntityDetailsPageBase<Machine>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            HostName.Text = String.Format("{0} ({1})", Item.HostName.Value, Item.HostName.ResolvedValue);
            AdminUrl.Text = String.Format("{0} ({1})", Item.AdminUrl.Value, Item.AdminUrl.ResolvedValue);
            AdminUrl.NavigateUrl = Item.AdminUrl.ResolvedValue;
            DeployUncPath.Text = String.Format("{0} ({1})", Item.DeployUncPath.Value, Item.DeployUncPath.ResolvedValue);
            DeployUncPath.NavigateUrl = Item.DeployUncPath.ResolvedValue;
        }

        protected override void InitLists()
        {
            base.InitLists();

            DiskGroupList.ParentEntity = Item;
            ServerInstanceList.ParentEntity = Item;
        }
    }
}