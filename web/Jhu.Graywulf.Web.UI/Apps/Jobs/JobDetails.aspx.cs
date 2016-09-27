using System;
using System.Security;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class JobDetails : FederationPageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Apps/Jobs/JobDetails.aspx?guid={0}", guid.ToString());
        }

        private Guid guid;
        private JobInstance jobInstance;
        private Job job;

        private void LoadJob()
        {
            jobInstance = new JobInstance(RegistryContext);
            jobInstance.Guid = guid;
            jobInstance.Load();

            // Check user ID
            if (IsAuthenticatedUser(jobInstance.UserGuidOwner))
            {
                throw new Registry.SecurityException("Access denied.");
            }

            // Get query details
            job = JobFactory.CreateJobFromInstance(jobInstance);
        }

        private void UpdateForm()
        {
            Name.Text = job.Name;
            Comments.Text = job.Comments;
            DateCreated.Text = Util.DateFormatter.Format(job.DateCreated);
            DateStarted.Text = Util.DateFormatter.Format(job.DateStarted);
            DateFinished.Text = Util.DateFormatter.Format(job.DateFinished);
            JobExecutionStatus.Status = job.Status;
            Cancel.Enabled = job.CanCancel;

            // Error

            if (!String.IsNullOrEmpty(job.Error))
            {
                ErrorForm.Job = job;
            }
            else
            {
                errorTab.Hidden = true;
            }

            // Query
            if (job is QueryJob)
            {
                queryForm.Job = (QueryJob)job;
            }
            else
            {
                queryTab.Hidden = true;
            }

            // Export
            if (job is ExportJob)
            {
                exportForm.Job = (ExportJob)job;
            }
            else
            {
                exportTab.Hidden = true;
            }

            // Import
            if (job is ImportJob)
            {
                importForm.Job = (ImportJob)job;
            }
            else
            {
                importTab.Hidden = true;
            }

            // Set button actions

            Back.OnClientClick = Util.UrlFormatter.GetClientRedirect(OriginalReferer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            guid = Guid.Parse(Request.QueryString["guid"]);

            LoadJob();
            UpdateForm();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(CancelJob.GetUrl(guid), false);
        }
    }
}