using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Controls
{
    public class JobStatus : WebControl
    {
        private JobExecutionState status;

        public JobExecutionState Status
        {
            get { return status; }
            set { status = value; }
        }

        public JobStatus()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.status = JobExecutionState.Unknown;
        }

        private string GetStatusBackColor()
        {
            switch (status)
            {
                case JobExecutionState.Scheduled:
                case JobExecutionState.Starting:
                case JobExecutionState.Persisted:
                case JobExecutionState.CancelRequested:
                case JobExecutionState.Persisting:
                    return "yellow";
                case JobExecutionState.Cancelled:
                case JobExecutionState.Cancelling:
                    return "black";
                case JobExecutionState.TimedOut:
                case JobExecutionState.Failed:
                    return "red";
                case JobExecutionState.Completed:
                    return "blue";
                case JobExecutionState.Executing:
                    return "green";
                default:
                    return "";
            }
        }

        private string GetStatusForeColor()
        {
            switch (status)
            {
                case JobExecutionState.Scheduled:
                case JobExecutionState.Starting:
                case JobExecutionState.Persisted:
                case JobExecutionState.CancelRequested:
                case JobExecutionState.Persisting:
                    return "black";
                case JobExecutionState.Cancelled:
                case JobExecutionState.Cancelling:
                    return "white";
                case JobExecutionState.TimedOut:
                case JobExecutionState.Failed:
                    return "white";
                case JobExecutionState.Completed:
                    return "white";
                case JobExecutionState.Executing:
                    return "white";
                default:
                    return "";
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            AddAttributesToRender(writer);

            writer.AddStyleAttribute("background-color", GetStatusBackColor());
            writer.AddStyleAttribute("color", GetStatusForeColor());

            writer.RenderBeginTag("span");

            writer.Write(status.ToString().ToLower());

            writer.RenderEndTag();
        }
    }
}
