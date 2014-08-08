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
    public partial class DatabaseInstanceFileGroupDetails : EntityDetailsPageBase<DatabaseInstanceFileGroup>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            FileGroup.EntityReference.Value = Item.FileGroup;
            Partition.EntityReference.Value = Item.Partition;
            AllocatedSpace.Text = Util.ByteSizeFormatter.Format(Item.AllocatedSpace);
            UsedSpace.Text = Util.ByteSizeFormatter.Format(Item.UsedSpace);
            ReservedSpace.Text = Util.ByteSizeFormatter.Format(Item.ReservedSpace);
        }

        protected override void InitLists()
        {
            base.InitLists();

            FileList.ParentEntity = Item;
        }
    }
}