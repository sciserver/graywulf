using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class QueryDetails : JobDetails
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            var job = (QueryJob)Job;
        }
    }
}