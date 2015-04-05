using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class QueryForm : CustomUserControlBase
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
            ((CustomPageBase)Page).SetQueryInSession(job.Query, null, true);
            Response.Redirect(Jhu.Graywulf.Web.UI.Query.Default.GetUrl());
        }
    }
}