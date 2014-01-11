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

            ShortTitle.Text = Item.ShortTitle;
            LongTitle.Text = Item.LongTitle;
            Email.Text = Item.Email;
            MyDbDatabaseVersion.EntityReference.Value = Item.MyDBDatabaseVersion;
            TempDatabaseVersion.EntityReference.Value = Item.TempDatabaseVersion;
            CodeDatabaseVersion.EntityReference.Value = Item.CodeDatabaseVersion;
            ControllerMachine.EntityReference.Value = Item.ControllerMachine;
            SchemaSourceServerInstance.EntityReference.Value = Item.SchemaSourceServerInstance;
        }

        protected override void InitLists()
        {
            base.InitLists();

            DatabaseDefinitionList.ParentEntity = Item;
            RemoteDatabaseList.ParentEntity = Item;
        }
    }
}