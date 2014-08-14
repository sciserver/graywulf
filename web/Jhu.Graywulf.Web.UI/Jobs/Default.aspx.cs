using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Controls;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class Default : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/Jobs/Default.aspx";
        }

        enum Views : int
        {
            All = 0,
            Query = 1,
            Export = 2,
            Import = 3
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            var list = GetVisibleListView();
            list.DataSource = JobDataSource;
        }

        private void RedirectToDetails(Guid guid)
        {
            var jobInstance = new JobInstance(RegistryContext);
            jobInstance.Guid = guid;
            jobInstance.Load();

            var job = JobFactory.CreateJobFromInstance(jobInstance);

            switch (job.Type)
            {
                case JobType.Query:
                    Response.Redirect(QueryJobDetails.GetUrl(guid));
                    break;
                case JobType.Export:
                    Response.Redirect(ExportJobDetails.GetUrl(guid));
                    break;
                case JobType.Import:
                    //Response.Redirect(ImportJobDetails.GetUrl(guid));
                default:
                    throw new NotImplementedException();
            }
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
                        RedirectToDetails(guids[0]);
                        break;
                    case "Cancel":
                        Response.Redirect(Jobs.CancelJob.GetUrl(guids));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}