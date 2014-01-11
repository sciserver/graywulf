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
    public partial class MachineRoleForm : EntityFormPageBase<MachineRole>
    {

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            MachineRoleType.SelectedValue = Item.MachineRoleType.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.MachineRoleType = (MachineRoleType)Enum.Parse(typeof(MachineRoleType), MachineRoleType.SelectedValue);
        }
    }
}