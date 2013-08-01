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
    public partial class JobDefinitionForm : EntityFormPageBase<JobDefinition>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            TypeName.Text = item.WorkflowTypeName;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.WorkflowTypeName = TypeName.Text;
        }

    }
}