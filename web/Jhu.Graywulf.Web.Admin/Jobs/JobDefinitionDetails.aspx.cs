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
    public partial class JobDefinitionDetails : EntityDetailsPageBase<JobDefinition>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            TypeName.Text = Item.WorkflowTypeName;

            foreach (var par in Item.Parameters.Values)
            {
                Parameters.Items.Add(string.Format("{0} ({1})", par.Name, par.TypeName));
            }

            CheckpointProgress.Checkpoints = Item.Checkpoints;
        }

    }
}