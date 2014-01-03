using System;
using System.Linq;
using System.IO;
using System.Security;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class ExportJobDetails : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/ExportJobDetails.aspx?guid={0}", guid.ToString());
        }

        private Guid guid;
        private JobInstance job;
        private JobDescription ej;

        private void LoadJob()
        {
            job = new JobInstance(RegistryContext);
            job.Guid = guid;
            job.Load();

            // Check user ID
            if (job.UserGuidOwner != UserGuid)
            {
                throw new Registry.SecurityException("Access denied.");  // TODO
            }

            // Get query details
            ej = JobDescriptionFactory.GetJobDescription(job);
        }

        private void UpdateForm()
        {
            if (!String.IsNullOrEmpty(ej.Job.ExceptionMessage))
            {
                Error.JobDescription = ej;
            }
            else
            {
                multiView.Views.Remove(exceptionTab);
            }

            Name.Text = ej.Job.Name;
            Comments.Text = ej.Job.Comments;
            DateCreated.Text = Web.Util.DateFormatter.Format(ej.Job.DateCreated);
            DateStarted.Text = Web.Util.DateFormatter.Format(ej.Job.DateStarted);
            DateFinished.Text = Web.Util.DateFormatter.Format(ej.Job.DateFinished);
            JobExecutionStatus.Status = ej.Job.JobExecutionStatus;

            // Set button actions

            Cancel.Enabled = ej.Job.CanCancel;
            Cancel.OnClientClick = Web.Util.UrlFormatter.GetClientRedirect(CancelJob.GetUrl(job.Guid));

            Back.OnClientClick = Web.Util.UrlFormatter.GetClientRedirect(OriginalReferer);

            Download.Enabled = ej.Job.JobExecutionStatus == JobExecutionState.Completed;
            Download.OnClientClick = Web.Util.UrlFormatter.GetClientPopUp(GetExportUrl(ej));
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

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Download":
                    LoadJob();
                    Response.Redirect(GetExportUrl(ej));
                    break;
                case "Cancel":
                    Response.Redirect(CancelJob.GetUrl(guid));
                    break;
                case "Back":
                    Response.Redirect(OriginalReferer);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}