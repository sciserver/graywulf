using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Import : CustomPageBase
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
                RefreshImportMethodList();
            }
        }

        protected void ImportMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (importMethod.SelectedValue == "upload")
            {
                uploadForm.Visible = true;
                uriForm.Visible = false;
                credentialsForm.Visible = false;
            }
            else
            {
                var factory = ImportTablesJobFactory.Create(RegistryContext.Federation);
                var method = factory.GetMethod(importMethod.SelectedValue);

                uploadForm.Visible = false;
                uriForm.Visible = true;
                credentialsForm.Visible = method.HasCredentials;
            }
        }

        protected void ToggleAdvanced_Click(object sender, EventArgs e)
        {
            detailsPanel.Visible = !detailsPanel.Visible;

            if (detailsPanel.Visible)
            {
                toggleAdvanced.Text = "simple mode";
            }
            else
            {
                toggleAdvanced.Text = "advanced mode";
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                if (importMethod.SelectedValue == "upload")
                {
                    ImportUploadedFile();
                    uploadResultsForm.Visible = true;
                }
                else
                {
                    ScheduleImportJob();
                    jobResultsForm.Visible = true;
                }

                importForm.Visible = false;
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

        private void RefreshImportMethodList()
        {
            var factory = ImportTablesJobFactory.Create(RegistryContext.Federation);

            foreach (var method in factory.EnumerateMethods())
            {
                importMethod.Items.Add(new ListItem(method.Description, method.ID));
            }
        }

        private void ImportUploadedFile()
        {
            var uri = uploadForm.Uri;
            var file = fileFormatForm.GetDataFile(uri);
            var table = destinationTableForm.GetDestinationTable();
            var importer = uploadForm.GetTableImporter(file, table);

            importer.Execute();

            foreach (var r in importer.Results)
            {
                var li = new ListItem()
                {
                    Text = String.Format("{0} > {1} ({2})", r.FileName, r.TableName, r.Status)
                };

                resultTableList.Items.Add(li);
            }
        }

        private void ScheduleImportJob()
        {
            var uri = uriForm.Uri;
            var file = fileFormatForm.GetFormat();
            var table = destinationTableForm.GetTableName();
            var credentials = credentialsForm.GetCredentials();

            var job = new ImportJob()
            {
                Uri = uri,
                FileFormat = file,
                Destination = table,
                Credentials = credentialsForm.GetCredentials(),

                Comments = commentsForm.Comments,
                Queue = JobQueue.Long,
            };

            job.Schedule(FederationContext);
        }
    }
}