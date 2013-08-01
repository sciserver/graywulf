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
    public partial class QueueInstanceDetails : EntityDetailsPageBase<QueueInstance>
    {
        protected override void InitLists()
        {
            base.InitLists();

            JobInstanceList.ParentEntity = item;
        }

        protected override void UpdateForm()
        {
            base.UpdateForm();

            QueueDefinition.EntityReference.Value = item.QueueDefinition;
            MaxOutstandingJobs.Text = item.MaxOutstandingJobs.ToString();
            Timeout.Text = item.Timeout.ToString();

            //switch (item.RunningState)
            //{
            //    case RunningState.Running:
            //        ChangeRunningState.Text = "Pause";   // *** TODO add resource
            //        break;
            //    case RunningState.Paused:
            //        ChangeRunningState.Text = "Resume";   // *** TODO add resource
            //        break;
            //    default:
            //        throw new NotImplementedException();
            //}
        }

        /*
        protected void JobInstanceList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/jobs/JobInstanceDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddJobInstance_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.JobInstance));
        }

        protected void ChangeRunningState_Click(object sender, EventArgs e)
        {
            switch (item.RunningState)
            {
                case RunningState.Running:
                    item.RunningState = RunningState.Paused;
                    break;
                case RunningState.Paused:
                    item.RunningState = RunningState.Running;
                    break;
                default:
                    throw new NotImplementedException();
            }

            item.Save();
            UpdateForm();
        }
         * */
    }
}