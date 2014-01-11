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
using Jhu.Graywulf.Web.Util;

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class UserDatabaseInstanceDetails : EntityDetailsPageBase<UserDatabaseInstance>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            Federation.EntityReference.Value = Item.DatabaseVersion.DatabaseDefinition.Federation;
            DatabaseVersion.EntityReference.Value = Item.DatabaseVersion;
            DatabaseInstance.EntityReference.Value = Item.DatabaseInstance;
        }
    }
}