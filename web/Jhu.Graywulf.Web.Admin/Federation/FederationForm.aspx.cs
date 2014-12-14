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
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class FederationForm : EntityFormPageBase<Registry.Federation>
    {
        protected Registry.Cluster cluster;

        protected override void OnItemLoaded(bool newentity)
        {
            if (newentity)
            {
                var fi = new FederationInstaller(Item);
                fi.GenerateDefaultSettings();
            }
        }

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshLists();

            SchemaManager.Text = Item.SchemaManager;
            UserDatabaseFactory.Text = Item.UserDatabaseFactory;
            QueryFactory.Text = Item.QueryFactory;
            FileFormatFactory.Text = Item.FileFormatFactory;
            StreamFactory.Text = Item.StreamFactory;
            ShortTitle.Text = Item.ShortTitle;
            LongTitle.Text = Item.LongTitle;
            Email.Text = Item.Email;
            Copyright.Text = Item.Copyright;
            Disclaimer.Text = Item.Disclaimer;

            ControllerMachine.SelectedValue = Item.ControllerMachine;
            SchemaSourceServerInstance.SelectedValue = Item.SchemaSourceServerInstance;
            MyDbDatabaseVersion.SelectedValue = Item.UserDatabaseVersion;
            TempDatabaseVersion.SelectedValue = Item.TempDatabaseVersion;
            CodeDatabaseVersion.SelectedValue = Item.CodeDatabaseVersion;

        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.SchemaManager = SchemaManager.Text;
            Item.UserDatabaseFactory = UserDatabaseFactory.Text;
            Item.QueryFactory = QueryFactory.Text;
            Item.FileFormatFactory = FileFormatFactory.Text;
            Item.StreamFactory = StreamFactory.Text;
            Item.ShortTitle = ShortTitle.Text;
            Item.LongTitle = LongTitle.Text;
            Item.Email = Email.Text;
            Item.Copyright = Copyright.Text;
            Item.Disclaimer = Disclaimer.Text;

            Item.ControllerMachine = (Machine) ControllerMachine.SelectedValue;
            Item.UserDatabaseVersion = (DatabaseVersion)MyDbDatabaseVersion.SelectedValue;
            Item.TempDatabaseVersion = (DatabaseVersion)TempDatabaseVersion.SelectedValue;
            Item.CodeDatabaseVersion = (DatabaseVersion) CodeDatabaseVersion.SelectedValue;
            Item.SchemaSourceServerInstance = (ServerInstance)SchemaSourceServerInstance.SelectedValue;
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                if (MyDbServerVersion.SelectedValue != null &&
                    NodeServerVersion.SelectedValue != null)
                {
                    var fi = new FederationInstaller(Item);
                    fi.GenerateDefaultChildren(
                        (ServerVersion)MyDbServerVersion.SelectedValue,
                        (ServerVersion)NodeServerVersion.SelectedValue);
                }
            }
        }

        private void RefreshLists()
        {
            // Set visibility of parts of the form depending whether
            // this is a new federation or an exisiting one
            if (!Item.IsExisting)
            {
                // MyDB version will be automatically generated
                MyDbServerVersionRow.Visible = true;
                MyDbDatabaseVersionRow.Visible = false;

                // CodeDB will also be automatically generated
                NodeServerVersionRow.Visible = true;
                TempDatabaseVersionRow.Visible = true;
                CodeDatabaseVersionRow.Visible = false;
            }
            else
            {
                MyDbServerVersionRow.Visible = false;
                MyDbDatabaseVersionRow.Visible = true;

                TempDatabaseVersionRow.Visible = true;
                CodeDatabaseVersionRow.Visible = true;
            }

            if (ControllerMachine.Visible)
            {
                ControllerMachine.ChildEntityTypes = new[] { EntityType.MachineRole, EntityType.Machine };
                ControllerMachine.ParentEntity = Item.Domain.Cluster;
            }

            if (SchemaSourceServerInstance.Visible)
            {
                SchemaSourceServerInstance.ChildEntityTypes = new[] { EntityType.MachineRole, EntityType.Machine, EntityType.ServerInstance };
                SchemaSourceServerInstance.ParentEntity = Item.Domain.Cluster;
            }

            if (MyDbServerVersion.Visible)
            {
                MyDbServerVersion.ChildEntityTypes = new[] { EntityType.MachineRole, EntityType.ServerVersion };
                MyDbServerVersion.ParentEntity = Item.Domain.Cluster;
            }

            if (MyDbDatabaseVersion.Visible)
            {
                MyDbDatabaseVersion.ChildEntityTypes = new[] { EntityType.DatabaseDefinition, EntityType.DatabaseVersion };
                MyDbDatabaseVersion.ParentEntity = Item;
            }

            if (NodeServerVersion.Visible)
            {
                NodeServerVersion.ChildEntityTypes = new[] { EntityType.MachineRole, EntityType.ServerVersion };
                NodeServerVersion.ParentEntity = Item.Domain.Cluster;
            }

            // TODO: This can be done on the cluster or on the federation level!
            if (TempDatabaseVersion.Visible)
            {
                var c = Item.Domain.Cluster;
                c.LoadDomains(false);
                var d = c.Domains[Registry.Constants.SystemDomainName];
                d.LoadFederations(false);
                var f = d.Federations[Registry.Constants.SystemFederationName];

                TempDatabaseVersion.ChildEntityTypes = new[] { EntityType.DatabaseDefinition, EntityType.DatabaseVersion };
                TempDatabaseVersion.ParentEntity = f;
            }

            // This is always on the federation level
            if (CodeDatabaseVersion.Visible)
            {
                CodeDatabaseVersion.ChildEntityTypes = new[] { EntityType.DatabaseDefinition, EntityType.DatabaseVersion };
                CodeDatabaseVersion.ParentEntity = Item;
            }
        }
    }
}