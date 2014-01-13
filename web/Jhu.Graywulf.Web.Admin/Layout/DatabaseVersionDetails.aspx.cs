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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class DatabaseVersionDetails : EntityDetailsPageBase<DatabaseVersion>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            ServerVersion.EntityReference.Value = Item.ServerVersion;
        }

        protected override void InitLists()
        {
            base.InitLists();

            UserDatabaseInstanceList.ParentEntity = Item;
        }
    }
}