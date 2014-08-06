using System;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI.Jobs;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Download : CustomPageBase
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
            RegistryContext.TransactionMode = Registry.TransactionMode.DirtyRead;
            var jf = new JobFactory(RegistryContext);
            jf.JobDefinitionGuids.Clear();
            jf.JobDefinitionGuids.UnionWith(jf.SelectJobDefinitions(JobType.Export).Select(jd => jd.Guid));

            e.ObjectInstance = jf;
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
                            var ej = new ExportJob(job);

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