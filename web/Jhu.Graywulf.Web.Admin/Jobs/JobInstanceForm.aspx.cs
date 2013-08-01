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
using System.Reflection;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class JobInstanceForm : EntityFormPageBase<JobInstance>
    {
        protected Dictionary<string, JobParameter> parameters;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshJobDefinitionList();

            JobDefinition.SelectedValue = item.JobDefinitionReference.Guid.ToString();
            WorkflowTypeName.Text = item.WorkflowTypeName;
            JobExecutionStatus.SelectedValue = item.JobExecutionStatus.ToString();
            ScheduleType.SelectedValue = item.ScheduleType.ToString();
            ScheduleTime.Text = item.ScheduleTime.ToString();
            RecurringPeriod.SelectedValue = item.RecurringPeriod.ToString();
            RecurringInterval.Text = item.RecurringInterval.ToString();
            RecurringMask.Text = item.RecurringMask.ToString();

            RefreshParametersTable();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack)
            {
                RefreshParametersTable();
            }

            base.OnLoad(e);
        }

        protected override void OnSaveForm()
        {

            base.OnSaveForm();

            item.JobDefinitionReference.Guid = new Guid(JobDefinition.SelectedValue);
            item.WorkflowTypeName = WorkflowTypeName.Text;
            item.JobExecutionStatus = (JobExecutionState)Enum.Parse(typeof(JobExecutionState), JobExecutionStatus.SelectedValue);
            item.ScheduleType = (ScheduleType)Enum.Parse(typeof(ScheduleType), ScheduleType.SelectedValue);
            item.ScheduleTime = DateTime.Parse(ScheduleTime.Text);
            item.RecurringPeriod = (RecurringPeriod)Enum.Parse(typeof(RecurringPeriod), RecurringPeriod.SelectedValue);
            item.RecurringInterval = int.Parse(RecurringInterval.Text);
            item.RecurringMask = int.Parse(RecurringMask.Text);

            foreach (string name in parameters.Keys)
            {
                TextBox tb = (TextBox)FindControlRecursive("Parameter_" + name);
                item.Parameters[name].XmlValue = tb.Text;
            }
        }

        private void RefreshJobDefinitionList()
        {
            JobDefinition.Items.Add(new ListItem("(select job definition)", Guid.Empty.ToString()));

            item.Context = RegistryContext;
            IEnumerable<JobDefinition> list = null;

            if (item.QueueInstance.QueueDefinition.Parent is Jhu.Graywulf.Registry.Federation)
            {
                Jhu.Graywulf.Registry.Federation f = item.QueueInstance.QueueDefinition.Parent as Jhu.Graywulf.Registry.Federation;
                f.LoadJobDefinitions(false);
                list = f.JobDefinitions.Values;
            }
            else if (item.QueueInstance.QueueDefinition.Parent is Jhu.Graywulf.Registry.Cluster)
            {
                Jhu.Graywulf.Registry.Cluster c = item.QueueInstance.QueueDefinition.Parent as Jhu.Graywulf.Registry.Cluster;
                c.LoadJobDefinitions(false);
                list = c.JobDefinitions.Values;
            }

            foreach (JobDefinition jd in list)
            {
                JobDefinition.Items.Add(new ListItem(jd.Name, jd.Guid.ToString()));
            }
        }

        private void RefreshParametersTable()
        {
            if (JobDefinition.SelectedValue != Guid.Empty.ToString())
            {
                JobDefinition jd = new JobDefinition(RegistryContext);
                jd.Guid = new Guid(JobDefinition.SelectedValue);
                jd.Load();

                parameters = jd.Parameters;

                foreach (var par in parameters.Values)
                {
                    HtmlTableRow tr = new HtmlTableRow();
                    tr.Attributes.Add("class", "FormLabel");

                    // Construct label
                    HtmlTableCell tdlabel = new HtmlTableCell();
                    Label label = new Label();
                    label.ID = "ParameterLabel_" + par.Name;
                    label.Text = par.Name;
                    tdlabel.Controls.Add(label);
                    tr.Cells.Add(tdlabel);

                    // Construct textbox
                    HtmlTableCell tdfield = new HtmlTableCell();
                    TextBox tb = new TextBox();
                    tb.ID = "Parameter_" + par.Name;
                    tb.CssClass = "FormField";
                    tb.TextMode = TextBoxMode.MultiLine;
                    tb.Rows = 3;
                    if (item != null && item.IsExisting && item.Parameters.ContainsKey(par.Name))
                    {
                        tb.Text = item.Parameters[par.Name].XmlValue;
                    }
                    tdfield.Controls.Add(tb);
                    tr.Cells.Add(tdfield);

                    ParametersTable.Rows.Add(tr);
                }
            }
        }

        protected void JobDefinition_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RefreshParametersTable();
            JobDefinition jd = new JobDefinition(RegistryContext);
            jd.Guid = new Guid(JobDefinition.SelectedValue);
            jd.Load();

            WorkflowTypeName.Text = jd.WorkflowTypeName;
        }
    }
}