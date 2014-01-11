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

            QueryFactory.Text = Item.QueryFactory;
            FileFormatFactory.Text = Item.FileFormatFactory;
            StreamFactory.Text = Item.StreamFactory;
            ShortTitle.Text = Item.ShortTitle;
            LongTitle.Text = Item.LongTitle;
            Email.Text = Item.Email;
            Copyright.Text = Item.Copyright;
            Disclaimer.Text = Item.Disclaimer;
            MyDbDatabaseVersion.SelectedValue = Item.MyDBDatabaseVersionReference.Guid.ToString();
            TempDatabaseVersion.SelectedValue = Item.TempDatabaseVersionReference.Guid.ToString();
            CodeDatabaseVersion.SelectedValue = Item.CodeDatabaseVersionReference.Guid.ToString();
            ControllerMachine.SelectedValue = Item.ControllerMachineReference.Guid.ToString();
            SchemaSourceServerInstance.SelectedValue = Item.SchemaSourceServerInstanceReference.Guid.ToString();

            if (!Item.IsExisting)
            {
                MyDbDatabaseVersionRow.Visible = false;

                RefreshServerVersionList();
                MyDbServerVersionRow.Visible = true;
            }
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.QueryFactory = QueryFactory.Text;
            Item.FileFormatFactory = FileFormatFactory.Text;
            Item.StreamFactory = StreamFactory.Text;
            Item.ShortTitle = ShortTitle.Text;
            Item.LongTitle = LongTitle.Text;
            Item.Email = Email.Text;
            Item.Copyright = Copyright.Text;
            Item.Disclaimer = Disclaimer.Text;
            Item.MyDBDatabaseVersionReference.Guid = new Guid(MyDbDatabaseVersion.SelectedValue);
            Item.TempDatabaseVersionReference.Guid = new Guid(TempDatabaseVersion.SelectedValue);
            Item.CodeDatabaseVersionReference.Guid = new Guid(CodeDatabaseVersion.SelectedValue);
            Item.ControllerMachineReference.Guid = new Guid(ControllerMachine.SelectedValue);
            Item.SchemaSourceServerInstanceReference.Guid = new Guid(SchemaSourceServerInstance.SelectedValue);
        }

        protected override void OnItemLoaded(bool newentity)
        {
            if (newentity)
            {
                var fi = new FederationInstaller(Item);
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

                    var fi = new FederationInstaller(Item);
                    fi.GenerateDefaultChildren(sv);
                }
            }
        }

        protected void RefreshMyDbDatabaseVersionList()
        {
            MyDbDatabaseVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Item.LoadDatabaseDefinitions(false);

            foreach (DatabaseDefinition dd in Item.DatabaseDefinitions.Values)
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

            Item.LoadDatabaseDefinitions(false);
            foreach (var dd in Item.DatabaseDefinitions.Values)
            {
                foreach (var dv in dd.DatabaseVersions.Values)
                {
                    TempDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}\\{2}", Item.Name, dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }
        }

        protected void RefreshCodeDatabaseVersionList()
        {
            CodeDatabaseVersion.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Item.LoadDatabaseDefinitions(false);
            foreach (var dd in Item.DatabaseDefinitions.Values)
            {
                foreach (var dv in dd.DatabaseVersions.Values)
                {
                    CodeDatabaseVersion.Items.Add(new ListItem(String.Format("{0}\\{1}\\{2}", Item.Name, dd.Name, dv.Name), dv.Guid.ToString()));
                }
            }
        }

        protected void RefreshControllerMachineList()
        {
            ControllerMachine.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            Item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in Item.Domain.Cluster.MachineRoles.Values)
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

            Item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in Item.Domain.Cluster.MachineRoles.Values)
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

            Item.Domain.Cluster.LoadMachineRoles(false);

            foreach (MachineRole mr in Item.Domain.Cluster.MachineRoles.Values)
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