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

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class SliceDetails : EntityDetailsPageBase<Slice>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            From.Text = item.From.ToString();
            To.Text = item.To.ToString();
        }

        protected override void InitLists()
        {
            base.InitLists();

            PartitionList.ParentEntity = item;
        }

        /*
        protected void PartitionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/federation/PartitionDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddPartition_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.Partition));
        }*/
    }
}