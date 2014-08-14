using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Import : CopyTablePage
    {
        public static string GetUrl()
        {
            return "~/MyDb/Import.aspx";
        }

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshFileFormatLists();
                RefreshForm();
            }
        }

        protected void ToggleAdvanced_Click(object sender, EventArgs e)
        {
            DetailsTable.Visible = !DetailsTable.Visible;
            RefreshForm();
        }

        protected void FileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshForm();
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // TODO: do work here

                ImportForm.Visible = false;
                ResultsForm.Visible = true;
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.MyDB.Tables.GetUrl());
        }

        #endregion

        protected void RefreshForm()
        {
            if (DetailsTable.Visible)
            {
                ToggleAdvanced.Text = "simple mode";
            }
            else
            {
                ToggleAdvanced.Text = "advanced mode";
            }
        }

        private void RefreshFileFormatLists()
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.CanRead)
                {
                    var li = new ListItem(df.DisplayName, df.Extension);
                    FileFormatList.Items.Add(li);
                    SupportedFormatsList.Items.Add(li);
                }
            }
        }

        private JobInstance ScheduleImportJob()
        {
            var itf = ImportTablesJobFactory.Create(RegistryContext.Federation);

            var uri = new Uri(Uri.Text);
            var destination = GetDestinationTable();
            var parameters = itf.CreateParameters(RegistryContext.Federation, uri, destination);

            var ji = itf.ScheduleAsJob(parameters, Jhu.Graywulf.Registry.Constants.LongQueueName, Comments.Text);

            ji.Save();

            return ji;
        }
    }
}