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

            JobDefinition.EntityReference.Value = Item.JobDefinition;
            WorkflowType.Text = Item.WorkflowTypeName;
            DateStarted.Text = Item.DateStarted == DateTime.MinValue ? "n/a" : Item.DateStarted.ToString();
            DateFinished.Text = Item.DateFinished == DateTime.MinValue ? "n/a" : Item.DateFinished.ToString();
            ExecutionStatus.Text = Item.JobExecutionStatus.ToString();
            ScheduleType.Text = Item.ScheduleType.ToString();
            if (Item.ScheduleType == Registry.ScheduleType.Recurring)
            {
                ScheduleTime.Text = String.Format("{0} (next: {1})", Item.ScheduleTime.ToString(), Item.GetNextScheduleTime().ToString());
            }
            else
            {
                ScheduleTime.Text = Item.ScheduleTime.ToString();
            }
            RecurringPeriod.Text = Item.RecurringPeriod.ToString();
            RecurringInterval.Text = Item.RecurringInterval.ToString();

            string[] days = Item.GetDayNamesFromMask();
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

            ExceptionMessage.Text = Item.ExceptionMessage;

            RefreshParametersTable();
            CheckpointProgress.Checkpoints = Item.Checkpoints;
        }

        private void RefreshParametersTable()
        {
            foreach (string name in Item.Parameters.Keys)
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
                value.Text = Server.HtmlEncode(Item.Parameters[name].XmlValue.ToString());
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
            Response.Redirect("~/log/?jobGuid=" + Item.Guid);
        }
    }
}