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

namespace Jhu.Graywulf.Web.Admin.Security
{
    public partial class UserGroupForm : EntityFormPageBase<UserGroup>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshLists();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();
        }

        private void RefreshLists()
        {
        }
    }
}