using System;
using System.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class QueryJobDetails : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/QueryJobDetails.aspx?guid={0}", guid.ToString());
        }

        private Guid guid;
        private JobInstance job;
        private JobDescription qj;

        private void LoadJob()
        {
            job = new JobInstance(RegistryContext);
            job.Guid = guid;
            job.Load();

            // Check user ID
            if (job.UserGuidOwner != UserGuid)
            {
                throw new Registry.SecurityException("Access denied.");
            }

            // Get query details
            qj = JobDescriptionFactory.GetJob(job, QueryFactory);
        }

        private void UpdateForm()
        {
            if (!String.IsNullOrEmpty(qj.Job.ExceptionMessage))
            {
                Error.JobDescription = qj;
            }
            else
            {
                exceptionTab.Hidden = true;
            }

            Name.Text = qj.Job.Name;
            Comments.Text = qj.Job.Comments;
            DateCreated.Text = Web.Util.DateFormatter.Format(qj.Job.DateCreated);
            DateStarted.Text = Web.Util.DateFormatter.Format(qj.Job.DateStarted);
            DateFinished.Text = Web.Util.DateFormatter.Format(qj.Job.DateFinished);
            JobExecutionStatus.Status = qj.Job.JobExecutionStatus;

            Query.Text = qj.Query;

            // Set button actions
            Cancel.Enabled = qj.Job.CanCancel;

            Back.OnClientClick = Web.Util.UrlFormatter.GetClientRedirect(OriginalReferer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            guid = Guid.Parse(Request.QueryString["guid"]);

            if (!IsPostBack)
            {
                LoadJob();
                UpdateForm();
            }
        }

        protected void Edit_Click(object sender, EventArgs e)
        {
            LoadJob();
            SetQueryInSession(qj.Query, null, true);
            Response.Redirect(Jhu.Graywulf.Web.UI.Query.Default.GetUrl());
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(CancelJob.GetUrl(guid));
        }
    }
}