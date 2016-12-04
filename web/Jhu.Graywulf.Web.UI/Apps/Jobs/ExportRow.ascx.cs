using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ExportRow : System.Web.UI.UserControl
    {
        private Job job;

        public Job Job
        {
            get { return this.job; }
            set
            {
                this.job = value;
                UpdateForm();
            }
        }

        private void UpdateForm()
        {
        }

        
    }
}