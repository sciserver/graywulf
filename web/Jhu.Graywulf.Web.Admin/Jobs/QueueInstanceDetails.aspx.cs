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
    public partial class QueueInstanceDetails : EntityDetailsPageBase<QueueInstance>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            QueueDefinition.EntityReference.Value = Item.QueueDefinition;
            MaxOutstandingJobs.Text = Item.MaxOutstandingJobs.ToString();
            Timeout.Text = Item.Timeout.ToString();
        }

        protected override void InitLists()
        {
            base.InitLists();

            JobInstanceList.ParentEntity = Item;
        }
    }
}