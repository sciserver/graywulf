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
    public partial class JobInstanceDependencyDetails : EntityDetailsPageBase<JobInstanceDependency>
    {
        protected override void InitLists()
        {
            base.InitLists();
        }

        protected override void UpdateForm()
        {
            base.UpdateForm();

            JobInstance.EntityReference.Value = Item.JobInstance;
            Condition.Text = Item.Condition.ToString();
        }
    }
}