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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class DatabaseInstanceFileDetails : EntityDetailsPageBase<DatabaseInstanceFile>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            DiskVolume.EntityReference.Value = Item.DiskVolume;
            DatabaseFileType.Text = Item.DatabaseFileType.ToString();
            LogicalName.Text = Item.LogicalName;
            filename.Text = Item.GetFullUncFilename();
            filename.NavigateUrl = String.Format("file:{0}", Item.GetFullUncFilename().Replace('\\', '/'));
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            UsedSpace.Text = Util.ByteSizeFormatter.Format(Item.UsedSpace);
            ReservedSpace.Text = Util.ByteSizeFormatter.Format(Item.ReservedSpace);
        }
    }
}