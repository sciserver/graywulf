using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI.Api;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class JobStatus : WebControl
    {
        private Api.JobStatus status;

        public Api.JobStatus Status
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
            this.status = Api.JobStatus.Unknown;
        }

        private string GetStatusBackColor()
        {
            switch (status)
            {
                case Api.JobStatus.Waiting:
                    return "yellow";
                case Api.JobStatus.Executing:
                    return "green";
                case Api.JobStatus.Completed:
                    return "blue";
                case Api.JobStatus.Canceled:
                    return "black";
                case Api.JobStatus.Failed:
                case Api.JobStatus.TimedOut:
                    return "red";
                case Api.JobStatus.Unknown:
                default:
                    return "";
            }
        }

        private string GetStatusForeColor()
        {
            switch (status)
            {
                case Api.JobStatus.Waiting:
                    return "black";
                case Api.JobStatus.Executing:
                    return "white";
                case Api.JobStatus.Completed:
                    return "white";
                case Api.JobStatus.Canceled:
                    return "white";
                case Api.JobStatus.Failed:
                case Api.JobStatus.TimedOut:
                    return "white";
                case Api.JobStatus.Unknown:
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
