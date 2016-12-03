using System;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
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

        #region Properties

        protected string CurrentView
        {
            get { return (string)(ViewState["CurrentView"] ?? "summary"); }
            set { ViewState["CurrentView"] = value; }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            guid = Guid.Parse(Request.QueryString["guid"]);

            LoadJob();
            UpdateForm();
        }

        protected void ToolbarButton_Command(object sender, CommandEventArgs e)
        {
            CurrentView = e.CommandName;
            UpdateForm();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(CancelJob.GetUrl(guid), false);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }

        #endregion

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

        private void HideAllViews()
        {
            jobForm.Visible = false;
            queryForm.Visible = false;
            copyForm.Visible = false;
            exportForm.Visible = false;
            importForm.Visible = false;
            errorForm.Visible = false;

            summary.CssClass = "";
            query.CssClass = "";
            copy.CssClass = "";
            export.CssClass = "";
            import.CssClass = "";
            error.CssClass = "";
        }

        private void UpdateForm()
        {
            HideAllViews();

            cancel.Enabled = job.CanCancel;

            query.Visible = (job is QueryJob);
            copy.Visible = (job is CopyJob);
            export.Visible = (job is ExportJob);
            import.Visible = (job is ImportJob);
            error.Visible = !String.IsNullOrEmpty(job.Error);

            switch (CurrentView)
            {
                case "summary":
                    ShowSummary();
                    break;
                case "query":
                    ShowQuery();
                    break;
                case "copy":
                    ShowCopy();
                    break;
                case "export":
                    ShowExport();
                    break;
                case "import":
                    ShowImport();
                    break;
                case "error":
                    ShowError();
                    break;
                default:
                    break;
            }
        }

        private void ShowSummary()
        {
            summary.CssClass = "selected";
            jobForm.Visible = true;
            jobForm.Job = this.job;
        }

        private void ShowQuery()
        {
            query.CssClass = "selected";
            queryForm.Visible = true;
            queryForm.Job = (QueryJob)job;
        }

        private void ShowCopy()
        {
            copy.CssClass = "selected";
            copyForm.Visible = true;
            copyForm.Job = (CopyJob)job;
        }

        private void ShowExport()
        {
            export.CssClass = "selected";
            exportForm.Visible = true;
            exportForm.Job = (ExportJob)job;
        }

        private void ShowImport()
        {
            import.CssClass = "selected";
            importForm.Visible = true;
            importForm.Job = (ImportJob)job;
        }

        private void ShowError()
        {
            error.CssClass = "selected";
            errorForm.Visible = true;
            errorForm.Job = job;
        }
    }
}