using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class ExportForm : FederationUserControlBase
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
            dataset.Text = job.Source.Dataset;
            table.Text = job.Source.Table;

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