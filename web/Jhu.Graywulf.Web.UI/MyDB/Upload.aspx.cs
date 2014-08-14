using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Upload : CopyTablePage
    {
        public static string GetUrl()
        {
            return "~/MyDb/Upload.aspx";
        }

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshFileFormatLists(true, false);
                RefreshForm();
            }
        }

        protected void FileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshForm();
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Run import
                CopyTableBase importer = GetTableImporter();
                importer.Execute();

                foreach (var r in importer.Results)
                {
                    var li = new ListItem()
                    {
                        Text = String.Format("{0} > {1} ({2})", r.FileName, r.TableName, r.Status)
                    };
                    
                    ResultTableList.Items.Add(li);
                }

                UploadForm.Visible = false;
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

        private void RefreshForm()
        {
        }

        private Uri GetUploadedFileUri()
        {
            // Now this is tricky here because we don't know anything about the
            // file path format the browser sends us. Lets's assume it has a '/', '\' or ':'
            // in the path before the file name, or if not, the whole string is
            // a file name.

            var filename = ImportedFile.PostedFile.FileName;
            int i;

            i = filename.LastIndexOf('/');
            i = Math.Max(i, filename.LastIndexOf('\\'));
            i = Math.Max(i, filename.LastIndexOf(':'));

            if (i >= 0)
            {
                filename = filename.Substring(i + 1);
            }

            // TODO: test this method with all browses and various types
            // containing fancy characters

            return new Uri(filename, UriKind.Relative);
        }

        private CopyTableBase GetTableImporter()
        {
            var uri = GetUploadedFileUri();

            // Check if uploaded file is an archive
            var archival = FederationContext.StreamFactory.GetArchivalMethod(uri);
            var compression = FederationContext.StreamFactory.GetCompressionMethod(uri);
            var batchName = Util.UriConverter.ToFileNameWithoutExtension(uri);

            if (archival == DataFileArchival.None)
            {
                // A single file is being uploaded
                var file = GetSourceDataFile(uri);
                
                file.BaseStream = FederationContext.StreamFactory.Open(
                    ImportedFile.PostedFile.InputStream,
                    DataFileMode.Read,
                    file.Compression,
                    DataFileArchival.None);

                // Use simple importer
                var task = new ImportTable()
                {
                    BatchName = batchName,
                    Source = file,
                    Destination = GetDestinationTable(),
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                };

                task.Open();

                return task;
            }
            else
            {
                // An archive is being uploaded, use archive importer
                var task = new ImportTableArchive()
                {
                    BypassExceptions = true,
                    BatchName = batchName,
                    Destination = GetDestinationTable(),
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                };

                task.Open(FederationContext.StreamFactory.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read, compression, archival));

                return task;
            }
        }
    }
}