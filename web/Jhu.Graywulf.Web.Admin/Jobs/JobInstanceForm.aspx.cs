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
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class JobInstanceForm : EntityFormPageBase<JobInstance>
    {
        protected Components.ParameterCollection parameters;

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshJobDefinitionList();

            JobDefinition.SelectedValue = Item.JobDefinitionReference.Guid.ToString();
            WorkflowTypeName.Text = Item.WorkflowTypeName;
            JobExecutionStatus.SelectedValue = Item.JobExecutionStatus.ToString();
            ScheduleType.SelectedValue = Item.ScheduleType.ToString();
            ScheduleTime.Text = Item.ScheduleTime.ToString();
            RecurringPeriod.SelectedValue = Item.RecurringPeriod.ToString();
            RecurringInterval.Text = Item.RecurringInterval.ToString();
            RecurringMask.Text = Item.RecurringMask.ToString();

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

            Item.JobDefinitionReference.Guid = new Guid(JobDefinition.SelectedValue);
            Item.WorkflowTypeName = WorkflowTypeName.Text;
            Item.JobExecutionStatus = (JobExecutionState)Enum.Parse(typeof(JobExecutionState), JobExecutionStatus.SelectedValue);
            Item.ScheduleType = (ScheduleType)Enum.Parse(typeof(ScheduleType), ScheduleType.SelectedValue);
            Item.ScheduleTime = DateTime.Parse(ScheduleTime.Text);
            Item.RecurringPeriod = (RecurringPeriod)Enum.Parse(typeof(RecurringPeriod), RecurringPeriod.SelectedValue);
            Item.RecurringInterval = int.Parse(RecurringInterval.Text);
            Item.RecurringMask = int.Parse(RecurringMask.Text);

            foreach (string name in parameters.Keys)
            {
                TextBox tb = (TextBox)FindControlRecursive("Parameter_" + name);
                Item.Parameters[name].XmlValue = tb.Text;
            }
        }

        private void RefreshJobDefinitionList()
        {
            JobDefinition.Items.Add(new ListItem("(select job definition)", Guid.Empty.ToString()));

            Item.Context = RegistryContext;
            IEnumerable<JobDefinition> list = null;


            Jhu.Graywulf.Registry.Federation f = Item.QueueInstance.QueueDefinition.Parent as Jhu.Graywulf.Registry.Federation;
            f.LoadJobDefinitions(false);
            list = f.JobDefinitions.Values;

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
                    if (Item != null && Item.IsExisting && Item.Parameters.ContainsKey(par.Name))
                    {
                        tb.Text = Item.Parameters[par.Name].XmlValue;
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