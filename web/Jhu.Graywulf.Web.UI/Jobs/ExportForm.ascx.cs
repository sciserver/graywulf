using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class ExportForm : CustomUserControlBase
    {
        private ExportJob job;

        public ExportJob Job
        {
            get { return job; }
            set
            {
                job = value;
                UpdateForm();
            }
        }

        public void UpdateForm()
        {
            table.Text = job.Table;

            if (job.Uri != null)
            {
                uri.Text = job.Uri.ToString();
            }

            if (job.FileFormat != null && job.FileFormat.MimeType != null)
            {
                fileFormat.Text = job.FileFormat.MimeType;
            }
        }
    }
}