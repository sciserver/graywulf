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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class DatabaseInstanceFileDetails : EntityDetailsPageBase<DatabaseInstanceFile>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            DiskVolume.EntityReference.Value = item.DiskVolume;
            DatabaseFileType.Text = item.DatabaseFileType.ToString();
            LogicalName.Text = item.LogicalName;
            filename.Text = item.GetFullUncFilename();
            filename.NavigateUrl = String.Format("file:{0}", item.GetFullUncFilename().Replace('\\', '/'));
            AllocatedSpace.Text = ByteSizeFormatter.Format(item.AllocatedSpace);
            UsedSpace.Text = ByteSizeFormatter.Format(item.UsedSpace);
            ReservedSpace.Text = ByteSizeFormatter.Format(item.ReservedSpace);
        }
    }
}