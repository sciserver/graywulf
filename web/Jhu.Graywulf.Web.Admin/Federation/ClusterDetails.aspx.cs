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

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class ClusterDetails : EntityDetailsPageBase<Registry.Cluster>
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Federation/ClusterDetails.aspx?guid={0}", guid);
        }

        protected override void InitLists()
        {
            base.InitLists();

            DatabaseDefinitionList.ParentEntity = item;
            DomainList.ParentEntity = item;
        }

        /*
        protected void DatabaseDefinitionList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/federation/DatabaseDefinitionDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddDatabaseDefinition_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.DatabaseDefinition));
        }

        protected void DomainList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/federation/DomainDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddDomain_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.Domain));
        }*/
    }
}