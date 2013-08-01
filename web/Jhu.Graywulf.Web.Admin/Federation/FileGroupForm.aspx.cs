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
    public partial class FileGroupForm : EntityFormPageBase<FileGroup>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            fileGroupTypeList.SelectedValue = item.FileGroupType.ToString();
            LayoutType.SelectedValue = item.LayoutType.ToString();
            allocationTypeList.SelectedValue = item.AllocationType.ToString();
            diskVolumeTypeList.SelectedValue = item.DiskVolumeType.ToString();
            FileGroupName.Text = item.FileGroupName;
            AllocatedSpace.Text = ByteSizeFormatter.Format(item.AllocatedSpace);
            FileCount.Text = item.FileCount.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.FileGroupType = (FileGroupType)Enum.Parse(typeof(FileGroupType), fileGroupTypeList.SelectedValue);
            item.LayoutType = (FileGroupLayoutType)Enum.Parse(typeof(FileGroupLayoutType), LayoutType.SelectedValue);
            item.AllocationType = (FileGroupAllocationType)Enum.Parse(typeof(FileGroupAllocationType), allocationTypeList.SelectedValue);
            item.DiskVolumeType = (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), diskVolumeTypeList.SelectedValue);
            item.FileGroupName = FileGroupName.Text;
            item.AllocatedSpace = ByteSizeFormatter.Parse(AllocatedSpace.Text);
            item.FileCount = int.Parse(FileCount.Text);
        }
    }
}