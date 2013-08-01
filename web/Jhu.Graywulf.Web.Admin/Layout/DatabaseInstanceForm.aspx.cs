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
    public partial class DatabaseInstanceForm : EntityFormPageBase<DatabaseInstance>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshSliceList();
            RefreshDatabaseVersionList();
            RefreshServerInstanceList();

            UpdateResolvedNames();

            DatabaseName.Text = item.DatabaseName;
            Slice.SelectedValue = item.SliceReference.Guid.ToString();
            DatabaseVersion.SelectedValue = item.DatabaseVersionReference.Guid.ToString();
            ServerInstance.SelectedValue = item.ServerInstanceReference.Guid.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.DatabaseName = DatabaseName.Text;
            item.SliceReference.Guid = new Guid(Slice.SelectedValue);
            item.ServerInstanceReference.Guid = new Guid(ServerInstance.SelectedValue);
            item.DatabaseVersionReference.Guid = new Guid(DatabaseVersion.SelectedValue);
        }

        protected void RefreshSliceList()
        {
            Slice.Items.Clear();
            item.DatabaseDefinition.LoadSlices(false);

            Slice.Items.Add(new ListItem("(select slice)", Guid.Empty.ToString()));
            foreach (Slice s in item.DatabaseDefinition.Slices.Values)
            {
                Slice.Items.Add(new ListItem(s.Name, s.Guid.ToString()));
            }
        }

        protected void RefreshDatabaseVersionList()
        {
            DatabaseVersion.Items.Clear();
            item.DatabaseDefinition.LoadDatabaseVersions(false);

            DatabaseVersion.Items.Add(new ListItem("(select version)", Guid.Empty.ToString()));
            foreach (DatabaseVersion r in item.DatabaseDefinition.DatabaseVersions.Values)
            {
                DatabaseVersion.Items.Add(new ListItem(r.Name, r.Guid.ToString()));
            }
        }

        protected void RefreshServerInstanceList()
        {
#if false
            ServerInstance.Items.Clear();

            ServerInstance.Items.Add(new ListItem("(select server instance)", Guid.Empty.ToString()));

            if (DatabaseVersion.SelectedValue != Guid.Empty.ToString())
            {
                DatabaseVersion dv = new DatabaseVersion(RegistryContext);
                dv.Guid = new Guid(DatabaseVersion.SelectedValue);
                dv.Load();

                EntityFactory ef = new EntityFactory(RegistryContext);
                foreach (ServerInstance si in ef.FindAll<ServerInstance>().Where(i => i.ServerVersionReference.Guid == dv.ServerVersionReference.Guid))
                {
                    ServerInstance.Items.Add(new ListItem(si.Machine.Name + "\\" + si.Name, si.Guid.ToString()));
                }
            }
#endif

            ServerInstance.Items.Add(new ListItem("(select server instance)", Guid.Empty.ToString()));

            Cluster.LoadMachineRoles(false);

            foreach (var mr in Cluster.MachineRoles.Values)
            {
                mr.LoadMachines(false);
                foreach (var m in mr.Machines.Values)
                {
                    m.LoadServerInstances(false);
                    foreach (var si in m.ServerInstances.Values)
                    {
                        ServerInstance.Items.Add(new ListItem(m.Name + "\\" + si.Name, si.Guid.ToString()));
                    }
                }
            }
        }

        protected void UpdateResolvedNames()
        {
            if (!item.IsExisting)
            {
                item.SliceReference.Guid = new Guid(Slice.SelectedValue);
                item.DatabaseVersionReference.Guid = new Guid(DatabaseVersion.SelectedValue);
                item.ServerInstanceReference.Guid = new Guid(ServerInstance.SelectedValue);

                try
                {
                    item.Name = Registry.Util.ResolveExpression(item, item.DatabaseDefinition.DatabaseInstanceNamePattern);
                    item.DatabaseName = Registry.Util.ResolveExpression(item, item.DatabaseDefinition.DatabaseNamePattern);
                }
                catch (ArgumentNullException)
                {
                    item.Name = String.Empty;
                    item.DatabaseName = String.Empty;
                }
            }
        }

        protected void DatabaseVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshServerInstanceList();
        }

        protected void ServerInstance_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateResolvedNames();

            EntityForm.Name.Text = item.Name;
            DatabaseName.Text = item.DatabaseName;
        }


    }
}