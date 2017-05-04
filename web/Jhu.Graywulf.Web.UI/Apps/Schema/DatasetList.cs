using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public class DatasetList : DropDownList
    {
        public void Populate(FederationContext context)
        {
            Items.Clear();

            // Add MyDB etc. to the beginning of the list
            if (context.RegistryUser != null)
            {
                var uf = UserDatabaseFactory.Create(context);
                var mydbds = uf.GetUserDatabases(context.RegistryUser);

                foreach (var key in mydbds.Keys)
                {
                    var mydbli = CreateListItem(mydbds[key]);
                    mydbli.Attributes.Add("class", "ToolbarControlHighlight");
                    Items.Add(mydbli);
                }
            }

            // Code is the next
            var codedbli = new ListItem(Registry.Constants.CodeDbName, Registry.Constants.CodeDbName);
            codedbli.Attributes.Add("class", "ToolbarControlHighlight");
            Items.Add(codedbli);

            // Add other registered catalogs     
            context.SchemaManager.Datasets.LoadAll(false);

            // TODO: this needs to be modified here, use flags instead of filtering on name!
            foreach (var dsd in context.SchemaManager.Datasets.Values.Where(k =>
                k.Name != Graywulf.Registry.Constants.UserDbName &&
                k.Name != Graywulf.Registry.Constants.CodeDbName &&
                k.Name != Graywulf.Registry.Constants.TempDbName).OrderBy(k => k.Name))
            {
                var li = CreateListItem(dsd);
                Items.Add(li);
            }
        }

        private ListItem CreateListItem(DatasetBase ds)
        {
            var li = new ListItem(ds.Name, ds.Name);

            if (ds.IsInError)
            {
                li.Text += " (not available)";
            }

            return li;
        }
    }
    
}