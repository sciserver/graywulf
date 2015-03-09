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

        protected void ImportMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (importMode.SelectedValue)
            {
                case "upload":
                    uploadForm.Visible = true;
                    uriForm.Visible = false;
                    credentialsForm.Visible = false;
                    break;
                case "fetch":
                    uploadForm.Visible = false;
                    uriForm.Visible = true;
                    credentialsForm.Visible = true;
                    break;
                case "scidrive":
                    uploadForm.Visible = false;
                    uriForm.Visible = true;
                    credentialsForm.Visible = false;
                    break;
                default:
                    throw new NotImplementedException();
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
                switch (importMode.SelectedValue)
                {
                    case "upload":
                        ImportUploadedFile();
                        uploadResultsForm.Visible = true;
                        break;
                    case "fetch":
                        ScheduleImportJob();
                        jobResultsForm.Visible = true;
                        break;
                    case "scidrive":
                        
                        break;
                    default:
                        throw new NotImplementedException();
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