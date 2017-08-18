using System;
using System.Collections.Generic;
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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class MappingWizard : PageBase, IEntityForm
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Layout/MappingWizard.aspx?guid={0}", guid);
        }

        protected DatabaseDefinition item;
        protected DatabaseVersion databaseVersion;
        protected ServerVersion serverVersion;
        protected List<ServerInstance> serverInstances;
        protected List<Slice> slices;

        public Entity Item
        {
            get { return item; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var ef = new EntityFactory(RegistryContext);
            item = ef.LoadEntity<DatabaseDefinition>(new Guid(Request.QueryString["guid"]));

            if (!IsPostBack)
            {
                RefreshDatabaseVersionList();
                RefreshServerVersionList();
            }

            // Load currently selected database version
            databaseVersion = ef.LoadEntity<DatabaseVersion>(new Guid(databaseVersionList.SelectedValue));

            if (!IsPostBack)
            {
                serverVersionList.SelectedValue = databaseVersion.ServerVersion.Guid.ToString();
            }

            // Load currently selected server version
            serverVersion = ef.LoadEntity<ServerVersion>(new Guid(serverVersionList.SelectedValue));

            // Load server instances
            serverInstances = new List<ServerInstance>(ef.FindAll<ServerInstance>()
                .Where(i => i.ServerVersionReference.Guid == serverVersion.Guid)
                .OrderBy(i => i.Machine.Number)
                .ThenBy(i => i.Number));

            // Load slices
            item.LoadSlices(false);
            slices = new List<Slice>(item.Slices.Values);

            if (!IsPostBack)
            {
                UpdateForm();
            }

            GenerateTable();
        }

        protected void UpdateForm()
        {
            Name.Text = item.Name;
            LayoutType.Text = item.LayoutType.ToString();
            SizeFactor.Text = databaseVersion.SizeMultiplier.ToString();
            NamePattern.Text = item.DatabaseInstanceNamePattern;
            DatabaseNamePattern.Text = item.DatabaseNamePattern;
        }

        protected void RefreshDatabaseVersionList()
        {
            databaseVersionList.Items.Clear();

            item.LoadDatabaseVersions(false);

            foreach (DatabaseVersion rs in item.DatabaseVersions.Values)
            {
                ListItem li = new ListItem(rs.Name, rs.Guid.ToString());
                databaseVersionList.Items.Add(li);
            }
        }

        protected void RefreshServerVersionList()
        {
            serverVersionList.Items.Clear();

            var cluster = RegistryContext.Cluster;
            cluster.LoadMachineRoles(false);

            foreach (var mr in cluster.MachineRoles.Values)
            {
                mr.LoadServerVersions(false);

                foreach (var sv in mr.ServerVersions.Values)
                {
                    ListItem li = new ListItem(mr.Name + "/" + sv.Name, sv.Guid.ToString());
                    serverVersionList.Items.Add(li);
                }
            }
        }

        protected void GenerateTable()
        {
            CheckBox[][] cb = new CheckBox[slices.Count][];
            for (int i = 0; i < cb.Length; i++)
            {
                cb[i] = new CheckBox[serverInstances.Count];
            }

            TableCell cell;

            // Add header (slices in columns)
            TableRow header = new TableRow();
            for (int sli = 0; sli < slices.Count; sli++)
            {
                if (sli == 0) header.Cells.Add(new TableCell());    // corner cell

                cell = new TableCell();
                cell.Text = slices[sli].Name;
                header.Cells.Add(cell);
            }
            MappingTable.Rows.Add(header);

            // Add rows (server instances in rows)
            for (int sii = 0; sii < serverInstances.Count; sii++)
            {
                TableRow tr = new TableRow();

                cell = new TableCell();
                cell.Text = serverInstances[sii].Machine.Name + Constants.EntityNameSeparator + serverInstances[sii].Name;
                tr.Cells.Add(cell);

                for (int sli = 0; sli < slices.Count; sli++)
                {
                    cell = new TableCell();

                    CheckBox c = cb[sli][sii] = new CheckBox();
                    c.ID = string.Format("cb_{0}_{1}", sli, sii);
                    c.Enabled = true;
                    c.Checked = true;

                    cell.Controls.Add(c);
                    tr.Cells.Add(cell);
                }

                MappingTable.Rows.Add(tr);
            }

            // Find already mapped instances
            item.LoadDatabaseInstances(false);
            foreach (DatabaseInstance di in item.DatabaseInstances.Values.Where(x => x.DatabaseVersionReference.Guid == databaseVersion.Guid))
            {
                int i = slices.FindIndex(x => x.Guid == di.SliceReference.Guid);
                int j = serverInstances.FindIndex(x => x.Guid == di.ServerInstanceReference.Guid);

                //cb[i][j].Checked = false;
                //cb[i][j].Enabled = false;
            }
        }

        protected void DatabaseVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            SizeFactor.Text = databaseVersion.SizeMultiplier.ToString();
        }

        protected void ServerVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            // TODO: add serverNumber or similar to add database instances to the layout
            // when some are already in place

            double sizefactor = double.Parse(SizeFactor.Text);
            string postfix = String.Empty;

            var ef = new EntityFactory(RegistryContext);
            var databaseVersion = ef.LoadEntity<DatabaseVersion>(new Guid(databaseVersionList.SelectedValue));

            databaseVersion.DatabaseDefinition.LoadDatabaseInstances(false);

            int q = 0;
            for (int sli = 0; sli < slices.Count; sli++)
            {
                for (int ssi = 0; ssi < serverInstances.Count; ssi++)
                {
                    CheckBox cb = (CheckBox)FindControlRecursive(string.Format("cb_{0}_{1}", sli, ssi));
                    if (cb.Checked)
                    {
                        switch (item.LayoutType)
                        {
                            case DatabaseLayoutType.Sliced:
                                break;
                            default:
                                postfix = String.Format("{0}", q.ToString("00"));
                                break;
                        }

                        var name = NamePattern.Text.Replace("[$Number]", postfix);
                        var dbname = DatabaseNamePattern.Text.Replace("[$Number]", postfix);

                        // Check for name duplicate
                        if (SkipDuplicates.Checked)
                        {
                            var ndi = new DatabaseInstance(databaseVersion.DatabaseDefinition);
                            ndi.DatabaseVersion = databaseVersion;
                            ndi.ServerInstance = serverInstances[ssi];
                            var nname = ExpressionProperty.ResolveExpression(ndi, name);

                            if (databaseVersion.DatabaseDefinition.DatabaseInstances.ContainsKey(nname))
                            {
                                q++;
                                continue;
                            }
                        }

                        var dii = new DatabaseInstanceInstaller(item);
                        dii.GenerateDatabaseInstance(
                            serverInstances[ssi],
                            slices[sli],
                            databaseVersion,
                            name,
                            dbname,
                            sizefactor,
                            GenerateFileGroups.Checked);

                        q++;
                    }
                }
            }

            Response.Redirect(item.GetDetailsUrl(EntityGroup.Layout), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetDetailsUrl(EntityGroup.Layout), false);
        }


        #region IEntityForm Members


        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}