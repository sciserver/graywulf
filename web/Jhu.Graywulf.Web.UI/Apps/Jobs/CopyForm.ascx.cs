using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Jobs
{
    public partial class CopyForm : FederationUserControlBase
    {
        private CopyJob job;

        public CopyJob Job
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
            sourceDataset.Text = job.Source.Dataset;
            sourceTable.Text = job.Source.Table;
            destinationDataset.Text = job.Destination.Dataset;
            destinationTable.Text = job.Destination.Table;
        }
    }
}