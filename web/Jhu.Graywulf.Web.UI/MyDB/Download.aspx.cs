using System;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Download : PageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Download.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void jobDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            var factory = new JobDescriptionFactory(RegistryContext);
            factory.JobDefinitionGuids.Clear();
            factory.JobDefinitionGuids.UnionWith(JobDescriptionFactory.ExportJobDefinitionGuids);

            e.ObjectInstance = factory;
        }

        protected void JobList_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "JobDetails":
                    Response.Redirect("QueryDetails.aspx?guid=" + e.CommandArgument);
                    break;
                case "JobCancel":
                    Response.Redirect("JobCancel.aspx?guid=" + e.CommandArgument);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void JobSelected_ServerValidate(object source, ServerValidateEventArgs args)
        {

        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            if (IsValid)
            {
                var guid = new Guid(JobList.SelectedDataKeys.First());
                var guids = JobList.SelectedDataKeys.Select(i => new Guid(i)).ToArray();

                switch (e.CommandName)
                {
                    case "Download":
                        var job = new JobInstance(RegistryContext);
                        job.Guid = guid;
                        job.Load();

                        if (job.JobExecutionStatus == JobExecutionState.Completed)
                        {
                            // Get query details
                            var ej = JobDescriptionFactory.GetJobDescription(job);

                            Response.Redirect(GetExportUrl(ej));
                        }
                        else
                        {
                            JobNotCompleteValidator.IsValid = false;
                        }
                        break;
                    case "View":
                        Response.Redirect(Jhu.Graywulf.Web.UI.Jobs.ExportJobDetails.GetUrl(guid));
                        break;
                    case "Cancel":
                        Response.Redirect(Jhu.Graywulf.Web.UI.Jobs.CancelJob.GetUrl(guids));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}