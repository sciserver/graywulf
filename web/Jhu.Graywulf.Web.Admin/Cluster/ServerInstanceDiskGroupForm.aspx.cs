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
    public partial class ServerInstanceDiskGroupForm : EntityFormPageBase<ServerInstanceDiskGroup>
    {

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            foreach (ListItem li in DiskDesignation.Items)
            {
                li.Selected = ((Item.DiskDesignation & (DiskDesignation)Enum.Parse(typeof(DiskDesignation), li.Value)) > 0);
            }

            RefreshDiskGroupList();

            DiskGroup.SelectedValue = Item.DiskGroupReference.Guid.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.DiskDesignation = Registry.DiskDesignation.Unknown;

            foreach (ListItem li in DiskDesignation.Items)
            {
                if (li.Selected)
                {
                    Item.DiskDesignation |= (DiskDesignation)Enum.Parse(typeof(DiskDesignation), li.Value);
                }
            }

            Item.DiskGroupReference.Guid = new Guid(DiskGroup.SelectedValue);
        }

        private void RefreshDiskGroupList()
        {
            Item.ServerInstance.Machine.LoadDiskGroups(false);

            DiskGroup.Items.Add(new ListItem("(select disk group)", Guid.Empty.ToString()));
            foreach (var dg in Item.ServerInstance.Machine.DiskGroups.Values)
            {
                DiskGroup.Items.Add(new ListItem(dg.Name, dg.Guid.ToString()));
            }
        }

    }
}