using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;


namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public partial class Progress : System.Web.UI.Page
    {
        public static string GetUrl(Guid jobGuid)
        {
            return String.Format("~/Apps/Query/Progress.aspx?guid={0}", jobGuid);
        }

        private JobInstance jobInstance;

        protected Guid JobGuid
        {
            get { return Guid.Parse(Request["guid"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            // Use lower isolation level for polling
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.DirtyRead))
            {
                jobInstance = new JobInstance(context);
                jobInstance.Guid = JobGuid;
                jobInstance.Load();

                var job = Web.Api.V1.Job.FromJobInstance(jobInstance);

                switch (jobInstance.JobExecutionStatus)
                {
                    case JobExecutionState.Completed:
                        Response.Redirect(Results.GetUrl(JobGuid));
                        break;
                    case JobExecutionState.Failed:
                        statusPanel.Visible = true;
                        status.Status = job.Status;
                        exceptionLabel.Visible = true;
                        exception.Visible = false;
                        exception.Text = jobInstance.ExceptionMessage;
                        break;
                    default:
                        statusPanel.Visible = true;
                        status.Status = job.Status;
                        break;
                }
            }
        }

        protected void StatusTimer_Tick(object sender, EventArgs e)
        {
        }
    }
}