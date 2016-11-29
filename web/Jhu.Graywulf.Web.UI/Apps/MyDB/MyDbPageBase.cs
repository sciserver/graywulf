using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public class MyDbPageBase : FederationPageBase
    {
        protected void RefreshDatasetList(ListControl datasetList)
        {
            var uf = UserDatabaseFactory.Create(RegistryContext.Federation);
            var mydbds = uf.GetUserDatabases(RegistryUser);

            datasetList.Items.Clear();

            foreach (var key in mydbds.Keys)
            {
                var mydbli = new ListItem(key, key);
                datasetList.Items.Add(mydbli);
            }

            if (Request["dataset"] != null)
            {
                datasetList.SelectedValue = Request["dataset"];
            }
            else if (Request["objid"] != null)
            {
                DatabaseObjectType type;
                string datasetName, databaseName, schemaName, objectName;

                FederationContext.SchemaManager.GetNamePartsFromKey(
                    Request["objid"],
                    out type,
                    out datasetName,
                    out databaseName,
                    out schemaName,
                    out objectName);

                datasetList.SelectedValue = datasetName;
            }
        }
    }
}