using System;
using System.Collections;
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
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Web.Admin.Log
{
    public partial class LogFilterControl : System.Web.UI.UserControl
    {
        protected EventFilter filter;

        protected Registry.User user;
        protected JobInstance job;
        protected Entity entity;

        public EventFilter Filter
        {
            get
            {
                SaveForm();
                return filter;
            }
        }

        public LogFilterControl()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.filter = new EventFilter();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (filter.UserGuid != Guid.Empty)
            {
                user = new Registry.User(((PageBase)Page).RegistryContext);
                user.Guid = filter.UserGuid;
                user.Load();

                linkUser.EntityReference.Value = user;
                linkUser.Visible = true;
            }
            else
            {
                linkUser.Visible = false;
            }

            if (filter.JobGuid != Guid.Empty)
            {
                job = new JobInstance(((PageBase)Page).RegistryContext);
                job.Guid = filter.JobGuid;
                job.Load();

                linkJob.EntityReference.Value = job;
                linkJob.Visible = true;
            }
            else
            {
                linkJob.Visible = false;
            }

            if (filter.EntityGuid != Guid.Empty)
            {
                entity = new Entity(((PageBase)Page).RegistryContext);
                entity.Guid = filter.EntityGuid;
                entity.Load();

                linkEntity.EntityReference.Value = entity;
                linkEntity.Visible = true;
            }
            else
            {
                linkEntity.Visible = false;
            }
        }

        protected void SaveForm()
        {
            switch (EventDateTimeRange.SelectedValue)
            {
                case "All":
                    filter.EventDateTimeFrom = DateTime.MinValue;
                    filter.EventDateTimeTo = DateTime.MaxValue;
                    break;
                case "Custom":
                    filter.EventDateTimeFrom = DateTime.Parse(EventDateTimeFrom.Text);
                    filter.EventDateTimeTo = DateTime.Parse(EventDateTimeTo.Text);
                    break;
                default:    // Last{number} format where the number is the hours
                    int hours = int.Parse(EventDateTimeRange.SelectedValue.Substring(4));
                    filter.EventDateTimeFrom = DateTime.Now - new TimeSpan(hours, 0, 0);
                    filter.EventDateTimeTo = DateTime.Now;
                    break;
            }

            filter.ExecutionStatus.Clear();
            foreach (ListItem i in ExecutionStatus.Items)
            {
                if (i.Selected)
                {
                    filter.ExecutionStatus.Add((ExecutionStatus)Enum.Parse(typeof(ExecutionStatus), i.Value));
                }
            }

            filter.EventSource = Logging.EventSource.None;
            foreach (ListItem i in EventSource.Items)
            {
                if (i.Selected)
                {
                    filter.EventSource |= (EventSource)Enum.Parse(typeof(EventSource), i.Value);
                }
            }

            filter.EventSeverity = Logging.EventSeverity.None;
            foreach (ListItem i in EventSeverity.Items)
            {
                if (i.Selected)
                {
                    filter.EventSeverity |= (EventSeverity)Enum.Parse(typeof(EventSeverity), i.Value);
                }
            }

        }

        public void RefreshForm()
        {
        }

        protected void EventDateTimeRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventDateTimeFrom.Enabled = EventDateTimeTo.Enabled = (EventDateTimeRange.SelectedValue == "Custom");
        }

    }
}