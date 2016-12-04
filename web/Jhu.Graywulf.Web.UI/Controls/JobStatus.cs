using System.Drawing;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class JobStatus : Label
    {
        public Jhu.Graywulf.Web.Api.V1.JobStatus Status
        {
            get { return (Api.V1.JobStatus)(ViewState["Status"] ?? Api.V1.JobStatus.Unknown); }
            set { ViewState["Status"] = value; }
        }
        
        private void UpdateControl()
        {
            this.Text = Status.ToString().ToLower();
            this.BackColor = GetStatusBackColor();
            this.ForeColor = GetStatusForeColor();
        }

        private Color GetStatusBackColor()
        {
            switch (Status)
            {
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Waiting:
                    return Color.Yellow;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Executing:
                    return Color.Green;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Completed:
                    return Color.Blue;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Canceled:
                    return Color.Black;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Failed:
                case Jhu.Graywulf.Web.Api.V1.JobStatus.TimedOut:
                    return Color.Red;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Unknown:
                default:
                    return Color.White;
            }
        }

        private Color GetStatusForeColor()
        {
            switch (Status)
            {
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Waiting:
                    return Color.Black;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Executing:
                    return Color.White;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Completed:
                    return Color.White;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Canceled:
                    return Color.White;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Failed:
                case Jhu.Graywulf.Web.Api.V1.JobStatus.TimedOut:
                    return Color.White;
                case Jhu.Graywulf.Web.Api.V1.JobStatus.Unknown:
                default:
                    return Color.Black;
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            UpdateControl();
            base.Render(writer);
        }
    }
}
