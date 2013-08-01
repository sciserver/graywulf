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

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class MachineDetails : EntityDetailsPageBase<Machine>
    {
        protected override void InitLists()
        {
            base.InitLists();

            QueueInstanceList.ParentEntity = item;
        }

        /*
        protected void QueueInstanceList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/QueueInstanceDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddQueueInstance_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.QueueInstance));
        }

        protected void DeleteQueueInstance_Click(object sender, EventArgs e)
        {
            if (QueueInstanceList.SelectedKeys.Count == 1)
            {
                Response.Redirect("../EntityDelete.aspx?guid=" + QueueInstanceList.FirstSelectedKey + "&redirect=" + HttpUtility.UrlEncode(Request.RawUrl));
            }
        }
         * */
    }
    
}