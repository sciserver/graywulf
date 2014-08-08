using System;
using System.Security;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class QueryJobDetails : CustomPageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/QueryJobDetails.aspx?guid={0}", guid.ToString());
        }

        private Guid guid;
        private JobInstance jobInstance;
        private QueryJob queryJob;

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
            queryJob = new QueryJob(jobInstance);
        }

        private void UpdateForm()
        {
            if (!String.IsNullOrEmpty(queryJob.Error))
            {
                Error.Job = queryJob;
            }
            else
            {
                exceptionTab.Hidden = true;
            }

            Name.Text = queryJob.Name;
            Comments.Text = queryJob.Comments;
            DateCreated.Text = Util.DateFormatter.Format(queryJob.DateCreated);
            DateStarted.Text = Util.DateFormatter.Format(queryJob.DateStarted);
            DateFinished.Text = Util.DateFormatter.Format(queryJob.DateFinished);
            JobExecutionStatus.Status = queryJob.Status;

            Query.Text = queryJob.Query;

            // Set button actions
            Cancel.Enabled = queryJob.CanCancel;

            Back.OnClientClick = Util.UrlFormatter.GetClientRedirect(OriginalReferer);
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
            SetQueryInSession(queryJob.Query, null, true);
            Response.Redirect(Jhu.Graywulf.Web.UI.Query.Default.GetUrl());
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(CancelJob.GetUrl(guid));
        }
    }
}