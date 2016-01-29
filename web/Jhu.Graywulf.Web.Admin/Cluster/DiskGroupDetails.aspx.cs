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
    public partial class DiskGroupDetails : EntityDetailsPageBase<DiskGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            Type.Text = Item.Type.ToString();
            FullSpace.Text = Util.ByteSizeFormatter.Format(Item.FullSpace);
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            ReservedSpace.Text = Util.ByteSizeFormatter.Format(Item.ReservedSpace);
            ReadBandwidth.Text = (Item.ReadBandwidth / 100000.0).ToString("0.00");
            WriteBandwidth.Text = (Item.WriteBandwidth / 100000.0).ToString("0.00");

            Usage.Values.Clear();
            Usage.Values.Add((double)Item.AllocatedSpace / Item.FullSpace);
            Usage.Values.Add((double)Item.ReservedSpace / Item.FullSpace);
        }

        protected override void InitLists()
        {
            base.InitLists();

            DiskVolumeList.ParentEntity = Item;
        }
    }
}