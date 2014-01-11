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

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            RefreshMyDbDatabaseVersionList();
            RefreshTempDatabaseVersionList();
            RefreshCodeDatabaseVersionList();
            RefreshControllerMachineList();
            RefreshServerInstanceList();

            QueryFactory.Text = item.QueryFactory;
            FileFormatFactory.Text = item.FileFormatFactory;
            StreamFactory.Text = item.StreamFactory;
            ShortTitle.Text = item.ShortTitle;
            LongTitle.Text = item.LongTitle;
            Email.Text = item.Email;
            Copyright.Text = item.Copyright;
            Disclaimer.Text = item.Disclaimer;
            MyDbDatabaseVersion.SelectedValue = item.MyDBDatabaseVersionReference.Guid.ToString();
            TempDatabaseVersion.SelectedValue = item.TempDatabaseVersionReference.Guid.ToString();
            CodeDatabaseVersion.SelectedValue = item.CodeDatabaseVersionReference.Guid.ToString();
            ControllerMachine.SelectedValue = item.ControllerMachineReference.Guid.ToString();
            SchemaSourceServerInstance.SelectedValue = item.SchemaSourceServerInstanceReference.Guid.ToString();

            if (!item.IsExisting)
            {
                MyDbDatabaseVersionRow.Visible = false;

                RefreshServerVersionList();
                MyDbServerVersionRow.Visible = true;
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.QueryFactory = QueryFactory.Text;
            item.FileFormatFactory = FileFormatFactory.Text;
            item.StreamFactory = StreamFactory.Text;
            item.ShortTitle = ShortTitle.Text;
            item.LongTitle = LongTitle.Text;
            item.Email = Email.Text;
            item.Copyright = Copyright.Text;
            item.Disclaimer = Disclaimer.Text;
            item.MyDBDatabaseVersionReference.Guid = new Guid(MyDbDatabaseVersion.SelectedValue);
            item.TempDatabaseVersionReference.Guid = new Guid(TempDatabaseVersion.SelectedValue);
            item.CodeDatabaseVersionReference.Guid = new Guid(CodeDatabaseVersion.SelectedValue);
            item.ControllerMachineReference.Guid = new Guid(ControllerMachine.SelectedValue);
            item.SchemaSourceServerInstanceReference.Guid = new Guid(SchemaSourceServerInstance.SelectedValue);
        }

        protected override void OnItemCreated(bool newentity)
        {
            if (newentity)
            {
                var fi = new FederationInstaller(item);
                fi.GenerateDefaultSettings();
            }
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var svguid = new Guid(MyDbServerVersion.SelectedValue);

                if (svguid != Guid.Empty)
                {
                    var sv = new ServerVersion(RegistryContext);
                    sv.Guid = new Guid(MyDbServerVersion.SelectedValue);
                    sv.Load();

                    var fi = new FederationInstaller(item);
                    fi.GenerateDefaultChildren(sv);
                }
            }
        }

        protected void RefreshMyDbDatabaseVersionList()
        {
            MyDbDatabaseVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.LoadDatabaseDefinitions(false);

            foreach (DatabaseDefinition dd in item.DatabaseDefinitions.Values)
            {
                dd.LoadDatabaseVersions(false);

                foreach (DatabaseVersion dv in dd.DatabaseVersions.Values)
                {
                    MyDbDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}", dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }
        }

        protected void RefreshTempDatabaseVersionList()
        {
            TempDatabaseVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Cluster.LoadDatabaseDefinitions(false);
            foreach (var dd in Cluster.DatabaseDefinitions.Values)
            {
                dd.LoadDatabaseVersions(false);

                foreach (var dv in dd.DatabaseVersions.Values)
                {
                    TempDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}\\{2}", Cluster.Name, dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }

            item.LoadDatabaseDefinitions(false);
            foreach (var dd in item.DatabaseDefinitions.Values)
            {
                foreach (var dv in dd.DatabaseVersions.Values)
                {
                    TempDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}\\{2}", item.Name, dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }
        }

        protected void RefreshCodeDatabaseVersionList()
        {
            CodeDatabaseVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.LoadDatabaseDefinitions(false);
            foreach (var dd in item.DatabaseDefinitions.Values)
            {
                foreach (var dv in dd.DatabaseVersions.Values)
                {
                    CodeDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}\\{2}", item.Name, dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }
        }

        protected void RefreshControllerMachineList()
        {
            ControllerMachine.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in item.Domain.Cluster.MachineRoles.Values)
            {
                mr.LoadMachines(false);
                foreach (Machine m in mr.Machines.Values)
                {
                    ControllerMachine.Items.Add(new ListItem(m.Name, m.Guid.ToString()));
                }
            }
        }

        protected void RefreshServerInstanceList()
        {
            SchemaSourceServerInstance.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in item.Domain.Cluster.MachineRoles.Values)
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
            MyDbServerVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in item.Domain.Cluster.MachineRoles.Values)
            {
                mr.LoadServerVersions(false);
                foreach (ServerVersion sv in mr.ServerVersions.Values)
                {
                    MyDbServerVersion.Items.Add(new ListItem(mr.Name + "\\" + sv.Name, sv.Guid.ToString()));
                }
            }
        }

    }
}