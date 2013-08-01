using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Query
{
    public partial class ResultsProgress : PageBase
    {
        public static string GetUrl()
        {
            return "~/Query/ResultsProgress.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistryContext.TransactionMode = Registry.TransactionMode.DirtyRead;

            var ji = new JobInstance(RegistryContext);
            ji.Guid = Guid.Parse(Request.QueryString["guid"]);
            ji.Load();

            switch (ji.JobExecutionStatus)
            {
                case JobExecutionState.Completed:
                    Response.Redirect(String.Format("Results.aspx?guid={0}", ji.Guid));
                    break;
                case JobExecutionState.Cancelled:
                case JobExecutionState.Failed:
                case JobExecutionState.TimedOut:
                    // *** TODO
                    ProgressLabel.Text = "Query execution failed: " + ji.ExceptionMessage;
                    RefreshMeta.Parent.Controls.Remove(RefreshMeta);
                    break;
                default:
                    break;
            }
        }
    }
}