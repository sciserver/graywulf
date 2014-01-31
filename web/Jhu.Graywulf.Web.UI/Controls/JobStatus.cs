using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class JobStatus : WebControl
    {
        private Jobs.JobStatus status;

        public Jobs.JobStatus Status
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
            this.status = Jobs.JobStatus.Unknown;
        }

        private string GetStatusBackColor()
        {
            switch (status)
            {
                case Jobs.JobStatus.Waiting:
                    return "yellow";
                case Jobs.JobStatus.Executing:
                    return "green";
                case Jobs.JobStatus.Completed:
                    return "blue";
                case Jobs.JobStatus.Canceled:
                    return "black";
                case Jobs.JobStatus.Failed:
                case Jobs.JobStatus.TimedOut:
                    return "red";
                case Jobs.JobStatus.Unknown:
                default:
                    return "";
            }
        }

        private string GetStatusForeColor()
        {
            switch (status)
            {
                case Jobs.JobStatus.Waiting:
                    return "black";
                case Jobs.JobStatus.Executing:
                    return "white";
                case Jobs.JobStatus.Completed:
                    return "white";
                case Jobs.JobStatus.Canceled:
                    return "white";
                case Jobs.JobStatus.Failed:
                case Jobs.JobStatus.TimedOut:
                    return "white";
                case Jobs.JobStatus.Unknown:
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
