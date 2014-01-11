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
            MaxOutstandingJobs.Text = Item.MaxOutstandingJobs.ToString();
            Timeout.Text = Item.Timeout.ToString();

            QueueDefinition.SelectedValue = Item.QueueDefinitionReference.Guid.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.QueueDefinitionReference.Guid = new Guid(QueueDefinition.SelectedValue);
            Item.MaxOutstandingJobs = int.Parse(MaxOutstandingJobs.Text);
            Item.Timeout = int.Parse(Timeout.Text);
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