using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Controls;
using Jhu.Graywulf.Web.Api.V1;
using System.Collections.Generic;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class AllList : System.Web.UI.UserControl
    {
        public MultiSelectListView List
        {
            get { return list; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected void List_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem &&
                e.Item.DataItem != null)
            {
                var job = (Job)e.Item.DataItem;
                var errorDiv = e.Item.FindControl("errorDiv");
                var errorText = (Label)e.Item.FindControl("errorText");
                var details = (Button)e.Item.FindControl("details");
                var cancel = (Button)e.Item.FindControl("cancel");

                errorDiv.Visible = !String.IsNullOrWhiteSpace(job.Error);
                errorText.Text = job.Error;

                details.OnClientClick = Util.UrlFormatter.GetClientRedirect(JobDetails.GetUrl(job.Guid));

                cancel.Visible = job.CanCancel;
                cancel.OnClientClick = Util.UrlFormatter.GetClientRedirect(CancelJob.GetUrl(job.Guid));
            }
        }
    }
}