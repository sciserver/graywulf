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
    public partial class DatabaseInstanceFileGroupDetails : EntityDetailsPageBase<DatabaseInstanceFileGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            FileGroup.EntityReference.Value = Item.FileGroup;
            Partition.EntityReference.Value = Item.Partition;
            AllocatedSpace.Text = ByteSizeFormatter.Format(Item.AllocatedSpace);
            UsedSpace.Text = ByteSizeFormatter.Format(Item.UsedSpace); // TODO
            ReservedSpace.Text = ByteSizeFormatter.Format(Item.ReservedSpace); // TODO
        }

        protected override void InitLists()
        {
            base.InitLists();

            FileList.ParentEntity = Item;
        }

        /*
        protected void DatabaseInstanceFileList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/layout/DatabaseInstanceFileDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddDatabaseFile_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.DatabaseInstanceFile));
        }*/
    }
}