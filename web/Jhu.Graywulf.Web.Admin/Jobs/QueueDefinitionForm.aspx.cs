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
    public partial class QueueDefinitionForm : EntityFormPageBase<QueueDefinition>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            MaxOutstandingJobs.Text = item.MaxOutstandingJobs.ToString();
            Timeout.Text = item.Timeout.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.MaxOutstandingJobs = int.Parse(MaxOutstandingJobs.Text);
            item.Timeout = int.Parse(Timeout.Text);
        }
    }
}