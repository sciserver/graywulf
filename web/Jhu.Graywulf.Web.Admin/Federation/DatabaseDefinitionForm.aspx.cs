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
    public partial class DatabaseDefinitionForm : EntityFormPageBase<DatabaseDefinition>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            // Display server version box for new database definitions
            // Default database version will be generated using this
            if (!Item.IsExisting)
            {
                RefreshServerVersionList();
                ServerVersionRow.Visible = true;
            }

            RefreshServerInstanceList();
            SchemaSourceServerInstance.SelectedValue = Item.SchemaSourceServerInstanceReference.Guid.ToString();

            SchemaSourceDatabaseName.Text = Item.SchemaSourceDatabaseName;
            LayoutType.SelectedValue = Item.LayoutType.ToString();
            DatabaseInstanceNamePattern.Text = Item.DatabaseInstanceNamePattern;
            DatabaseNamePattern.Text = Item.DatabaseNamePattern;
            SliceCount.Text = Item.SliceCount.ToString();
            PartitionCount.Text = Item.PartitionCount.ToString();
            PartitionRangeType.SelectedValue = Item.PartitionRangeType.ToString();
            PartitionFunction.Text = Item.PartitionFunction;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.SchemaSourceServerInstanceReference.Guid = new Guid(SchemaSourceServerInstance.SelectedValue);
            Item.SchemaSourceDatabaseName = SchemaSourceDatabaseName.Text;
            Item.LayoutType = (DatabaseLayoutType)Enum.Parse(typeof(DatabaseLayoutType), LayoutType.SelectedValue);
            Item.DatabaseInstanceNamePattern = DatabaseInstanceNamePattern.Text;
            Item.DatabaseNamePattern = DatabaseNamePattern.Text;
            Item.SliceCount = int.Parse(SliceCount.Text);
            Item.PartitionCount = int.Parse(PartitionCount.Text);
            Item.PartitionRangeType = (PartitionRangeType)Enum.Parse(typeof(PartitionRangeType), PartitionRangeType.SelectedValue);
            Item.PartitionFunction = PartitionFunction.Text;
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var sv = new ServerVersion(RegistryContext);
                sv.Guid = new Guid(ServerVersion.SelectedValue);
                sv.Load();

                var ddi = new DatabaseDefinitionInstaller(Item);
                ddi.GenerateDefaultChildren(sv, Registry.Constants.ProdDatabaseVersionName);
            }
        }

        protected void RefreshServerInstanceList()
        {
            SchemaSourceServerInstance.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Item.Federation.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in Item.Federation.Domain.Cluster.MachineRoles.Values)
            {
                mr.LoadMachines(false);
                foreach (Machine m in mr.Machines.Values)
                {
                    m.LoadServerInstances(false);
                    foreach (ServerInstance si in m.ServerInstances.Values)
                    {
                        SchemaSourceServerInstance.Items.Add(new ListItem(m.Name + "\\" + si.Name, si.Guid.ToString()));
                    }
                }
            }
        }

        protected void RefreshServerVersionList()
        {
            ServerVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            var c = Item.Federation.Domain.Cluster;
            c.LoadMachineRoles(false);

            foreach (MachineRole mr in c.MachineRoles.Values)
            {
                mr.LoadServerVersions(false);
                foreach (ServerVersion sv in mr.ServerVersions.Values)
                {
                    ServerVersion.Items.Add(new ListItem(mr.Name + "\\" + sv.Name, sv.Guid.ToString()));
                }
            }
        }

        protected void LayoutType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((DatabaseLayoutType)Enum.Parse(typeof(DatabaseLayoutType), LayoutType.SelectedValue))
            {
                case DatabaseLayoutType.Monolithic:
                    SliceCountRow.Visible =
                        PartitionCountRow.Visible =
                        PartitionRangeTypeRow.Visible =
                        PartitionFunctionRow.Visible = false;

                    DatabaseInstanceNamePattern.Text = Jhu.Graywulf.Registry.Constants.MonolithicDatabaseInstanceNamePattern;
                    DatabaseNamePattern.Text = Jhu.Graywulf.Registry.Constants.MonolithicDatabaseNamePattern;

                    break;
                case DatabaseLayoutType.Mirrored:
                    SliceCountRow.Visible =
                        PartitionCountRow.Visible =
                        PartitionRangeTypeRow.Visible =
                        PartitionFunctionRow.Visible = false;

                    DatabaseInstanceNamePattern.Text = Jhu.Graywulf.Registry.Constants.MirroredDatabaseInstanceNamePattern;
                    DatabaseNamePattern.Text = Jhu.Graywulf.Registry.Constants.MirroredDatabaseNamePattern;

                    break;
                case DatabaseLayoutType.Sliced:
                    SliceCountRow.Visible =
                        PartitionCountRow.Visible =
                        PartitionRangeTypeRow.Visible =
                        PartitionFunctionRow.Visible = true;

                    DatabaseInstanceNamePattern.Text = Jhu.Graywulf.Registry.Constants.SlicedDatabaseInstanceNamePattern;
                    DatabaseNamePattern.Text = Jhu.Graywulf.Registry.Constants.SlicedDatabaseNamePattern;

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}