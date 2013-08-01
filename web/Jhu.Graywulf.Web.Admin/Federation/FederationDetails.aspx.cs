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
    public partial class FederationDetails : EntityDetailsPageBase<Registry.Federation>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            ShortTitle.Text = item.ShortTitle;
            LongTitle.Text = item.LongTitle;
            Email.Text = item.Email;
            MyDbDatabaseVersion.EntityReference.Value = item.MyDBDatabaseVersion;
            TempDatabaseVersion.EntityReference.Value = item.TempDatabaseVersion;
            CodeDatabaseVersion.EntityReference.Value = item.CodeDatabaseVersion;
            ControllerMachine.EntityReference.Value = item.ControllerMachine;
            SchemaSourceServerInstance.EntityReference.Value = item.SchemaSourceServerInstance;
            QueryFactoryTypeName.Text = item.QueryFactoryTypeName;
            FileFormatFactoryTypeName.Text = item.FileFormatFactoryTypeName;
        }

        protected override void InitLists()
        {
            base.InitLists();

            DatabaseDefinitionList.ParentEntity = item;
            RemoteDatabaseList.ParentEntity = item;
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

        protected void RemoteDatabaseList_ItemCommand(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/federation/RemoteDatabaseDetails.aspx?Guid=" + e.CommandArgument);
        }

        protected void AddRemoteDatabase_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetNewChildFormUrl(EntityType.RemoteDatabase));
        }*/
    }
}