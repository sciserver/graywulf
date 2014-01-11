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

namespace Jhu.Graywulf.Web.Admin.Cluster
{
    public partial class DiskVolumeForm : EntityFormPageBase<DiskVolume>
    {

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            foreach (ListItem li in DiskVolumeType.Items)
            {
                li.Selected = ((Item.DiskVolumeType & (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), li.Value)) > 0);
            }
            LocalPath.Text = Item.LocalPath.Value;
            UncPath.Text = Item.UncPath.Value;
            FullSpace.Text = ByteSizeFormatter.Format(Item.FullSpace);
            AllocatedSpace.Text = ByteSizeFormatter.Format(Item.AllocatedSpace);
            ReservedSpace.Text = ByteSizeFormatter.Format(Item.ReservedSpace);
            Speed.Text = (Item.Speed / 100000.0).ToString("0.00");
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.DiskVolumeType = Registry.DiskVolumeType.Unknown;
            foreach (ListItem li in DiskVolumeType.Items)
            {
                if (li.Selected)
                    Item.DiskVolumeType |= (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), li.Value);
            }
            Item.LocalPath.Value = LocalPath.Text;
            Item.UncPath.Value = UncPath.Text;
            Item.FullSpace = ByteSizeFormatter.Parse(FullSpace.Text);
            //item.AllocatedSpace = ByteSizeFormatter.ParseFileSize(AllocatedSpace.Text);    // read only field
            Item.ReservedSpace = ByteSizeFormatter.Parse(ReservedSpace.Text);
            Item.Speed = (long)(double.Parse(Speed.Text) * 100000.0);
        }

        protected void SpaceValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
        }
    }
}