using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Api;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class JobStatus : WebControl
    {
        private Jhu.Graywulf.Web.Api.JobStatus status;

        public Jhu.Graywulf.Web.Api.JobStatus Status
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
            this.status = Jhu.Graywulf.Web.Api.JobStatus.Unknown;
        }

        private string GetStatusBackColor()
        {
            switch (status)
            {
                case Jhu.Graywulf.Web.Api.JobStatus.Waiting:
                    return "yellow";
                case Jhu.Graywulf.Web.Api.JobStatus.Executing:
                    return "green";
                case Jhu.Graywulf.Web.Api.JobStatus.Completed:
                    return "blue";
                case Jhu.Graywulf.Web.Api.JobStatus.Canceled:
                    return "black";
                case Jhu.Graywulf.Web.Api.JobStatus.Failed:
                case Jhu.Graywulf.Web.Api.JobStatus.TimedOut:
                    return "red";
                case Jhu.Graywulf.Web.Api.JobStatus.Unknown:
                default:
                    return "";
            }
        }

        private string GetStatusForeColor()
        {
            switch (status)
            {
                case Jhu.Graywulf.Web.Api.JobStatus.Waiting:
                    return "black";
                case Jhu.Graywulf.Web.Api.JobStatus.Executing:
                    return "white";
                case Jhu.Graywulf.Web.Api.JobStatus.Completed:
                    return "white";
                case Jhu.Graywulf.Web.Api.JobStatus.Canceled:
                    return "white";
                case Jhu.Graywulf.Web.Api.JobStatus.Failed:
                case Jhu.Graywulf.Web.Api.JobStatus.TimedOut:
                    return "white";
                case Jhu.Graywulf.Web.Api.JobStatus.Unknown:
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
