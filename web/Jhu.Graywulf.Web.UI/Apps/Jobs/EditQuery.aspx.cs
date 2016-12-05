using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class EditQuery : FederationPageBase
    {
        public static string GetUrl(Guid jobGuid)
        {
            return String.Format("~/Apps/Jobs/EditQuery.aspx?guid={0}", jobGuid);
        }

        private JobInstance jobInstance;
        private QueryJob job;

        protected Guid JobGuid
        {
            get { return Guid.Parse(Request["guid"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadJob();

            SetQueryInSession(job.Query, null, true);
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Query.Default.GetUrl(), false);
        }

        private void LoadJob()
        {
            jobInstance = new JobInstance(RegistryContext);
            jobInstance.Guid = JobGuid;
            jobInstance.Load();
            
            // Check user ID
            if (IsAuthenticatedUser(jobInstance.UserGuidOwner))
            {
                throw new Registry.SecurityException("Access denied.");
            }

            // Get query details
            job = (QueryJob)JobFactory.CreateJobFromInstance(jobInstance);
        }
    }
}