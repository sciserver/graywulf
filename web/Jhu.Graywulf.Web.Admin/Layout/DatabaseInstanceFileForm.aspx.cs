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
    public partial class DatabaseInstanceFileForm : EntityFormPageBase<DatabaseInstanceFile>
    {
        protected Registry.Cluster cluster;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshDiskVolumeList();

            DiskVolume.SelectedValue = item.DiskVolumeReference.Guid.ToString();
            DatabaseFileType.SelectedValue = item.DatabaseFileType.ToString();
            LogicalName.Text = item.LogicalName;
            Filename.Text = item.Filename;
            AllocatedSpace.Text = ByteSizeFormatter.Format(item.AllocatedSpace);
            UsedSpace.Text = ByteSizeFormatter.Format(item.UsedSpace);
            ReservedSpace.Text = ByteSizeFormatter.Format(item.ReservedSpace);
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.DiskVolumeReference.Guid = new Guid(DiskVolume.SelectedValue);
            item.DatabaseFileType = (DatabaseFileType)Enum.Parse(typeof(DatabaseFileType), DatabaseFileType.SelectedValue);
            item.LogicalName = LogicalName.Text;
            item.Filename = Filename.Text;
            item.AllocatedSpace = ByteSizeFormatter.Parse(AllocatedSpace.Text);
            item.UsedSpace = ByteSizeFormatter.Parse(UsedSpace.Text);
            item.ReservedSpace = ByteSizeFormatter.Parse(ReservedSpace.Text);
        }

        protected void RefreshDiskVolumeList()
        {
            // TODO
            DiskVolume.Items.Clear();
            DiskVolume.Items.Add(new ListItem("(select disk volume)", Guid.Empty.ToString()));

            item.Context = RegistryContext;
            
            DatabaseInstance di;
            if (item.Parent is DatabaseInstanceFileGroup)
            {
                var fg = (DatabaseInstanceFileGroup)item.Parent;
                di = (DatabaseInstance)fg.Parent;
            }
            else
            {
                di = (DatabaseInstance)item.Parent;
            }

            if (di.ServerInstance != null)
            {
                var m = (Machine)di.ServerInstance.Parent;
                m.LoadDiskVolumes(false);
                foreach (DiskVolume d in m.DiskVolumes.Values)
                {
                    DiskVolume.Items.Add(new ListItem(d.Name, d.Guid.ToString()));
                }
            }
        }
    }
}