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

namespace Jhu.Graywulf.Web.Admin.Security
{
    public partial class ClusterDetails : EntityDetailsPageBase<Registry.Cluster>
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Security/ClusterDetails.aspx?guid={0}", guid);
        }

        protected override void InitLists()
        {
            base.InitLists();

            DomainList.ParentEntity = Item;
            UserList.ParentEntity = Item;
            UserGroupList.ParentEntity = Item;
        }

        /*
        protected void DomainList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/security/DomainDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void UserList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/security/UserDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void UserGroupList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/security/UserGroupDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddUser_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.User));
        }

        protected void AddUserGroup_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.UserGroup));
        }*/
    }
}