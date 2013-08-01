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
    public partial class DiskVolumeDetails : EntityDetailsPageBase<DiskVolume>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            DiskVolumeType.Text = item.DiskVolumeType.ToString();
            LocalPath.Text = String.Format("{0} ({1})", item.LocalPath.Value, item.LocalPath.ResolvedValue);
            UncPath.Text = String.Format("{0} ({1})", item.UncPath.Value, item.UncPath.ResolvedValue);
            FullSpace.Text = ByteSizeFormatter.Format(item.FullSpace);
            AllocatedSpace.Text = ByteSizeFormatter.Format(item.AllocatedSpace);
            ReservedSpace.Text = ByteSizeFormatter.Format(item.ReservedSpace);
            Speed.Text = (item.Speed / 100000.0).ToString("0.00");

            Usage.Values.Clear();
            Usage.Values.Add((double)item.AllocatedSpace / item.FullSpace);
            Usage.Values.Add((double)item.ReservedSpace / item.FullSpace);
        }
    }
}