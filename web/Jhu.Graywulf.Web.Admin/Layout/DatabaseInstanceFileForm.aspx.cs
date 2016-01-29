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
    public partial class DatabaseInstanceFileForm : EntityFormPageBase<DatabaseInstanceFile>
    {
        protected Registry.Cluster cluster;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshDiskVolumeList();

            DiskVolume.SelectedValue = Item.DiskVolumeReference.Guid.ToString();
            DatabaseFileType.SelectedValue = Item.DatabaseFileType.ToString();
            LogicalName.Text = Item.LogicalName;
            Filename.Text = Item.Filename;
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            UsedSpace.Text = Util.ByteSizeFormatter.Format(Item.UsedSpace);
            ReservedSpace.Text = Util.ByteSizeFormatter.Format(Item.ReservedSpace);
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.DiskVolumeReference.Guid = new Guid(DiskVolume.SelectedValue);
            Item.DatabaseFileType = (DatabaseFileType)Enum.Parse(typeof(DatabaseFileType), DatabaseFileType.SelectedValue);
            Item.LogicalName = LogicalName.Text;
            Item.Filename = Filename.Text;
            Item.AllocatedSpace = Util.ByteSizeFormatter.Parse(AllocatedSpace.Text);
            Item.UsedSpace = Util.ByteSizeFormatter.Parse(UsedSpace.Text);
            Item.ReservedSpace = Util.ByteSizeFormatter.Parse(ReservedSpace.Text);
        }

        protected void RefreshDiskVolumeList()
        {
            // TODO
            DiskVolume.Items.Clear();
            DiskVolume.Items.Add(new ListItem("(select disk volume)", Guid.Empty.ToString()));

            Item.Context = RegistryContext;
            
            DatabaseInstance di;
            if (Item.Parent is DatabaseInstanceFileGroup)
            {
                var fg = (DatabaseInstanceFileGroup)Item.Parent;
                di = (DatabaseInstance)fg.Parent;
            }
            else
            {
                di = (DatabaseInstance)Item.Parent;
            }

            if (di.ServerInstance != null)
            {
                // TODO: reimplement

                throw new NotImplementedException();

                /*
                var m = (Machine)di.ServerInstance.Parent;
                m.LoadDiskVolumes(false);
                foreach (DiskVolume d in m.DiskVolumes.Values)
                {
                    DiskVolume.Items.Add(new ListItem(d.Name, d.Guid.ToString()));
                }
                 * */
            }
        }
    }
}