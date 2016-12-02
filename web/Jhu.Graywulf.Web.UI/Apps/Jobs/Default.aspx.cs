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
            return "~/Apps/Jobs/Default.aspx";
        }

        #region Properties

        protected string CurrentView
        {
            get { return (string)(ViewState["CurrentView"] ?? "all"); }
            set { ViewState["CurrentView"] = value; }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateForm();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            var list = GetCurrentList().List;
            list.DataSource = JobDataSource;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void ToolbarButton_Command(object sender, CommandEventArgs e)
        {
            CurrentView = e.CommandName;
            UpdateForm();
        }

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
                default:
                    throw new NotImplementedException();
            }

            jf.JobDefinitionGuids.UnionWith(jf.SelectJobDefinitions(jobType).Select(jd => jd.Guid));

            e.ObjectInstance = jf;
        }

        protected void JobSelected_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var listview = GetCurrentList().List;
            args.IsValid = listview.SelectedDataKeys.Count > 0;
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            if (IsValid)
            {
                var listview = GetCurrentList().List;
                var guids = listview.SelectedDataKeys.Select(g => Guid.Parse(g)).ToArray();

                switch (e.CommandName)
                {
                    case "Details":
                        Response.Redirect(JobDetails.GetUrl(guids[0]), false);
                        break;
                    case "Cancel":
                        Response.Redirect(Jobs.CancelJob.GetUrl(guids), false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        private IJobList GetCurrentList()
        {
            switch (CurrentView)
            {
                case "all":
                    return jobList;
                case "query":
                    return queryList;
                case "copy":
                    return copyList;
                case "export":
                    return exportList;
                case "import":
                    return importList;
                default:
                    throw new NotImplementedException();
            }
        }

        private void HideAllViews()
        {
            jobList.Visible = false;
            queryList.Visible = false;
            copyList.Visible = false;
            exportList.Visible = false;
            importList.Visible = false;

            all.CssClass = "";
            query.CssClass = "";
            copy.CssClass = "";
            export.CssClass = "";
            import.CssClass = "";
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
                default:
                    throw new NotImplementedException();
            }
        }

        private void ShowAll()
        {
            jobList.Visible = true;
            all.CssClass = "selected";
        }

        private void ShowCopy()
        {
            copyList.Visible = true;
            copy.CssClass = "selected";
        }

        private void ShowQuery()
        {
            queryList.Visible = true;
            query.CssClass = "selected";
        }

        private void ShowExport()
        {
            exportList.Visible = true;
            export.CssClass = "selected";
        }

        private void ShowImport()
        {
            importList.Visible = true;
            import.CssClass = "selected";
        }

        
    }
}