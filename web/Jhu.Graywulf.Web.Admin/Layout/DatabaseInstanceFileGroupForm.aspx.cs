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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class DatabaseInstanceFileGroupForm : EntityFormPageBase<DatabaseInstanceFileGroup>
    {
        protected Registry.Cluster cluster;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshFileGroupList();
            RefreshPartitionList();

            FileGroup.SelectedValue = item.FileGroupReference.Guid.ToString();
            Partition.SelectedValue = item.PartitionReference.Guid.ToString();
            AllocatedSpace.Text = item.AllocatedSpace.ToString(); // TODO
            UsedSpace.Text = item.UsedSpace.ToString(); // TODO
            ReservedSpace.Text = item.ReservedSpace.ToString(); // TODO
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.FileGroupReference.Guid = new Guid(FileGroup.SelectedValue);
            item.PartitionReference.Guid = new Guid(Partition.SelectedValue);
            item.AllocatedSpace = long.Parse(AllocatedSpace.Text); // TODO
            item.UsedSpace = long.Parse(UsedSpace.Text);    // TODO
            item.ReservedSpace = long.Parse(ReservedSpace.Text);
        }

        protected void RefreshFileGroupList()
        {
            FileGroup.Items.Clear();
            FileGroup.Items.Add(new ListItem("(select file group)", Guid.Empty.ToString()));

            item.Context = RegistryContext;
            item.DatabaseInstance.DatabaseDefinition.LoadFileGroups(false);
            foreach (FileGroup fg in item.DatabaseInstance.DatabaseDefinition.FileGroups.Values)
            {
                FileGroup.Items.Add(new ListItem(fg.Name, fg.Guid.ToString()));
            }
        }

        protected void RefreshPartitionList()
        {
            Partition.Items.Clear();
            Partition.Items.Add(new ListItem(Resources.Labels.NotApplicable, Guid.Empty.ToString()));

            item.Context = RegistryContext;
            item.DatabaseInstance.Slice.LoadPartitions(false);
            foreach (Partition p in item.DatabaseInstance.Slice.Partitions.Values)
            {
                Partition.Items.Add(new ListItem(p.Name, p.Guid.ToString()));
            }
        }
    }
}