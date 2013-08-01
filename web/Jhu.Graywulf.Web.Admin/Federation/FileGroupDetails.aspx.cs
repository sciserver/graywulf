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
using Jhu.Graywulf.Web.Util;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class FileGroupDetails : EntityDetailsPageBase<FileGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            FileGroupType.Text = item.FileGroupType.ToString();
            LayoutType.Text = item.LayoutType.ToString();
            AllocationType.Text = item.AllocationType.ToString();
            DiskVolumeType.Text = item.DiskVolumeType.ToString();
            FileGroupName.Text = item.FileGroupName;
            AllocatedSpace.Text = ByteSizeFormatter.Format(item.AllocatedSpace);
            FileCount.Text = item.FileCount.ToString();
        }
    }
}