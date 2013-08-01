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
            if (!item.IsExisting)
            {
                RefreshServerVersionList();
                ServerVersionRow.Visible = true;
            }

            RefreshServerInstanceList();
            SchemaSourceServerInstance.SelectedValue = item.SchemaSourceServerInstanceReference.Guid.ToString();

            SchemaSourceDatabaseName.Text = item.SchemaSourceDatabaseName;
            LayoutType.SelectedValue = item.LayoutType.ToString();
            DatabaseInstanceNamePattern.Text = item.DatabaseInstanceNamePattern;
            DatabaseNamePattern.Text = item.DatabaseNamePattern;
            SliceCount.Text = item.SliceCount.ToString();
            PartitionCount.Text = item.PartitionCount.ToString();
            PartitionRangeType.SelectedValue = item.PartitionRangeType.ToString();
            PartitionFunction.Text = item.PartitionFunction;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.SchemaSourceServerInstanceReference.Guid = new Guid(SchemaSourceServerInstance.SelectedValue);
            item.SchemaSourceDatabaseName = SchemaSourceDatabaseName.Text;
            item.LayoutType = (DatabaseLayoutType)Enum.Parse(typeof(DatabaseLayoutType), LayoutType.SelectedValue);
            item.DatabaseInstanceNamePattern = DatabaseInstanceNamePattern.Text;
            item.DatabaseNamePattern = DatabaseNamePattern.Text;
            item.SliceCount = int.Parse(SliceCount.Text);
            item.PartitionCount = int.Parse(PartitionCount.Text);
            item.PartitionRangeType = (PartitionRangeType)Enum.Parse(typeof(PartitionRangeType), PartitionRangeType.SelectedValue);
            item.PartitionFunction = PartitionFunction.Text;
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var sv = new ServerVersion(RegistryContext);
                sv.Guid = new Guid(ServerVersion.SelectedValue);
                sv.Load();

                var ddi = new DatabaseDefinitionInstaller(item);
                ddi.GenerateDefaultChildren(sv, Registry.Constants.HotDatabaseVersionName);
            }
        }

        protected void RefreshServerInstanceList()
        {
            SchemaSourceServerInstance.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.Federation.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in item.Federation.Domain.Cluster.MachineRoles.Values)
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

            Jhu.Graywulf.Registry.Cluster c;
            if (item.Parent is Jhu.Graywulf.Registry.Cluster)
            {
                c = item.Cluster;
            }
            else if (item.Parent is Jhu.Graywulf.Registry.Federation)
            {
                c = item.Federation.Domain.Cluster;
            }
            else
            {
                throw new NotImplementedException();
            }

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