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

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class MachineRoleDetails : EntityDetailsPageBase<MachineRole>
    {
        protected override void InitLists()
        {
            base.InitLists();

            MachineList.ParentEntity = item;
        }

        /*
        protected void MachineList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/MachineDetails.aspx?Guid=" + e.CommandArgument);
        }*/
    }
}