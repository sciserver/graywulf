using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Controls;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class Default : FederationPageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Jobs/Default.aspx";
        }

        enum Views : int
        {
            All = 0,
            Query = 1,
            Import = 2,
            Export = 3,
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            var list = GetVisibleListView();
            list.DataSource = JobDataSource;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        private MultiSelectGridView GetVisibleListView()
        {
            switch ((Views)multiView.ActiveViewIndex)
            {
                case Views.All:
                    return JobList;
                case Views.Query:
                    return QueryJobList;
                case Views.Export:
                    return ExportJobList;
                case Views.Import:
                    return ImportJobList;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void JobDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            RegistryContext.TransactionMode = Registry.TransactionMode.DirtyRead;
            var jf = new JobFactory(RegistryContext);

            var jobType = JobType.Unknown;
            switch ((Views)multiView.ActiveViewIndex)
            {
                case Views.All:
                    jobType = JobType.All;
                    break;
                case Views.Query:
                    jobType = JobType.Query;
                    break;
                case Views.Export:
                    jobType = JobType.Export;
                    break;
                case Views.Import:
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
            var listview = GetVisibleListView();
            args.IsValid = listview.SelectedDataKeys.Count > 0;
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            if (IsValid)
            {
                var listview = GetVisibleListView();
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
    }
}