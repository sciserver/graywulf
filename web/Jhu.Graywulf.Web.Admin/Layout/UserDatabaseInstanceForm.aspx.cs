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
    public partial class UserDatabaseInstanceForm : EntityFormPageBase<UserDatabaseInstance>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            if (Item.User.Domain == null)
            {
                throw new InvalidOperationException();
            }

            RefreshFederationList();
            if (Item.IsExisting)
            {
                Federation.SelectedValue = Item.DatabaseVersion.DatabaseDefinition.Federation.Guid.ToString();
            }
            
            RefreshDatabaseVersionList();
            if (Item.IsExisting)
            {
                DatabaseVersion.SelectedValue = Item.DatabaseVersion.Guid.ToString();
            }

            RefreshDatabaseInstanceList();
            if (Item.IsExisting)
            {
                DatabaseInstance.SelectedValue = Item.DatabaseInstance.Guid.ToString();
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.DatabaseVersionReference.Guid = new Guid(DatabaseVersion.SelectedValue);
            Item.DatabaseInstanceReference.Guid = new Guid(DatabaseInstance.SelectedValue);
        }

        private void RefreshFederationList()
        {
            Federation.Items.Clear();
            Federation.Items.Add(new ListItem("(select federation", Guid.Empty.ToString()));

            Item.User.Domain.LoadFederations(false);
            foreach (var f in Item.User.Domain.Federations.Values)
            {
                Federation.Items.Add(new ListItem(f.Name, f.Guid.ToString()));
            }
        }

        private void RefreshDatabaseVersionList()
        {
            DatabaseVersion.Items.Clear();
            DatabaseVersion.Items.Add(new ListItem("(select database version)", Guid.Empty.ToString()));

            if (Federation.SelectedValue != Guid.Empty.ToString())
            {
                var f = new Registry.Federation(RegistryContext);
                f.Guid = new Guid(Federation.SelectedValue);
                f.Load();

                f.LoadDatabaseDefinitions(false);
                foreach (var dd in f.DatabaseDefinitions.Values)
                {
                    dd.LoadDatabaseVersions(false);

                    foreach (var dv in dd.DatabaseVersions.Values)
                    {
                        DatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}", dd.Name, dv.Name), dv.Guid.ToString()));
                    }
                }
            }
        }

        private void RefreshDatabaseInstanceList()
        {
            DatabaseInstance.Items.Clear();
            DatabaseInstance.Items.Add(new ListItem("(select database instance)", Guid.Empty.ToString()));

            if (DatabaseVersion.SelectedValue != Guid.Empty.ToString())
            {
                var dv = new Registry.DatabaseVersion(RegistryContext);
                dv.Guid = new Guid(DatabaseVersion.SelectedValue);
                dv.Load();

                var dd = dv.DatabaseDefinition;
                dd.LoadDatabaseInstances(false);
                foreach (var di in dd.DatabaseInstances.Values.Where(dii => dii.DatabaseVersionReference.Guid == dv.Guid))
                {
                    DatabaseInstance.Items.Add(new ListItem(di.Name, di.Guid.ToString()));
                }
            }
        }

        protected void Federation_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDatabaseVersionList();
        }

        protected void DatabaseDefinition_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDatabaseInstanceList();
        }
    }
}