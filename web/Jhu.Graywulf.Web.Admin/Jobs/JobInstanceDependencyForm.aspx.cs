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
    public partial class JobInstanceDependencyForm : EntityFormPageBase<JobInstanceDependency>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshJobInstanceList();

            Condition.SelectedValue = Item.Condition.ToString();
            PredecessorJobInstance.SelectedValue = Item.PredecessorJobInstanceReference.Guid.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.Condition = (JobDependencyCondition)Enum.Parse(typeof(JobDependencyCondition), Condition.SelectedValue);
            Item.PredecessorJobInstanceReference.Guid = new Guid(PredecessorJobInstance.SelectedValue);
        }

        private void RefreshJobInstanceList()
        {
            // TODO: this needs a lot of optimization here
            Item.JobInstance.QueueInstance.LoadJobInstances(false);

            PredecessorJobInstance.Items.Add(new ListItem("(select predecessor job)", Guid.Empty.ToString()));
            foreach (var j in Item.JobInstance.QueueInstance.JobInstances.Values)
            {
                PredecessorJobInstance.Items.Add(new ListItem(j.Name, j.Guid.ToString()));
            }
        }
    }
}