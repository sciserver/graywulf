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
    public partial class DiskVolumeForm : EntityFormPageBase<DiskVolume>
    {

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            foreach (ListItem li in Type.Items)
            {
                li.Selected = ((Item.Type & (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), li.Value)) > 0);
            }
            LocalPath.Text = Item.LocalPath.Value;
            UncPath.Text = Item.UncPath.Value;
            FullSpace.Text = Util.ByteSizeFormatter.Format(Item.FullSpace);
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            ReservedSpace.Text = Util.ByteSizeFormatter.Format(Item.ReservedSpace);
            ReadBandwidth.Text = (Item.ReadBandwidth / 100000.0).ToString("0.00");
            WriteBandwidth.Text = (Item.WriteBandwidth / 100000.0).ToString("0.00");
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.Type = Registry.DiskVolumeType.Unknown;

            foreach (ListItem li in Type.Items)
            {
                if (li.Selected)
                {
                    Item.Type |= (DiskVolumeType)Enum.Parse(typeof(DiskVolumeType), li.Value);
                }
            }

            Item.LocalPath.Value = LocalPath.Text;
            Item.UncPath.Value = UncPath.Text;
            Item.FullSpace = Util.ByteSizeFormatter.Parse(FullSpace.Text);
            //item.AllocatedSpace = ByteSizeFormatter.ParseFileSize(AllocatedSpace.Text);    // read only field
            Item.ReservedSpace = Util.ByteSizeFormatter.Parse(ReservedSpace.Text);
            Item.ReadBandwidth = (long)(double.Parse(ReadBandwidth.Text) * 100000.0);
            Item.WriteBandwidth = (long)(double.Parse(WriteBandwidth.Text) * 100000.0);
        }

        protected void SpaceValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
        }
    }
}