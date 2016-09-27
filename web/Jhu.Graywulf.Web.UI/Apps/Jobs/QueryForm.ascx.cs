using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class QueryForm : FederationUserControlBase
    {
        private QueryJob job;

        public QueryJob Job
        {
            get { return job; }
            set
            {
                job = value;
                UpdateForm();
            }
        }

        public void UpdateForm()
        {
            Query.Text = job.Query;
        }

        protected void Edit_Click(object sender, EventArgs e)
        {
            ((FederationPageBase)Page).SetQueryInSession(job.Query, null, true);
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Query.Default.GetUrl(), false);
        }
    }
}