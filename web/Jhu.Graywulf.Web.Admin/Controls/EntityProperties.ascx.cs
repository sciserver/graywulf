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
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public partial class EntityProperties : UserControlBase
    {
        protected override void OnPreRender(EventArgs e)
        {
            UpdateForm();

            base.OnPreRender(e);
        }

        public void UpdateForm()
        {
            Entity entity = ((IEntityForm)Page).Item;

            FullyQualifiedName.Text = entity.GetFullyQualifiedName();
            EntityGuid.Text = entity.Guid.ToString();
            Version.Text = entity.Version;
            RunningState.Text = entity.RunningState.ToString();
            AlertState.Text = entity.AlertState.ToString();
            DeploymentState.Text = entity.DeploymentState.ToString();
            Comments.Text = entity.Comments;

            System1.Checked = entity.System;
            Hidden.Checked = entity.Hidden;
            ReadOnly.Checked = entity.ReadOnly;
            Locked.Checked = entity.IsLocked;
            Primary.Checked = entity.Primary;
            Deleted.Checked = entity.Deleted;

            UserOwner.EntityReference.Guid = entity.UserGuidOwner;
            UserCreated.EntityReference.Guid = entity.UserGuidCreated;
            UserModified.EntityReference.Guid = entity.UserGuidModified;
            UserDeleted.EntityReference.Guid = entity.UserGuidDeleted;

            DateCreated.Text = entity.DateCreated.ToString();
            DateModified.Text = entity.DateModified.ToString();
            if (entity.Deleted)
            {
                DateDeleted.Text = entity.DateDeleted.ToString();
            }
            else
            {
                DateDeleted.Text = Resources.Labels.NotApplicable;
            }
        }
    }
}