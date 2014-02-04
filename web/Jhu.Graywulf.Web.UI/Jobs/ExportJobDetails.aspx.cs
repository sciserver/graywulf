using System;
using System.Linq;
using System.IO;
using System.Security;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class ExportJobDetails : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/ExportJobDetails.aspx?guid={0}", guid.ToString());
        }

        private Guid guid;
        private JobInstance jobInstance;
        private ExportJob exportJob;

        private void LoadJob()
        {
            jobInstance = new JobInstance(RegistryContext);
            jobInstance.Guid = guid;
            jobInstance.Load();

            // Check user ID
            if (IsAuthenticatedUser(jobInstance.UserGuidOwner))
            {
                throw new Registry.SecurityException("Access denied.");  // TODO
            }

            // Get query details
            exportJob = new ExportJob(jobInstance);
        }

        private void UpdateForm()
        {
            if (!String.IsNullOrEmpty(exportJob.Error))
            {
                Error.Job = exportJob;
            }
            else
            {
                multiView.Views.Remove(exceptionTab);
            }

            Name.Text = exportJob.Name;
            Comments.Text = exportJob.Comments;
            DateCreated.Text = Web.Util.DateFormatter.Format(exportJob.DateCreated);
            DateStarted.Text = Web.Util.DateFormatter.Format(exportJob.DateStarted);
            DateFinished.Text = Web.Util.DateFormatter.Format(exportJob.DateFinished);
            JobExecutionStatus.Status = exportJob.Status;

            // Set button actions

            Cancel.Enabled = exportJob.CanCancel;
            Cancel.OnClientClick = Web.Util.UrlFormatter.GetClientRedirect(CancelJob.GetUrl(exportJob.Guid));

            Back.OnClientClick = Web.Util.UrlFormatter.GetClientRedirect(OriginalReferer);

            Download.Enabled = exportJob.Status == JobStatus.Completed;
            Download.OnClientClick = Web.Util.UrlFormatter.GetClientPopUp(GetExportUrl(exportJob));
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
                    Response.Redirect(GetExportUrl(exportJob));
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