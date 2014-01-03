using System;
using System.Linq;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Controls;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return "~/Jobs/Default.aspx";
        }

        private void RedirectToDetails(Guid guid)
        {
            var job = new JobInstance(RegistryContext);
            job.Guid = guid;
            job.Load();

            var wjob = JobDescriptionFactory.GetJobDescription(job);

            switch (wjob.JobType)
            {
                case JobType.Query:
                    Response.Redirect(QueryJobDetails.GetUrl(guid));
                    break;
                case JobType.ExportTable:
                    Response.Redirect(ExportJobDetails.GetUrl(guid));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private MultiSelectGridView GetVisibleListView()
        {
            switch (multiView.ActiveViewIndex)
            {
                case 0:
                    return JobList;
                case 1:
                    return QueryJobList;
                case 2:
                    return ExportJobList;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void jobDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            RegistryContext.TransactionMode = Registry.TransactionMode.DirtyRead;
            var factory = new JobDescriptionFactory(RegistryContext, QueryFactory.Create(RegistryContext));
            e.ObjectInstance = factory;

            switch (multiView.ActiveViewIndex)
            {
                case 0:     // all jobs
                    break;
                case 1:     // query jobs
                    factory.JobDefinitionGuids.Clear();
                    factory.JobDefinitionGuids.UnionWith(JobDescriptionFactory.QueryJobDefinitionGuids);
                    break;
                case 2:     // export jobs
                    factory.JobDefinitionGuids.Clear();
                    factory.JobDefinitionGuids.UnionWith(JobDescriptionFactory.ExportJobDefinitionGuids);
                    break;
                default:
                    throw new NotImplementedException();
            }
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