using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class CheckpointProgress : System.Web.UI.UserControl
    {
        private IEnumerable<JobCheckpoint> checkpoints;

        public IEnumerable<JobCheckpoint> Checkpoints
        {
            get { return checkpoints; }
            set
            {
                checkpoints = value;
                if (checkpoints != null) RefreshCheckpointList();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void RefreshCheckpointList()
        {
            CheckpointsTable.Rows.Clear();

            HtmlTableRow tr = new HtmlTableRow();

            foreach (JobCheckpoint cp in checkpoints)
            {
                HtmlTableCell value = new HtmlTableCell();
                value.InnerText = cp.Name;

                switch (cp.ExecutionStatus)
                {
                    case JobExecutionState.Unknown:
                    case JobExecutionState.Scheduled:
                        value.Attributes.Add("class", "Checkpoint");
                        break;
                    default:
                        value.Attributes.Add("class", "Checkpoint Checkpoint" + cp.ExecutionStatus.ToString());
                        break;
                }

                tr.Cells.Add(value);
            }

            CheckpointsTable.Rows.Add(tr);
        }
    }
}