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
        }

        protected override void InitLists()
        {
            base.InitLists();

            DatabaseDefinitionList.ParentEntity = item;
            RemoteDatabaseList.ParentEntity = item;
        }
    }
}