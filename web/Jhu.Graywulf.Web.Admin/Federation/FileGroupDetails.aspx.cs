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

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class FileGroupDetails : EntityDetailsPageBase<FileGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            FileGroupType.Text = Item.FileGroupType.ToString();
            LayoutType.Text = Item.LayoutType.ToString();
            AllocationType.Text = Item.AllocationType.ToString();
            DiskDesignation.Text = Item.DiskDesignation.ToString();
            FileGroupName.Text = Item.FileGroupName;
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            FileCount.Text = Item.FileCount.ToString();
        }
    }
}