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
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.Admin.Cluster
{
    public partial class ServerInstanceDiskGroupDetails : EntityDetailsPageBase<ServerInstanceDiskGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            DiskGroup.EntityReference.Value = Item.DiskGroup;
            DiskDesignation.Text = Item.DiskDesignation.ToString();
        }
    }
}