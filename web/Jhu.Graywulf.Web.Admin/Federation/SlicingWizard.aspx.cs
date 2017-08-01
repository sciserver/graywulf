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
    public partial class SlicingWizard : PageBase, IEntityForm
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Federation/SlicingWizard.aspx?guid={0}", guid);
        }

        protected DatabaseDefinition item;
        protected int sliceCount;
        protected int partitionCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            var ef = new EntityFactory(RegistryContext);
            item = ef.LoadEntity<DatabaseDefinition>(new Guid(Request.QueryString["guid"]));

            if (!IsPostBack)
            {
                SliceCount2.Text = item.SliceCount.ToString();
                PartitionCount2.Text = item.PartitionCount.ToString();
                PartitionRangeType.SelectedValue = item.PartitionRangeType.ToString();
            }

            sliceCount = int.Parse(SliceCount2.Text);
            partitionCount = int.Parse(PartitionCount2.Text);

            GenerateTable();
            if (!IsPostBack) GenerateDefaultData();
        }

        protected void GenerateTable()
        {
            TextBox[][] tb = new TextBox[sliceCount + sliceCount * partitionCount + 1][];
            for (int i = 0; i < tb.Length; i++)
            {
                tb[i] = new TextBox[2];
            }

            TableRow tr;
            TableCell cell;
            TextBox t;

            // Add rows
            int q = 0;
            for (int si = 0; si < sliceCount; si++)
            {
                tr = new TableRow();

                cell = new TableCell();
                cell.Text = string.Format("Slice {0}:", si);
                tr.Cells.Add(cell);

                cell = new TableCell();
                t = tb[q][0] = new TextBox();
                t.ID = string.Format("slice_name_{0}", si);
                t.Width = new Unit(120); // TODO
                cell.Controls.Add(t);
                tr.Cells.Add(cell);

                cell = new TableCell();
                tr.Cells.Add(cell);

                LimitsTable.Rows.Add(tr);
                q++;

                for (int pi = 0; pi < partitionCount; pi++)
                {
                    tr = new TableRow();

                    cell = new TableCell();
                    cell.Text = string.Format("Partition {0}:", pi);
                    tr.Cells.Add(cell);

                    cell = new TableCell();
                    t = tb[q][0] = new TextBox();
                    t.ID = string.Format("partition_name_{0}_{1}", si, pi);
                    t.Width = new Unit(120); // TODO
                    cell.Controls.Add(t);
                    tr.Cells.Add(cell);

                    cell = new TableCell();
                    t = tb[q][1] = new TextBox();
                    t.ID = string.Format("partition_limit_{0}_{1}", si, pi);
                    t.Width = new Unit(80); // TODO
                    cell.Controls.Add(t);
                    tr.Cells.Add(cell);

                    LimitsTable.Rows.Add(tr);
                    q++;
                }
            }

            // Closing range
            tr = new TableRow();

            cell = new TableCell();
            cell.Text = "Closing limit:";
            tr.Cells.Add(cell);

            cell = new TableCell();
            tr.Cells.Add(cell);

            cell = new TableCell();
            t = tb[q][1] = new TextBox();
            t.ID = string.Format("closing_limit");
            t.Width = new Unit(80); // TODO
            cell.Controls.Add(t);
            tr.Cells.Add(cell);

            LimitsTable.Rows.Add(tr);
            q++;
        }

        private void GenerateDefaultData()
        {
            TextBox tb;

            for (int si = 0; si < sliceCount; si++)
            {
                tb = (TextBox)FindControlRecursive(string.Format("slice_name_{0}", si));
                tb.Text = string.Format("S{0}", si);

                for (int pi = 0; pi < partitionCount; pi++)
                {
                    tb = (TextBox)FindControlRecursive(string.Format("partition_name_{0}_{1}", si, pi));
                    tb.Text = string.Format("S{0}_P{1}", si, pi);

                    // TODO for testing
                    tb = (TextBox)FindControlRecursive(string.Format("partition_limit_{0}_{1}", si, pi));
                    tb.Text = (si * 1000 + pi * 250).ToString();
                    //
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            string[] slicenames = new string[sliceCount];
            long[][] slicelimits = new long[sliceCount][];
            string[][] partitionnames = new string[sliceCount][];
            long[][][] partitionlimits = new long[sliceCount][][];

            TextBox tb;

            for (int si = 0; si < sliceCount; si++)
            {
                tb = (TextBox)FindControlRecursive(string.Format("slice_name_{0}", si));
                slicenames[si] = tb.Text;

                partitionnames[si] = new string[partitionCount];
                partitionlimits[si] = new long[partitionCount][];
                for (int pi = 0; pi < partitionCount; pi++)
                {
                    tb = (TextBox)FindControlRecursive(string.Format("partition_name_{0}_{1}", si, pi));
                    partitionnames[si][pi] = tb.Text;

                    partitionlimits[si][pi] = new long[2];

                    tb = (TextBox)FindControlRecursive(string.Format("partition_limit_{0}_{1}", si, pi));
                    partitionlimits[si][pi][0] = long.Parse(tb.Text);
                }
            }

            long closing_limit = long.Parse(((TextBox)FindControlRecursive("closing_limit")).Text);

            for (int si = 0; si < sliceCount; si++)
            {
                slicelimits[si] = new long[2];
                slicelimits[si][0] = partitionlimits[si][0][0];

                if (si < sliceCount - 1)
                    slicelimits[si][1] = partitionlimits[si + 1][0][0];
                else
                    slicelimits[si][1] = closing_limit;
            }

            for (int si = 0; si < sliceCount; si++)
            {
                for (int pi = 0; pi < partitionCount; pi++)
                {
                    if (pi < partitionCount - 1)
                        partitionlimits[si][pi][1] = partitionlimits[si][pi + 1][0];
                    else if (si < sliceCount - 1)
                        partitionlimits[si][pi][1] = slicelimits[si + 1][0];
                    else
                        partitionlimits[si][pi][1] = closing_limit;
                }
            }

            var ddi = new DatabaseDefinitionInstaller(item);

            ddi.GenerateSlices(slicenames, slicelimits, partitionnames, partitionlimits);

            Response.Redirect(item.GetDetailsUrl(), false);
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetDetailsUrl(), false);
        }

        #region IEntityForm Members

        public Entity Item
        {
            get { return item; }
        }

        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
        }

        #endregion
    }
}