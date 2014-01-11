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

            MaxOutstandingJobs.Text = Item.MaxOutstandingJobs.ToString();
            Timeout.Text = Item.Timeout.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.MaxOutstandingJobs = int.Parse(MaxOutstandingJobs.Text);
            Item.Timeout = int.Parse(Timeout.Text);
        }
    }
}