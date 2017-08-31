using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class Default : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string view)
        {
            var url = "~/Apps/Jobs/Default.aspx";

            if (view != null)
            {
                url += "?view=" + view;
            }

            return url;
        }

        #region Properties

        protected string CurrentView
        {
            get { return (string)(Request["view"] ?? "all"); }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateForm();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            list.DataSource = JobDataSource;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void ToolbarButton_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect(GetUrl(e.CommandName));
        }

        // TODO: simplify this a little bit and try to get rid of the many switches

        protected void JobDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            RegistryContext.TransactionMode = Registry.TransactionMode.DirtyRead;
            var jf = new JobFactory(RegistryContext);

            var jobType = JobType.Unknown;
            switch (CurrentView)
            {
                case "all":
                    jobType = JobType.All;
                    break;
                case "query":
                    jobType = JobType.Query;
                    break;
                case "copy":
                    jobType = JobType.Copy;
                    break;
                case "export":
                    jobType = JobType.Export;
                    break;
                case "import":
                    jobType = JobType.Import;
                    break;
                case "script":
                    jobType = JobType.SqlScript;
                    break;
                default:
                    throw new NotImplementedException();
            }

            jf.JobDefinitionGuids.UnionWith(jf.SelectJobDefinitions(jobType).Select(jd => jd.Guid));

            e.ObjectInstance = jf;
        }

        protected void List_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem &&
                e.Item.DataItem != null)
            {
                var job = (Job)e.Item.DataItem;
                var detailsPlaceholder = (PlaceHolder)e.Item.FindControl("detailsPlaceholder");
                var error = (ErrorDetails)e.Item.FindControl("errorDetails");
                var duration = (Web.Controls.FancyTimeSpanLabel)e.Item.FindControl("duration");

                error.Job = job;

                JobDetails details = null;

                if (job is CopyJob)
                {
                    details = (JobDetails)LoadControl("CopyDetails.ascx");
                }
                else if (job is ExportJob)
                {
                    details = (JobDetails)LoadControl("ExportDetails.ascx");
                }
                else if (job is ImportJob)
                {
                    details = (JobDetails)LoadControl("ImportDetails.ascx");
                }
                else if (job is QueryJob)
                {
                    details = (JobDetails)LoadControl("QueryDetails.ascx");
                }
                else
                {
                    details = (JobDetails)LoadControl("JobDetails.ascx");
                }

                if (job.DateFinished != DateTime.MinValue)
                {
                    duration.Value = job.DateFinished - job.DateStarted;
                }
                else
                {
                    duration.Value = TimeSpan.MinValue;
                }

                details.Job = job;
                detailsPlaceholder.Controls.Add(details);
            }
        }

        protected void JobSelected_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = list.SelectedDataKeys.Count > 0;
        }

        #endregion

        private void HideAllViews()
        {
            all.CssClass = "";
            query.CssClass = "";
            copy.CssClass = "";
            export.CssClass = "";
            import.CssClass = "";
            script.CssClass = "";
        }
        private void UpdateForm()
        {
            HideAllViews();

            switch (CurrentView)
            {
                case "all":
                    ShowAll();
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
                case "script":
                    ShowScript();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ShowAll()
        {
            all.CssClass = "selected";
        }

        private void ShowCopy()
        {
            copy.CssClass = "selected";
        }

        private void ShowQuery()
        {
            query.CssClass = "selected";
        }

        private void ShowExport()
        {
            export.CssClass = "selected";
        }

        private void ShowImport()
        {
            import.CssClass = "selected";
        }

        private void ShowScript()
        {
            script.CssClass = "selected";
        }
    }
}