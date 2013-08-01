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

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class JobInstanceDetails : EntityDetailsPageBase<JobInstance>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            JobDefinition.EntityReference.Value = item.JobDefinition;
            WorkflowType.Text = item.WorkflowTypeName;
            DateStarted.Text = item.DateStarted == DateTime.MinValue ? "n/a" : item.DateStarted.ToString();
            DateFinished.Text = item.DateFinished == DateTime.MinValue ? "n/a" : item.DateFinished.ToString();
            ExecutionStatus.Text = item.JobExecutionStatus.ToString();
            ScheduleType.Text = item.ScheduleType.ToString();
            if (item.ScheduleType == Registry.ScheduleType.Recurring)
            {
                ScheduleTime.Text = String.Format("{0} (next: {1})", item.ScheduleTime.ToString(), item.GetNextScheduleTime().ToString());
            }
            else
            {
                ScheduleTime.Text = item.ScheduleTime.ToString();
            }
            RecurringPeriod.Text = item.RecurringPeriod.ToString();
            RecurringInterval.Text = item.RecurringInterval.ToString();

            string[] days = item.GetDayNamesFromMask();
            string daysstring = string.Empty;
            for (int i = 0; i < days.Length; i++)
            {
                if (i != 0) daysstring += ", ";
                daysstring += days[i];
            }
            RecurringMask.Text = daysstring;

            /*
            if (item.JobExecutionStatus == JobExecutionState.Suspended && item.AdminRequestData != null)
            {
                AdminRequest.Visible = true;

                AdminRequestTitle.Text = item.AdminRequestData.Title;
                AdminRequestMessage.Text = item.AdminRequestData.Message;
                AdminRequestTime.Text = item.AdminRequestTime.ToString();
                AdminRequestTimeout.Text = item.SuspendTimeout.ToString();

                for (int i = 0; i < item.AdminRequestData.Options.Count; i++)
                {
                    AdminRequestOption.Items.Add(new ListItem(item.AdminRequestData.Options[i], i.ToString()));
                }
            }
            else
            {
             * */
                AdminRequest.Visible = false;
            /*
            }
             * */

            ExceptionMessage.Text = item.ExceptionMessage;

            RefreshParametersTable();
            CheckpointProgress.Checkpoints = item.Checkpoints;
        }

        private void RefreshParametersTable()
        {
            foreach (string name in item.Parameters.Keys)
            {
                HtmlTableRow tr = new HtmlTableRow();

                // Construct label
                HtmlTableCell tdlabel = new HtmlTableCell();
                Label label = new Label();
                label.Attributes.Add("class", "FormLabel");
                label.ID = "ParameterLabel_" + name;
                label.Text = name;
                tdlabel.Controls.Add(label);
                tr.Cells.Add(tdlabel);

                // Construct textbox
                HtmlTableCell tdfield = new HtmlTableCell();
                Label value = new Label();
                value.Attributes.Add("class", "FormField");
                value.ID = "Parameter_" + name;
                value.Text = Server.HtmlEncode(item.Parameters[name].XmlValue.ToString());
                tdfield.Controls.Add(value);
                tr.Cells.Add(tdfield);

                ParametersTable.Rows.Add(tr);
            }
        }

        protected void ProcessAdminRequest_Click(object sender, EventArgs e)
        {

            /*item.JobExecutionStatus = JobExecutionState.Resumed;
            item.AdminRequestResult = int.Parse(AdminRequestOption.SelectedValue);

            item.Save();

            UpdateForm();*/
        }

        protected void Log_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/log/?jobGuid=" + item.Guid);
        }
    }
}