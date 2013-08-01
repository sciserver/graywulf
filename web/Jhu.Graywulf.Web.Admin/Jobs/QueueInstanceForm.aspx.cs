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

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class QueueInstanceForm : EntityFormPageBase<QueueInstance>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshQueueDefinitionList();
            MaxOutstandingJobs.Text = item.MaxOutstandingJobs.ToString();
            Timeout.Text = item.Timeout.ToString();

            QueueDefinition.SelectedValue = item.QueueDefinitionReference.Guid.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.QueueDefinitionReference.Guid = new Guid(QueueDefinition.SelectedValue);
            item.MaxOutstandingJobs = int.Parse(MaxOutstandingJobs.Text);
            item.Timeout = int.Parse(Timeout.Text);
        }

        private void RefreshQueueDefinitionList()
        {
            EntityFactory f = new EntityFactory(RegistryContext);

            QueueDefinition.Items.Add(new ListItem("(select queue definition)", Guid.Empty.ToString()));
            foreach (QueueDefinition m in f.FindAll<QueueDefinition>())
            {
                QueueDefinition.Items.Add(new ListItem(m.Name, m.Guid.ToString()));
            }
        }
    }
}