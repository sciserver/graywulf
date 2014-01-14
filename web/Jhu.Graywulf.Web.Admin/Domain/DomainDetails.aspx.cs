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
using Jhu.Graywulf.Web.Admin;

namespace Jhu.Graywulf.Web.Admin.Domain
{
    public partial class DomainDetails : EntityDetailsPageBase<Registry.Domain>
    {
        protected override void InitLists()
        {
            base.InitLists();

            UserList.ParentEntity = Item;
            UserGroupList.ParentEntity = Item;
        }
    }
}