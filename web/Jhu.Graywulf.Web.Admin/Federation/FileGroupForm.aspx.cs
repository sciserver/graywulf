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
    public partial class FileGroupForm : EntityFormPageBase<FileGroup>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            fileGroupTypeList.SelectedValue = Item.FileGroupType.ToString();
            LayoutType.SelectedValue = Item.LayoutType.ToString();
            allocationTypeList.SelectedValue = Item.AllocationType.ToString();
            diskVolumeTypeList.SelectedValue = Item.DiskVolumeType.ToString();
            FileGroupName.Text = Item.FileGroupName;
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            FileCount.Text = Item.FileCount.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.FileGroupType = (FileGroupType)Enum.Parse(typeof(FileGroupType), fileGroupTypeList.SelectedValue);
            Item.LayoutType = (FileGroupLayoutType)Enum.Parse(typeof(FileGroupLayoutType), LayoutType.SelectedValue);
            Item.AllocationType = (FileGroupAllocationType)Enum.Parse(typeof(FileGroupAllocationType), allocationTypeList.SelectedValue);
            Item.DiskVolumeType = (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), diskVolumeTypeList.SelectedValue);
            Item.FileGroupName = FileGroupName.Text;
            Item.AllocatedSpace = Util.ByteSizeFormatter.Parse(AllocatedSpace.Text);
            Item.FileCount = int.Parse(FileCount.Text);
        }
    }
}